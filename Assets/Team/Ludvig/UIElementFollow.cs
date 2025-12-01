using UnityEngine;

public class UIElementFollow : MonoBehaviour
{
    [Header("Target to follow")]
    public Transform target; // The world object to follow

    [Header("UI Settings")]
    public Vector3 offset = new Vector3(0, 50, 0); // Pixel offset in UI space
    public float limit = 200f; // Max distance from screen center
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

        // Apply offset
        Vector3 desiredPos = screenPos + offset;

        // Screen center
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, desiredPos.z);

        // Clamp distance from center
        Vector3 direction = desiredPos - screenCenter;
        if (direction.magnitude > limit)
        {
            direction = direction.normalized * limit;
        }

        uiElement.position = screenCenter + direction;
    }
}