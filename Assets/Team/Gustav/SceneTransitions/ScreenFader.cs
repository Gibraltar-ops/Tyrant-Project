using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    public float fadeDuration = 2f;
    private Image image;
    private Coroutine currentFadeRoutine;
    private void Awake()
    {
        GetImageComponent();
        if (image != null)
        {
            Color c = image.color;
            image.color = new Color(c.r, c.g, c.b, 1f);
            image.raycastTarget = true;
        }
    }

    private void GetImageComponent()
    {
        if (image == null)
            image = GetComponent<Image>();
    }

    public IEnumerator FadeOut()
    {
        GetImageComponent();
        image.raycastTarget = true;

        if (currentFadeRoutine != null) StopCoroutine(currentFadeRoutine);

        currentFadeRoutine = StartCoroutine(Fade(1f));
        yield return currentFadeRoutine;
    }

    public IEnumerator FadeIn()
    {
        GetImageComponent();

        if (currentFadeRoutine != null) StopCoroutine(currentFadeRoutine);

        currentFadeRoutine = StartCoroutine(Fade(0f));
        yield return currentFadeRoutine;

        image.raycastTarget = false;
    }

    private IEnumerator Fade(float targetAlpha)
    {
        Debug.Log($"ScreenFader: Startar fade till {targetAlpha}");
        GetImageComponent();

        float t = 0f;
        Color c = image.color;
        float startAlpha = c.a;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, t / fadeDuration);

            image.color = new Color(c.r, c.g, c.b, newAlpha);
            yield return null;
        }
        image.color = new Color(c.r, c.g, c.b, targetAlpha);
    }

}
