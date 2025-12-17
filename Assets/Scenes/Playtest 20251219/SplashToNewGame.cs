using UnityEngine;
using UnityEngine.Video;

public class SplashToNewGame : MonoBehaviour
{
    void Start()
    {
        GetComponent<VideoPlayer>().loopPointReached += (vp) =>
        {
            TransitionManager.Instance.TransitionTo("GameSceneFinal");
        };
    }
}