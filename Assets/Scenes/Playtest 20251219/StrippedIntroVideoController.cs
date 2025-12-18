using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class StrippedIntroVideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer;


    private IEnumerator PlayVideoSequence()
    {
        // Wait 1 second before playing
        yield return new WaitForSeconds(2f);

        // Subscribe to the event that fires when the video finishes
        /*   videoPlayer.loopPointReached += OnVideoFinished;
   */
        // Start playback
        videoPlayer.Play();
    }

}
