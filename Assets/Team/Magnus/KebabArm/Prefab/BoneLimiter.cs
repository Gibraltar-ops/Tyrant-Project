using UnityEngine;

[DefaultExecutionOrder(100)]
public class BoneLimiter : MonoBehaviour
{
    [Header("Angle Limits (Local Rotation)")]
    [Range(-180, 180)]
    public float minAngle = -45f;

    [Range(-180, 180)]
    public float maxAngle = 45f;

    void LateUpdate()
    {
        Vector3 currentRot = transform.localEulerAngles;

        float angle = currentRot.z;
        if (angle > 180)
            angle -= 360;

        float clampedAngle = Mathf.Clamp(angle, minAngle, maxAngle);

        if (Mathf.Abs(angle - clampedAngle) > 0.01f)
        {
            transform.localEulerAngles = new Vector3(currentRot.x, currentRot.y, clampedAngle);
        }
    }
}
