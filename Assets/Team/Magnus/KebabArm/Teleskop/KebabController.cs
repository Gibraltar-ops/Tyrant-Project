using UnityEngine;

public class ClawController : MonoBehaviour
{
    [Header("Setup")]
    public Transform ikTarget;
    public Transform baseMount;

    [Header("Settings")]
    public float maxIKLength = 5f;
    public float maxBaseExtension = 10f;

    private Vector3 _startLocalPos;

    void Start()
    {
        if (baseMount != null)
            _startLocalPos = baseMount.localPosition;
    }

    void Update()
    {
        if (ikTarget == null || baseMount == null)
            return;

        Transform parent = baseMount.parent != null ? baseMount.parent : transform;
        Vector3 localTargetPos = parent.InverseTransformPoint(ikTarget.position);

        float distanceToTarget = localTargetPos.magnitude;

        if (distanceToTarget > maxIKLength)
        {
            float missingDistance = distanceToTarget - maxIKLength;

            float extensionAmount = Mathf.Clamp(missingDistance, 0, maxBaseExtension);

            Vector3 direction = localTargetPos.normalized;

            baseMount.localPosition = _startLocalPos + (direction * extensionAmount);
        }
        else
        {
            baseMount.localPosition = _startLocalPos;
        }
    }

    void OnDrawGizmos()
    {
        if (baseMount != null && baseMount.parent != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(baseMount.parent.position, maxIKLength);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(baseMount.parent.position, maxIKLength + maxBaseExtension);
        }
    }
}
