using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class FullscreenEffectToggle : MonoBehaviour
{
    [Header("Assign your Full Screen Pass Renderer Feature")]
    public ScriptableRendererFeature fullscreenFeature;

    [Header("Duration in seconds")]
    public float activeTime = 1f;

    Coroutine currentRoutine;

    void Awake()
    {
        if (fullscreenFeature != null)
            fullscreenFeature.SetActive(false);
    }

    // Public method you can call from anywhere
    public void PlayEffect()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(EffectRoutine());
    }

    IEnumerator EffectRoutine()
    {
        fullscreenFeature.SetActive(true);
        yield return new WaitForSeconds(activeTime);
        fullscreenFeature.SetActive(false);
    }
}
