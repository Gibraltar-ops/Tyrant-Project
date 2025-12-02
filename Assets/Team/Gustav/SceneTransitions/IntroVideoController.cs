using UnityEngine;
using UnityEngine.Video;
using System.Collections;

public class IntroVideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    private void Start()
    {
        // Start the sequence when the scene loads
        StartCoroutine(PlayVideoSequence());
    }

    private IEnumerator PlayVideoSequence()
    {
        // Wait 1 second before playing
        yield return new WaitForSeconds(1f);

        // Subscribe to the event that fires when the video finishes
        videoPlayer.loopPointReached += OnVideoFinished;

        // Start playback
        videoPlayer.Play();
    }
    private void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log("Video finished!");
        // Do anything here: load next scene, fade out, etc.
    }
}
