using UnityEngine;
using UnityEngine.U2D;

public class RopeTentacleController : MonoBehaviour
{
    public TargetJoint2D effectorJoint;

    public Rigidbody2D rootBody;

    private Vector3 targetPosition;

    /*  [Header("Stretching Parameters")]
     public float totalUnstretchedLength = 9.5f;
     public float maxStretchLimit = 3.0f;
     public Transform rootBone;
     public Transform[] bonesToStretch;
  */
    void LateUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = 0;

            if (effectorJoint != null)
            {
                effectorJoint.target = targetPosition;
            }
        }

        transform.position = targetPosition;

        /* HandleStretching(); */
    }

    /* void HandleStretching()
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
    } */
}
