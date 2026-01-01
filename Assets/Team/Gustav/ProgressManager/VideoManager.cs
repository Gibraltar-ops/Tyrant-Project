using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;

public class VideoManager : MonoBehaviour
{
    public static VideoManager Instance;

    public VideoPlayer videoPlayer;
    public RawImage rawImage;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        rawImage.enabled = false;
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    public Coroutine PlayVideo(VideoClip clip)
    {
        return StartCoroutine(PlayVideoRoutine(clip));
    }

    private IEnumerator PlayVideoRoutine(VideoClip clip)
    {
        rawImage.enabled = true;
        videoPlayer.clip = clip;
        videoPlayer.Play();

        yield return new WaitUntil(() => !videoPlayer.isPlaying && videoPlayer.frame > 0);

        rawImage.enabled = false;
    }

    private void OnVideoFinished(VideoPlayer vp)
    {

    }

    private void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
        }
    }
}