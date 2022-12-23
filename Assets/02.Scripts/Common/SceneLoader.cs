using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public CanvasGroup fadeCG;
    [Range(0.5f, 2f)]
    public float fadeDuration = 1f;

    public Dictionary<string, LoadSceneMode> loadScenes
        = new Dictionary<string, LoadSceneMode>();

    void InitSceneInfo()
    {
        loadScenes.Add("Level_1", LoadSceneMode.Additive);
        loadScenes.Add("PlayScene", LoadSceneMode.Additive);
    }

    private IEnumerator Start()
    {
        InitSceneInfo();

        fadeCG.alpha = 1f;

        foreach(var _loadScenes in loadScenes)
        {
            yield return StartCoroutine(LoadScene(_loadScenes.Key, _loadScenes.Value));
        }
        StartCoroutine(Fade(0f));
    }

    IEnumerator LoadScene(string sceneName, LoadSceneMode mode)
    {
        yield return SceneManager.LoadSceneAsync(sceneName, mode);

        Scene loadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
        SceneManager.SetActiveScene(loadedScene);
    }

    IEnumerator Fade(float finalAlpha)
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Level_1"));
        fadeCG.blocksRaycasts = true;
        float fadeSpeed = Mathf.Abs(fadeCG.alpha = finalAlpha) / fadeDuration;

        while(!Mathf.Approximately(fadeCG.alpha, finalAlpha))
        {
            fadeCG.alpha = Mathf.MoveTowards(fadeCG.alpha, finalAlpha, fadeSpeed * Time.deltaTime);
            yield return null;
        }

        fadeCG.blocksRaycasts = false;

        SceneManager.UnloadSceneAsync("SceneLoader");
    }
}
