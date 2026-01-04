using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class FullscreenEffectToggle : MonoBehaviour
{
    public ScriptableRendererFeature fullscreenFeature;
    public Material sourceMaterial;
    public float activeTime = 1.4f;

    FullScreenPassRendererFeature pass;

    void Awake()
    {
        pass = fullscreenFeature as FullScreenPassRendererFeature;

        // Force known safe state BEFORE first render
        fullscreenFeature.SetActive(false);

        // Initialize shader time offset immediately
        sourceMaterial.SetFloat("_EffectStartTime", Time.timeSinceLevelLoad);

        // Ensure the renderer feature references the source material
        pass.passMaterial = sourceMaterial;
    }

    public void PlayEffect()
    {
        StopAllCoroutines();
        StartCoroutine(EffectRoutine());
    }

    IEnumerator EffectRoutine()
    {
        // Reset start time EXACTLY when effect begins
        sourceMaterial.SetFloat("_EffectStartTime", Time.timeSinceLevelLoad);

        fullscreenFeature.SetActive(true);

        yield return new WaitForSeconds(activeTime);

        fullscreenFeature.SetActive(false);
    }

    void OnDisable()
    {
        fullscreenFeature.SetActive(false);
    }
}
