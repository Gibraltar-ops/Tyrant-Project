using System.Collections.Generic;
using UnityEngine;

public class TentacleSprawlController : MonoBehaviour
{
    [Header("Settings")]
    public GameObject legPrefab;
    public int numberOfLegs = 8;

    public float maxReach = 40f;

    public float attackSpeed = 5f;

    [Header("Random Variance (Min/Max)")]
    public float minTriggerDist = 35f;
    public float maxTriggerDist = 40f;

    public float minArch = 0.2f;
    public float maxArch = 0.5f;

    [Tooltip("Slumpa hur mycket de sprider sig vid målet.")]
    public float minGripRadius = 0.5f;
    public float maxGripRadius = 1.2f;

    [Header("References")]
    public TentacleControllerIK mainController;

    private List<SprawlLeg> legs = new List<SprawlLeg>();

    private bool isAttacking = false;
    private bool hasActiveTarget = false;
    private Vector3 currentTarget;

    //Current grab values
    private float currentTriggerDistance;
    private float currentArchAmount;

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

            // base direction set
            float angle = (360f / numberOfLegs) * i;
            Vector3 direction = new Vector3(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad),
                0
            );
            legArchDirections[i] = direction;
        }
    }

    void Update()
    {
        // logik
        if (hasActiveTarget && !isAttacking)
        {
            CheckDistanceAndTrigger();
        }

        if (isAttacking)
        {
            UpdateLegsPositions();
        }
    }

    public void SetNewTarget(Vector3 targetPosition)
    {
        currentTarget = targetPosition;
        hasActiveTarget = true;

        // randomize distance
        currentTriggerDistance = Random.Range(minTriggerDist, maxTriggerDist);

        // randomize arch
        currentArchAmount = Random.Range(minArch, maxArch);

        // randomize grip radius
        float currentGrip = Random.Range(minGripRadius, maxGripRadius);
        RecalculateLegOffsets(currentGrip);

        // reset legs
        StopAttack();
    }

    void RecalculateLegOffsets(float radius)
    {
        for (int i = 0; i < numberOfLegs; i++)
        {
            //Grip variation randomizer
            legTargetOffsets[i] = (Vector3)Random.insideUnitCircle * radius;
        }
    }

    void CheckDistanceAndTrigger()
    {
        float distanceToTarget = Vector3.Distance(
            mainController.effectorBody.position,
            currentTarget
        );

        // use random distance
        if (distanceToTarget <= currentTriggerDistance)
        {
            StartAttack();
        }
    }

    void StartAttack()
    {
        isAttacking = true;
        foreach (var leg in legs)
        {
            leg.SetVisibility(true);
        }
    }

    public void StopAttack()
    {
        isAttacking = false;
        hasActiveTarget = false; // Reset target
        foreach (var leg in legs)
        {
            leg.SetVisibility(false);
        }
    }

    void UpdateLegsPositions()
    {
        Vector3 startPos = mainController.effectorBody.position;

        for (int i = 0; i < legs.Count; i++)
        {
            Vector3 finalTarget = currentTarget + legTargetOffsets[i];

            Vector3 archDir = legArchDirections[i];
            float distance = Vector3.Distance(startPos, finalTarget);

            // random arch value usage
            Vector3 finalArchOffset = archDir * (currentArchAmount * (distance / 5f));

            legs[i].UpdateLeg(startPos, finalTarget, finalArchOffset, attackSpeed);
        }
    }
}
