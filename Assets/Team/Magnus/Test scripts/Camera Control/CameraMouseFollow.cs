using UnityEngine;

public class CameraMouseFollow : MonoBehaviour
{
    public Transform anchor;
    [Range(0f, 1f)] public float followStrength = 0.3f;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (anchor == null) return;

        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = anchor.position.z;

        Vector3 midpoint = Vector3.Lerp(anchor.position, mouseWorld, followStrength);

        transform.position = midpoint;
    }
}
