using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance;

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
        {
            Destroy(gameObject);
            return;
        }

        if (fader != null) StartCoroutine(fader.FadeIn());
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


        // KOlla på att sy ihop med det här - hur ser vi vilken scen vi skall ladda härnäst? (Använda Async för att ladda från intro till main menu, använd async för att ladda från MM till stages)
        //  Se om vi kan sy ihop med Introvideocontroller (onvideofinished) 
        //Behöver även en fade out från stage complete till blackout 
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        loadOp.allowSceneActivation = false;

        while (loadOp.progress < 0.9f)
            yield return null;

        loadOp.allowSceneActivation = true;

        if (fader != null)
            yield return fader.FadeIn();
    }
}
