using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public const string HighestCompletedLevelPrefsKey = "ktb_highest_completed_level";
    public const string LastCompletedLevelPrefsKey = "ktb_last_completed_level";
    public const string SoundPrefsKey = "ktb_sound";

    public CanvasGroup loadscreen;
    public GameObject titleScreen;
    public GameObject[] gameUI;
    public GameObject creditsScreen;
    public CameraFollow cameraFollow;

    public GameObject continueButton;
    public GameObject quitButton;

    [Scene] public int[] levels;
    private int _currentLevelIndex = 0;
    private Level _currentLevel;

    private int _lastCompletedLevel;

    private void Start()
    {
        UpdateContinueButton();

        SetGameUIActive(false);

#if UNITY_WEBGL
        quitButton.SetActive(false);
#endif
    }

    private void UpdateContinueButton()
    {
        _lastCompletedLevel = PlayerPrefs.GetInt(LastCompletedLevelPrefsKey, -1);
        continueButton.SetActive(_lastCompletedLevel >= 0 && _lastCompletedLevel < levels.Length - 1);
    }

    public void OnNewGameClicked()
    {
        //loadscreen.gameObject.SetActive(true);
        titleScreen.SetActive(false);
        if (_currentLevel != null)
        {
            StartCoroutine(UnloadAndStartNewGame());
        }
        else
        {
            StartCoroutine(LoadLevelAsync(0));
        }
    }

    private IEnumerator UnloadAndStartNewGame()
    {
        yield return UnloadLevel(_currentLevelIndex);
        yield return LoadLevelAsync(0);
    }

    public void OnContinueClicked()
    {
        //loadscreen.gameObject.SetActive(true);
        titleScreen.SetActive(false);

        if (_currentLevel != null)
        {
            SetGameUIActive(true);
        }
        else
        {
            StartCoroutine(LoadLevelAsync(_lastCompletedLevel + 1));
        }
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

        SetGameUIActive(true);
        //loadscreen.gameObject.SetActive(false);
    }

    private void OnLevelCompleted()
    {
        _currentLevel.onCompleted -= OnLevelCompleted;

        _lastCompletedLevel = _currentLevelIndex;
        PlayerPrefs.SetInt(LastCompletedLevelPrefsKey, _lastCompletedLevel);
        var highestCompletedLevel = PlayerPrefs.GetInt(HighestCompletedLevelPrefsKey, -1);
        if (_lastCompletedLevel > highestCompletedLevel)
            PlayerPrefs.SetInt(HighestCompletedLevelPrefsKey, _lastCompletedLevel);

        UpdateContinueButton();

        StartCoroutine(NextLevelCoroutine());
    }

    private IEnumerator NextLevelCoroutine()
    {
        //loadscreen.gameObject.SetActive(true);

        yield return UnloadLevel(_currentLevelIndex);

        if (++_currentLevelIndex <= levels.Length - 1)
        {
            yield return LoadLevelAsync(_currentLevelIndex);
        }
        else
        {
            Credits();
            //loadscreen.gameObject.SetActive(false);
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
        //loadscreen.gameObject.SetActive(true);

        yield return UnloadLevel(_currentLevelIndex);
        yield return LoadLevelAsync(_currentLevelIndex);
    }

    public void Credits()
    {
        titleScreen.SetActive(true);
        SetGameUIActive(false);
        creditsScreen.SetActive(true);
    }

    public void ToMenu()
    {
        titleScreen.SetActive(true);
        SetGameUIActive(false);
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

    private void SetGameUIActive(bool value)
    {
        foreach (var go in gameUI)
        {
            go.SetActive(value);
        }
    }
}
