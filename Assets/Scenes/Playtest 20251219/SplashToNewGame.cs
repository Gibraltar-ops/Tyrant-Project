using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class SplashToNewGame : MonoBehaviour
{
    void Start()
    {
        GetComponent<VideoPlayer>().loopPointReached += (vp) =>
        {
            if (TransitionManager.Instance != null)
            {
                TransitionManager.Instance.TransitionTo("GameSceneFinal");

            }
            SceneManager.LoadScene("GameSceneFinal");
        };
    }
}