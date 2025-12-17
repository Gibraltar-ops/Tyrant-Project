using UnityEngine;
using System.Collections;

public class UIDrawer : MonoBehaviour
{
    [Header("Positions")]
    [SerializeField] private Vector2 openPosition = Vector2.zero;
    [SerializeField] private float closedOffsetX = 1200f;

    [Header("Animation")]
    [SerializeField] private float duration = 0.25f;
    [SerializeField] private AnimationCurve easing = AnimationCurve.EaseInOut(0, 0, 1, 1);

    public bool IsOpen => isOpen;

    private RectTransform rect;
    private Coroutine routine;
    private bool isOpen;

    private Vector2 closedPosition;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        closedPosition = openPosition + Vector2.right * closedOffsetX;

        // Start closed
        rect.anchoredPosition = closedPosition;
        isOpen = false;
    }

    public void Toggle()
    {
        if (isOpen)
            Close();
        else
            Open();
    }

    public void Open()
    {
        if (isOpen) return;
        isOpen = true;
        Animate(openPosition);
    }

    public void Close()
    {
        if (!isOpen) return;
        isOpen = false;
        Animate(closedPosition);
    }

    private void Animate(Vector2 target)
    {
        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(Slide(target));
    }

    private IEnumerator Slide(Vector2 target)
    {
        Vector2 start = rect.anchoredPosition;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = easing.Evaluate(time / duration);
            rect.anchoredPosition = Vector2.Lerp(start, target, t);
            yield return null;
        }

        rect.anchoredPosition = target;
    }
}
