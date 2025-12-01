using System.Collections.Generic;
using UnityEngine;

public class TentacleSprawlController : MonoBehaviour
{
    [Header("Settings")]
    public GameObject legPrefab;
    public int numberOfLegs = 8;

    [Tooltip("Den absoluta maxlängden benen kan ritas.")]
    public float maxReach = 40f;

    public float attackSpeed = 5f;

    [Header("Random Variance (Min/Max)")]
    [Tooltip("Slumpa trigger-avståndet varje gång.")]
    public float minTriggerDist = 35f;
    public float maxTriggerDist = 40f;

    [Tooltip("Slumpa hur mycket de buktar ut.")]
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
        if (Input.GetMouseButtonDown(0))
        {
            SetNewTarget();
        }

        if (hasActiveTarget && !isAttacking)
        {
            CheckDistanceAndTrigger();
        }

        if (isAttacking)
        {
            UpdateLegsPositions();
        }
    }

    void SetNewTarget()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        currentTarget = mousePos;
        hasActiveTarget = true;

        currentTriggerDistance = Random.Range(minTriggerDist, maxTriggerDist);

        currentArchAmount = Random.Range(minArch, maxArch);

        float currentGrip = Random.Range(minGripRadius, maxGripRadius);
        RecalculateLegOffsets(currentGrip);

        StopAttack();
    }

    void RecalculateLegOffsets(float radius)
    {
        for (int i = 0; i < numberOfLegs; i++)
        {
            legTargetOffsets[i] = (Vector3)Random.insideUnitCircle * radius;
        }
    }

    void CheckDistanceAndTrigger()
    {
        float distanceToTarget = Vector3.Distance(transform.position, currentTarget);

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

    void StopAttack()
    {
        isAttacking = false;
        foreach (var leg in legs)
        {
            leg.SetVisibility(false);
        }
    }

    void UpdateLegsPositions()
    {
        Vector3 startPos = transform.position;

        for (int i = 0; i < legs.Count; i++)
        {
            Vector3 finalTarget = currentTarget + legTargetOffsets[i];

            Vector3 archDir = legArchDirections[i];
            float distance = Vector3.Distance(startPos, finalTarget);

            Vector3 finalArchOffset = archDir * (currentArchAmount * (distance / 5f));

            legs[i].UpdateLeg(startPos, finalTarget, finalArchOffset, attackSpeed);
        }
    }
}
