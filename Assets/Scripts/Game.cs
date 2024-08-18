using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public const string HighestCompletedLevelPrefsKey = "ktb_highest_completed_level";
    public const string SoundPrefsKey = "ktb_sound";

    public CanvasGroup loadscreen;
    public GameObject titleScreen;
    public GameObject gameUI;
    public CameraFollow cameraFollow;

    public GameObject continueButton;
    public GameObject quitButton;

    [Scene] public int[] levels;
    private int _currentLevelIndex = 0;
    private Level _currentLevel;

    private int _highestCompletedLevel;

    private void Start()
    {
        UpdateContinueButton();

#if UNITY_WEBGL
        quitButton.SetActive(false);
#endif
    }

    private void UpdateContinueButton()
    {
        _highestCompletedLevel = PlayerPrefs.GetInt(HighestCompletedLevelPrefsKey, -1);
        continueButton.SetActive(_highestCompletedLevel >= 0 && _highestCompletedLevel < levels.Length - 1);
    }

    public void OnNewGameClicked()
    {
        loadscreen.gameObject.SetActive(true);
        titleScreen.SetActive(false);

        StartCoroutine(LoadLevelAsync(0));
    }

    public void OnContinueClicked()
    {
        loadscreen.gameObject.SetActive(true);
        titleScreen.SetActive(false);

        StartCoroutine(LoadLevelAsync(_highestCompletedLevel + 1));
    }

    private IEnumerator LoadLevelAsync(int levelIndex)
    {
        _currentLevelIndex = levelIndex;
        var buildIndex = levels[levelIndex];

        var asyncOperation = SceneManager.LoadSceneAsync(buildIndex, LoadSceneMode.Additive);
        while (!asyncOperation.isDone)
        {
            yield return null;
        }

        var levelScene = SceneManager.GetSceneByBuildIndex(buildIndex);
        var catController = GetComponentOnScene<CatController>(levelScene);
        cameraFollow.target = catController.transform;

        _currentLevel = GetComponentOnScene<Level>(levelScene);
        _currentLevel.onCompleted += OnLevelCompleted;

        gameUI.gameObject.SetActive(true);
        loadscreen.gameObject.SetActive(false);
    }

    private void OnLevelCompleted()
    {
        _currentLevel.onCompleted -= OnLevelCompleted;

        _highestCompletedLevel = _currentLevelIndex;
        PlayerPrefs.SetInt(HighestCompletedLevelPrefsKey, _highestCompletedLevel);
        UpdateContinueButton();

        StartCoroutine(NextLevelCoroutine());
    }

    private IEnumerator NextLevelCoroutine()
    {
        loadscreen.gameObject.SetActive(true);

        yield return UnloadLevel(_currentLevelIndex);

        if (++_currentLevelIndex <= levels.Length - 1)
        {
            yield return LoadLevelAsync(_currentLevelIndex);
        }
        else
        {
            Debug.LogWarning("TODO: Game over, roll credits");
            titleScreen.SetActive(true);
            gameUI.gameObject.SetActive(false);
            loadscreen.gameObject.SetActive(false);
        }
    }

    private IEnumerator UnloadLevel(int levelIndex)
    {
        var buildIndex = levels[levelIndex];
        var asyncOperation = SceneManager.UnloadSceneAsync(buildIndex);
        while (!asyncOperation.isDone)
        {
            yield return null;
        }
    }

    public T GetComponentOnScene<T>(Scene scene)
    {
        foreach (var rootGameObject in scene.GetRootGameObjects())
        {
            var component = rootGameObject.GetComponentInChildren<T>(true);
            if (component != null)
                return component;
        }

        return default;
    }

    public void Restart()
    {
        StartCoroutine(RestartCoroutine());
    }

    private IEnumerator RestartCoroutine()
    {
        loadscreen.gameObject.SetActive(true);

        yield return UnloadLevel(_currentLevelIndex);
        yield return LoadLevelAsync(_currentLevelIndex);
    }

    public void Quit()
    {
#if !UNITY_WEBGL
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
#endif
    }
}
