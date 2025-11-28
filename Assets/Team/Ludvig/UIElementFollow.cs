using UnityEngine;

public class UIElementFollow : MonoBehaviour
{
    [Header("Target to follow")]
    public Transform target; // The world object to follow

    [Header("UI Settings")]
    public Vector3 offset = new Vector3(0, 50, 0); // Pixel offset in UI space
    public Camera mainCamera; // Camera used for world-to-screen conversion

    private RectTransform uiElement;

    void Awake()
    {
        uiElement = GetComponent<RectTransform>();

        // Auto-assign main camera if not set
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (target == null || mainCamera == null)
            return;

        // Convert world position to screen position
        Vector3 screenPos = mainCamera.WorldToScreenPoint(target.position);

        // If target is behind the camera, hide UI element
        if (screenPos.z < 0)
        {
            uiElement.gameObject.SetActive(false);
            return;
        }
        else
        {
            uiElement.gameObject.SetActive(true);
        }

        // Apply offset and set UI position
        uiElement.position = screenPos + offset;
    }
}