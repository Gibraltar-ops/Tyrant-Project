using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance;
    public ScreenFader screenFader;

    [Header("Fade Settings")]
    public ScreenFader fader; // assigned in inspector

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        screenFader.FadeOut();
    }

    // Public call for transitions
    public void TransitionTo(string sceneName)
    {
        StartCoroutine(PerformTransition(sceneName));
    }

    private IEnumerator PerformTransition(string sceneName)
    {
        if (fader != null)
            yield return fader.FadeOut();

        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        loadOp.allowSceneActivation = false;

        while (loadOp.progress < 0.9f)
            yield return null;

        loadOp.allowSceneActivation = true;

        if (fader != null)
            yield return fader.FadeIn();
    }
}
