using UnityEngine;
using UnityEngine.U2D;

public class TentacleControllerIK : MonoBehaviour
{
    public TargetJoint2D effectorJoint;
    public Rigidbody2D effectorBody;

    public Rigidbody2D rootBody;

    private Vector3 targetPosition;

    [Header("Visual Tip")]
    public Transform tentacleTip;

    [Header("Stretching Parameters")]
    public float totalUnstretchedLength = 9.5f;
    public float maxStretchLimit = 3.0f;
    public Transform rootBone;
    public Transform[] bonesToStretch;

    [Header("IK Dead Zone")]
    public float targetDeadZone = 0.5f;

    [Header("Target Joint Dynamics")]
    public float maxJointFrequency = 80f;
    public float minJointFrequency = 0.5f;
    public float relaxationDistance = 0.5f;

    void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = 0;
        }
        if (effectorJoint != null)
        {
            float distanceToNewTarget = Vector3.Distance(effectorJoint.target, targetPosition);

            if (distanceToNewTarget > targetDeadZone)
            {
                effectorJoint.target = targetPosition;
                effectorJoint.frequency = maxJointFrequency;
            }
        }

        AdjustTargetJointTension();

        transform.position = targetPosition;

        if (tentacleTip != null && effectorBody != null)
        {
            Vector3 directionToTarget = targetPosition - (Vector3)effectorBody.position;

            float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;

            tentacleTip.rotation = Quaternion.Euler(0f, 0f, angle - -90f);
        }

        /*         HandleStretching();
         */
    }

    private void AdjustTargetJointTension()
    {
        if (effectorJoint == null || effectorBody == null)
            return;

        // Distance between effectorbody and target
        float distanceToTarget = Vector3.Distance(effectorBody.position, effectorJoint.target);

        // Normalisation: 1.0 = Far away, 0.0 = Within reach
        float tensionFactor = Mathf.Clamp01(distanceToTarget / relaxationDistance);

        float newFrequency = Mathf.Lerp(minJointFrequency, maxJointFrequency, tensionFactor);

        effectorJoint.frequency = newFrequency;
    }

    void HandleStretching()
    {
        if (rootBone == null || bonesToStretch == null || totalUnstretchedLength <= 0.01f)
            return;

        float distanceToTarget = Vector3.Distance(rootBone.position, targetPosition);

        float stretchFactor = 1.0f;

        if (distanceToTarget > totalUnstretchedLength)
        {
            stretchFactor = distanceToTarget / totalUnstretchedLength;

            stretchFactor = Mathf.Min(stretchFactor, maxStretchLimit);
        }
        else
        {
            stretchFactor = distanceToTarget / totalUnstretchedLength;

            stretchFactor = Mathf.Max(stretchFactor, 0.8f);
        }

        foreach (Transform bone in bonesToStretch)
        {
            bone.localScale = new Vector3(bone.localScale.x, stretchFactor, bone.localScale.z);
        }
    }
}
