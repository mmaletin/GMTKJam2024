using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public CanvasGroup loadscreen;
    public GameObject titleScreen;
    public CameraFollow cameraFollow;

    [Scene] public int[] levels;

    public void OnNewGameClicked()
    {
        loadscreen.gameObject.SetActive(true);
        titleScreen.SetActive(false);

        StartCoroutine(LoadLevelAsync(levels[0]));
    }

    private IEnumerator LoadLevelAsync(int buildIndex)
    {
        var asyncOperation = SceneManager.LoadSceneAsync(buildIndex, LoadSceneMode.Additive);
        while (!asyncOperation.isDone)
        {
            yield return null;
        }

        // TODO Consider creating level class with all references and oncompleted event

        var levelScene = SceneManager.GetSceneByBuildIndex(buildIndex);
        var catController = GetComponentOnScene<CatController>(levelScene);
        cameraFollow.target = catController.transform;

        loadscreen.gameObject.SetActive(false);
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
}
