using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class KebabArm : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Attach all joints in order here: Bone1 -> Bone5")]
    public List<Transform> joints;

    // Target, later to be set to find Gustavs resistance vectors

    [Header("Target")]
    [Tooltip("Where we want the kebab to reach, i.e where we click and our systems finds a target")]
    public Transform target;

    [Header("Iterations")]
    public int iterations = 5;

    [Header("Reach tolerance")]
    [Tooltip("Tolerance of how far when the tentacle considers itself reached")]
    public float tolerance = 0.05f;

    [Header("Constraints")]
    [Tooltip("Forces joints to bend a certain way")]
    public bool enableClockwiseConstraint = true;

    [Header("Sprite adjustment")]
    [Range(-180f, 180f)]
    public float tipRotationOffset = 0f;

    [Range(0f, 180f)]
    [Tooltip("Hur much a bone is allowed to bend compared to previous")]
    public float maxBendAngle = 90f;

    private float[] boneLengths;
    private float totalLength;
    private Vector3[] positions;

    private void Start()
    {
        if (joints == null || joints.Count < 5)
        {
            Debug.LogError("Assign enough joints");
            return;
        }

        int count = joints.Count;
        boneLengths = new float[count - 1];
        positions = new Vector3[count];

        //Calculate bone length (Constants)

        totalLength = 0;
        for (int i = 0; i < count - 1; i++)
        {
            boneLengths[i] = Vector3.Distance(joints[i].position, joints[i + 1].position);
            totalLength += boneLengths[i];
        }
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        ResolveIK();
    }

    private void ResolveIK()
    {
        for (int i = 0; i < joints.Count; i++)
            positions[i] = joints[i].position;

        Vector3 rootPos = joints[0].position;
        Vector3 targetPos = target.position;

        float distToTarget = Vector3.Distance(rootPos, targetPos);
        if (distToTarget > totalLength)
        {
            Vector3 direction = (targetPos - rootPos).normalized;
            for (int i = 0; i < positions.Length - 1; i++)
                positions[i + 1] = positions[i] + direction * boneLengths[i];
        }
        else
        {
            for (int iter = 0; iter < iterations; iter++)
            {
                if (Vector3.Distance(positions[positions.Length - 1], targetPos) < tolerance)
                    break;

                // Forward
                positions[positions.Length - 1] = targetPos;
                for (int i = positions.Length - 2; i >= 0; i--)
                {
                    Vector3 direction = (positions[i] - positions[i + 1]).normalized;
                    positions[i] = positions[i + 1] + direction * boneLengths[i];
                }

                // Backward (with Constraints)
                positions[0] = rootPos;
                for (int i = 0; i < positions.Length - 1; i++)
                {
                    Vector3 direction = (positions[i + 1] - positions[i]).normalized;
                    if (i > 0)
                    {
                        Vector3 prevBoneDir = (positions[i] - positions[i - 1]).normalized;
                        direction = ApplyHingeConstraint(prevBoneDir, direction);
                    }
                    positions[i + 1] = positions[i] + direction * boneLengths[i];
                }
            }
        }

        for (int i = 0; i < joints.Count; i++)
        {
            joints[i].position = positions[i];

            if (i < joints.Count - 1)
            {
                Vector3 direction = positions[i + 1] - positions[i];
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                joints[i].rotation = Quaternion.Euler(0, 0, angle);
            }
            else
            {
                // calculate angel from previous leg
                Vector3 direction = positions[i] - positions[i - 1];
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                // Add manual offset
                joints[i].rotation = Quaternion.Euler(0, 0, angle + tipRotationOffset);
            }
        }
    }

    private Vector3 ApplyHingeConstraint(Vector3 prevDir, Vector3 wantedDir)
    {
        //Calculate new angle from previous bone and new iteration
        float angle = Vector3.SignedAngle(prevDir, wantedDir, Vector3.forward);

        //MIN MAX angle
        float min = enableClockwiseConstraint ? -maxBendAngle : 0f;
        float max = enableClockwiseConstraint ? 0f : maxBendAngle;

        float clampedAngle = Mathf.Clamp(angle, -maxBendAngle, maxBendAngle);

        if (enableClockwiseConstraint)
        {
            if (angle > 0)
                clampedAngle = 0;
            if (angle < -maxBendAngle)
                clampedAngle = -maxBendAngle;
        }
        else
        {
            if (angle < 0)
                clampedAngle = 0;
            if (angle > maxBendAngle)
                clampedAngle = maxBendAngle;
        }

        Quaternion rot = Quaternion.AngleAxis(clampedAngle, Vector3.forward);
        return rot * prevDir;
    }

    private void OnDrawGizmos()
    {
        if (joints == null || joints.Count == 0)
            return;
        Gizmos.color = Color.cyan;
        for (int i = 0; i < joints.Count - 1; i++)
        {
            if (joints[i] != null && joints[i + 1] != null)
                Gizmos.DrawLine(joints[i].position, joints[i + 1].position);
        }
    }
}
