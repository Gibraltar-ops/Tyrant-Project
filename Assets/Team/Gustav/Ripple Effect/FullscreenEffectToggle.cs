using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class FullscreenEffectToggle : MonoBehaviour
{
    [Header("Renderer Feature")]
    public ScriptableRendererFeature fullscreenFeature;

    [Header("Fullscreen Render Material")]
    public Material sourceMaterial;

    [Header("Duration")]
    public float activeTime = 1f;

    Coroutine currentRoutine;
    Material runtimeMaterial;

    void Awake()
    {
        if (fullscreenFeature != null)
            fullscreenFeature.SetActive(false);
    }

    public void PlayEffect()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(EffectRoutine());
    }

    IEnumerator EffectRoutine()
    {
        // Destroy previous runtime material (safety)
        if (runtimeMaterial != null)
            Destroy(runtimeMaterial);

        // Create a fresh instance / resets all shader time/state
        runtimeMaterial = new Material(sourceMaterial);

        // Assign it to the Full Screen Pass Renderer Feature
        AssignMaterialToFeature(runtimeMaterial);

        fullscreenFeature.SetActive(true);

        yield return new WaitForSeconds(activeTime);

        fullscreenFeature.SetActive(false);
    }

    void AssignMaterialToFeature(Material mat)
    {
        var fullScreenPass = fullscreenFeature as FullScreenPassRendererFeature;

        if (fullScreenPass != null)
        {
            fullScreenPass.passMaterial = mat;
        }
        else
        {
            Debug.LogWarning("Renderer Feature is not a FullScreenPassRendererFeature.");
        }
    }

    void OnDestroy()
    {
        if (runtimeMaterial != null)
            Destroy(runtimeMaterial);
    }
}
