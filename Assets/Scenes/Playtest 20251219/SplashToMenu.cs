using UnityEngine;
using UnityEngine.Video;

public class SplashToMenu : MonoBehaviour
{
    void Start()
    {
        GetComponent<VideoPlayer>().loopPointReached += (vp) =>
        {
            TransitionManager.Instance.TransitionTo("MainMenuPlaytest");
        };
    }
}