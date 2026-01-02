using System.Collections.Generic;
using UnityEngine;

public class KebabThrust : MonoBehaviour
{
    [Header("reaches")]
    [Tooltip("Inner arms reach")]
    public float innerRange = 25.0f;

    [Tooltip("Outer arms reach")]
    public float outerRange = 25.0f;

    [Header("Upgrade settings")]
    public CurrencySourceSO kebabStats;

    public bool debugMultiStrike = false;
    public int CurrentStrikeCount
    {
        get
        {
            if (debugMultiStrike)
            {
                return 4;
            }

            if (kebabStats != null)
            {
                return (int)Lood.UpgradeManager.Instance.CurrencySources[kebabStats.Tag].MainStats[MainStat.BaseSpeed];
            }

            return 1;
        }
    }


    [Header("Arm references")]
    public Transform leftMostArm;
    public Transform leftCenterArm;
    public Transform rightCenterArm;
    public Transform rightMostArm;

    private List<Transform> _allArms;
    private int _nextArmIndex = 0;

    void Awake()
    {
        Lood.UpgradeManager.Instance.AddUpgradeableObject(kebabStats);
        _allArms = new List<Transform> { leftMostArm, leftCenterArm, rightCenterArm, rightMostArm };
    }

    public void AttackAtPosition(Vector3 targetPosition, System.Action onHit = null)
    {
        AttackMultipleTargets(new List<Vector3> { targetPosition }, (i) => onHit?.Invoke());
    }

    public void AttackMultipleTargets(List<Vector3> targetPositions, System.Action<int> onHit = null)
    {
        if (targetPositions == null || targetPositions.Count == 0) return;

        Vector3 primaryTarget = targetPositions[0];

        for (int i = 0; i < _allArms.Count; i++)
        {
            int armIndex = (_nextArmIndex + i) % _allArms.Count;
            Transform currentArm = _allArms[armIndex];

            if (currentArm == null)
                continue;

            var muscle = currentArm.GetComponent<KebabMuscleMemory>();

            if (muscle == null || muscle.IsAttacking)
            {
                continue;
            }

            float currentMaxRange = GetRangeForArm(currentArm);
            float distanceToTarget = Vector2.Distance(primaryTarget, currentArm.position);

            if (distanceToTarget <= currentMaxRange)
            {
                _nextArmIndex = (armIndex + 1) % _allArms.Count;
                muscle.Thrust(targetPositions, onHit);
                return;
            }
        }
    }
    // Range help funktion
    float GetRangeForArm(Transform arm)
    {
        // Outer arm
        if (arm == leftMostArm || arm == rightMostArm)
        {
            return outerRange;
        }
        // Inner arm 
        return innerRange;
    }

    // Arm range gizmo
    void OnDrawGizmos()
    {
        if (leftMostArm == null)
            return;

        // Outer arms range = (RED)
        Gizmos.color = new Color(1, 0, 0, 0.2f);
        if (leftMostArm)
            Gizmos.DrawWireSphere(leftMostArm.position, outerRange);
        if (rightMostArm)
            Gizmos.DrawWireSphere(rightMostArm.position, outerRange);

        // Inner arms range =  (Yellow)
        Gizmos.color = new Color(1, 0.92f, 0.016f, 0.2f);
        if (leftCenterArm)
            Gizmos.DrawWireSphere(leftCenterArm.position, innerRange);
        if (rightCenterArm)
            Gizmos.DrawWireSphere(rightCenterArm.position, innerRange);
    }
}
