using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    public float fadeDuration = 0.5f;
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
        image.raycastTarget = false;
        FadeIn();
        //FadeOut();
    }

    public IEnumerator FadeOut()
    {
        yield return Fade(0f, 1f);
    }

    public IEnumerator FadeIn()
    {
        yield return Fade(1f, 0f);
    }

    private IEnumerator Fade(float start, float end)
    {
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
