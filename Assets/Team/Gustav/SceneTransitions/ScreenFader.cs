using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    public float fadeDuration = 0.5f;
    private Image image;
    private Coroutine currentFadeRoutine;
    private void Awake()
    {
        image = GetComponent<Image>();
        if (image != null)
        {
            image.raycastTarget = false;
            Color c = image.color;
            image.color = new Color(c.r, c.g, c.b, 0f);
        }


    }

    private void GetImageComponent()
    {
        if (image == null)
            image.GetComponent<Image>();
    }

    public IEnumerator FadeOut()
    {
        GetImageComponent();
        image.raycastTarget = true;

        if (currentFadeRoutine != null) StopCoroutine(currentFadeRoutine);

        currentFadeRoutine = StartCoroutine(Fade(0f, 1f));
        yield return currentFadeRoutine;
    }

    public IEnumerator FadeIn()
    {
        GetImageComponent();

        if (currentFadeRoutine != null) StopCoroutine(currentFadeRoutine);

        currentFadeRoutine = StartCoroutine(Fade(1f, 0f));
        yield return currentFadeRoutine;

        image.raycastTarget = false;
    }

    private IEnumerator Fade(float start, float end)
    {

        GetImageComponent();

        float t = 0f;
        Color c = image.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(start, end, t / fadeDuration);
            image.color = new Color(c.r, c.g, c.b, a);
            yield return null;
        }

        image.color = new Color(c.r, c.g, c.b, end);
    }
}
