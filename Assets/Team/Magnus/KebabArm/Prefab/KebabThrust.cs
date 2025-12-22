using System.Collections.Generic;
using UnityEngine;

public class KebabThrust : MonoBehaviour
{
    [Header("reaches")]
    [Tooltip("Inner arms reach")]
    public float innerRange = 25.0f;

    [Tooltip("Outer arms reach")]
    public float outerRange = 25.0f;

    [Header("Arm references")]
    public Transform leftMostArm;
    public Transform leftCenterArm;
    public Transform rightCenterArm;
    public Transform rightMostArm;

    private List<Transform> _allArms;

    void Awake()
    {
        _allArms = new List<Transform> { leftMostArm, leftCenterArm, rightCenterArm, rightMostArm };
    }

    public void AttackAtPosition(Vector3 targetPosition, System.Action onHit = null)
    {
        Transform bestArm = null;
        float shortestDistance = Mathf.Infinity;

        foreach (Transform arm in _allArms)
        {
            if (arm == null)
                continue;

            // Muscle script check
            var muscle = arm.GetComponent<KebabMuscleMemory>();
            // arm skip if attackimg
            if (muscle == null || muscle.IsAttacking)
            {
                continue;
            }

            float currentMaxRange = GetRangeForArm(arm);
            float distanceToTarget = Vector2.Distance(targetPosition, arm.position);

            // In range and closer than current arm?
            if (distanceToTarget <= currentMaxRange && distanceToTarget < shortestDistance)
            {
                shortestDistance = distanceToTarget;
                bestArm = arm;
            }
        }

        // Found free arm
        if (bestArm != null)
        {
            var muscle = bestArm.GetComponent<KebabMuscleMemory>();
            muscle.Thrust(targetPosition, onHit);
        }
        else { }
    }

    // Range help funktion
    float GetRangeForArm(Transform arm)
    {
        // Outer arm check
        if (arm == leftMostArm || arm == rightMostArm)
        {
            return outerRange;
        }
        // Inner arm check
        return innerRange;
    }

    // Arm range gizmo
    void OnDrawGizmos()
    {
        if (leftMostArm == null)
            return;

        // Outer arms range (RED)
        Gizmos.color = new Color(1, 0, 0, 0.2f);
        if (leftMostArm)
            Gizmos.DrawWireSphere(leftMostArm.position, outerRange);
        if (rightMostArm)
            Gizmos.DrawWireSphere(rightMostArm.position, outerRange);

        // Inner arms range (Yellow)
        Gizmos.color = new Color(1, 0.92f, 0.016f, 0.2f);
        if (leftCenterArm)
            Gizmos.DrawWireSphere(leftCenterArm.position, innerRange);
        if (rightCenterArm)
            Gizmos.DrawWireSphere(rightCenterArm.position, innerRange);
    }
}
