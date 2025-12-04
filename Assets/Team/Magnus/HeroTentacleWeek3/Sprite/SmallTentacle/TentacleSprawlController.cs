using System.Collections.Generic;
using UnityEngine;

public class TentacleSprawlController : MonoBehaviour
{
    public enum SprawlState
    {
        Idle,
        Reaching,
        Attacking,
        Retracting,
    }

    [Header("Settings")]
    public GameObject legPrefab;
    public int numberOfLegs = 8;

    [Tooltip("Sprawl attack base speed")]
    public float baseAttackSpeed = 4f;

    [Header("Organic Motion Settings")]
    [Tooltip("least delay")]
    public float minStartDelay = 0.0f;

    [Tooltip("most delay")]
    public float maxStartDelay = 0.3f;

    [Tooltip("How much the speed is allowed to vary in the sprawl")]
    public float speedVariance = 0.3f;

    [Header("Failsafe")]
    [Tooltip("Attack anyways if tentacle hasnt reached target within time.")]
    public float maxReachDuration = 0.5f;

    [Header("Random Variance")]
    public float minTriggerDist = 1.5f;
    public float maxTriggerDist = 3.0f;
    public float minArch = 0.2f;
    public float maxArch = 0.8f;
    public float minGripRadius = 0.5f;
    public float maxGripRadius = 0.8f;

    [Header("Idle Pose Settings")]
    public float idleExtensionFraction = 0.25f;

    [Tooltip("Downward offset")]
    public float idleDownOffset = 0.35f;

    [Tooltip("Idle arch multiplier")]
    public float idleArchMultiplier = 0.5f;

    public bool idleHangAtTip = false;

    [Header("References")]
    public TentacleControllerIK mainController;

    // State
    [SerializeField]
    private SprawlState currentState = SprawlState.Idle;

    private List<SprawlLeg> legs = new List<SprawlLeg>();
    private IGrabbable itemToPull;
    private Vector3 currentTargetPos;

    // Cache
    private float currentTriggerDistSqr;
    private float currentArchAmount;
    private float reachTimer; // Failsafe for reach override
    private Vector3[] legArchDirections;
    private Vector3[] legTargetOffsets;

    void Start()
    {
        if (mainController == null)
            mainController = GetComponentInParent<TentacleControllerIK>();
        InitializeLegs();
    }

    void InitializeLegs()
    {
        foreach (var leg in legs)
            if (leg != null)
                Destroy(leg.gameObject);
        legs.Clear();

        legArchDirections = new Vector3[numberOfLegs];
        legTargetOffsets = new Vector3[numberOfLegs];

        for (int i = 0; i < numberOfLegs; i++)
        {
            GameObject obj = Instantiate(
                legPrefab,
                transform.position,
                Quaternion.identity,
                transform
            );
            SprawlLeg legScript = obj.GetComponent<SprawlLeg>();
            legs.Add(legScript);

            float angle = (360f / numberOfLegs) * i;
            Vector3 dir = new Vector3(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad),
                0
            );
            legArchDirections[i] = dir;
        }
    }

    void Update()
    {
        switch (currentState)
        {
            case SprawlState.Idle:
                if (idleHangAtTip && mainController != null)
                {
                    Vector3 tip = mainController.effectorBody.position;
                    Vector3 centerTarget = tip + (Vector3.down * idleDownOffset);
                    UpdateIdleLegsTo(centerTarget);
                }
                break;
            case SprawlState.Reaching:
                HandleReachingLogic();
                break;
            case SprawlState.Attacking:
                HandleAttackMotion();
                break;
            case SprawlState.Retracting:
                HandleRetractionMotion();
                break;
        }
    }

    public void SetNewTarget(Vector3 targetPosition, IGrabbable item)
    {
        currentTargetPos = targetPosition;
        itemToPull = item;

        // Reset timer
        reachTimer = 0f;

        // Randomize
        float dist = Random.Range(minTriggerDist, maxTriggerDist);
        currentTriggerDistSqr = dist * dist;
        currentArchAmount = Random.Range(minArch, maxArch);
        float grip = Random.Range(minGripRadius, maxGripRadius);

        RecalculateOffsets(grip);
        ResetLegsVisuals();

        currentState = SprawlState.Reaching;
    }

    void RecalculateOffsets(float radius)
    {
        for (int i = 0; i < numberOfLegs; i++)
            legTargetOffsets[i] = (Vector3)Random.insideUnitCircle * radius;
    }

    void HandleReachingLogic()
    {
        reachTimer += Time.deltaTime;

        float distSqr = (
            (Vector3)mainController.effectorBody.position - currentTargetPos
        ).sqrMagnitude;

        if (distSqr <= currentTriggerDistSqr || reachTimer > maxReachDuration)
        {
            StartAttack();
        }
    }

    void StartAttack()
    {
        currentState = SprawlState.Attacking;

        foreach (var leg in legs)
        {
            float randomDelay = Random.Range(minStartDelay, maxStartDelay);

            float speedMult = Random.Range(1.0f - speedVariance, 1.0f + speedVariance);
            float finalSpeed = baseAttackSpeed * speedMult;

            leg.InitializeAttack(randomDelay, finalSpeed);
        }
    }

    void HandleAttackMotion()
    {
        Vector3 targetBase = (itemToPull != null) ? itemToPull.Position : currentTargetPos;

        UpdateLegsTo(targetBase, false);

        if (AllLegsExtended())
        {
            if (mainController != null && itemToPull != null)
            {
                mainController.InitiateCapture(itemToPull);
            }
            currentState = SprawlState.Retracting;
        }
    }

    void UpdateIdleLegsTo(Vector3 centerTarget)
    {
        Vector3 startPos = mainController.effectorBody.position;

        for (int i = 0; i < legs.Count; i++)
        {
            Vector3 finalTarget = centerTarget + legTargetOffsets[i];
            Vector3 arch = legArchDirections[i] * (currentArchAmount * idleArchMultiplier);
            legs[i].SetIdlePose(startPos, finalTarget, arch, idleExtensionFraction);
        }
    }

    void HandleRetractionMotion()
    {
        Vector3 targetBase = (itemToPull != null) ? itemToPull.Position : currentTargetPos;

        if (mainController.ActiveCaptureVisual != null)
            targetBase = mainController.ActiveCaptureVisual.transform.position;

        UpdateLegsTo(targetBase, true);

        if (AllLegsRetracted())
        {
            StopAttack();
        }
    }

    void UpdateLegsTo(Vector3 centerTarget, bool isRetracting)
    {
        Vector3 startPos = mainController.effectorBody.position;

        for (int i = 0; i < legs.Count; i++)
        {
            Vector3 finalTarget = centerTarget + legTargetOffsets[i];
            Vector3 arch = legArchDirections[i] * currentArchAmount;

            legs[i].UpdateLeg(startPos, finalTarget, arch, isRetracting);
        }
    }

    public void StopAttack()
    {
        currentState = SprawlState.Idle;
        itemToPull = null;

        if (idleHangAtTip) { }
        else
        {
            foreach (var leg in legs)
                leg.ResetLeg();
        }
    }

    private void ResetLegsVisuals()
    {
        foreach (var leg in legs)
            leg.ResetLeg();
    }

    private bool AllLegsExtended()
    {
        foreach (var leg in legs)
            if (!leg.IsFullyExtended())
                return false;
        return true;
    }

    private bool AllLegsRetracted()
    {
        foreach (var leg in legs)
            if (!leg.IsRetracted())
                return false;
        return true;
    }
}
