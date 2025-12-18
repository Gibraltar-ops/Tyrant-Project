using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance;

    [Header("Fade Settings")]
    public ScreenFader fader;

    [Header("Game Over Settings")]
    public Image flashImage;
    public float flashSpeed = 3f;

    private bool isTransitioning = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        ProgressEventTrigger.onProgressEvent += HandleProgressEvent;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        ProgressEventTrigger.onProgressEvent -= HandleProgressEvent;
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isTransitioning = false;
        if (fader != null)
        {
            StartCoroutine(fader.FadeIn());
        }
    }

    public void TransitionTo(string sceneName)
    {
        if (isTransitioning) return;
        StartCoroutine(PerformTransition(sceneName));
    }

    private IEnumerator PerformTransition(string sceneName)
    {
        isTransitioning = true;
        if (fader != null)
            yield return fader.FadeOut();

        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        if (loadOp == null)
        {
            Debug.LogError($"[TransitionManager] Kunde inte ladda scenen '{sceneName}'. Kontrollera stavning och Build Settings.");
            isTransitioning = false;
            if (fader != null) StartCoroutine(fader.FadeIn()); // Fadea tillbaka så vi inte fastnar i svart
            yield break;
        }
        loadOp.allowSceneActivation = false;

        while (loadOp.progress < 0.9f)
        {
            yield return null;
        }

        loadOp.allowSceneActivation = true;

        while (!loadOp.isDone)
        {
            yield return null;
        }
    }

    public void GameOverTransition(string sceneName)
    {
        StartCoroutine(PerformGameOverSequence(sceneName));
    }

    private IEnumerator PerformGameOverSequence(string sceneName)
    {
        if (flashImage != null)
        {
            flashImage.raycastTarget = true;

            float t = 0;
            while (t < 1)
            {
                t += Time.unscaledDeltaTime * flashSpeed;
                flashImage.color = new Color(1, 0, 0, Mathf.Lerp(0, 1, t));
                yield return null;


            }
        }

        yield return new WaitForSecondsRealtime(0.5f);

        if (fader != null)
        {
            yield return fader.FadeOut();

        }


        if (flashImage != null)
        {
            flashImage.color = new Color(1, 0, 0, 0);
            flashImage.raycastTarget = false;

        }
        SceneManager.LoadScene(sceneName);
    }
    private void HandleProgressEvent(ProgressEvent progressEvent)
    {
        if (progressEvent == ProgressEvent.WIN)
        {
            TransitionTo("OutroCinematic_Final");
        }
    }
}