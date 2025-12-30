using UnityEngine;
using UnityEngine.UI;

public class ScrollRectZoom : MonoBehaviour
{
    public float zoomSpeed = 0.1f;
    public float minZoom = 0.5f;
    public float maxZoom = 2.5f;

    private ScrollRect scrollRect;
    private RectTransform content;
    private RectTransform viewport;

    void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
        content = scrollRect.content;
        viewport = scrollRect.viewport;
    }

    void Update()
    {
        // Only zoom if mouse is over the viewport
        if (!RectTransformUtility.RectangleContainsScreenPoint(
                viewport,
                Input.mousePosition,
                null))
            return;

        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) < 0.01f)
            return;

        float oldScale = content.localScale.x;
        float newScale = Mathf.Clamp(
            oldScale + scroll * zoomSpeed,
            minZoom,
            maxZoom
        );

        if (Mathf.Approximately(oldScale, newScale))
            return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            content,
            Input.mousePosition,
            null,
            out Vector2 localPoint
        );

        float scaleFactor = newScale / oldScale;

        content.localScale = Vector3.one * newScale;
        content.anchoredPosition -= localPoint * (scaleFactor - 1f);
    }
}
