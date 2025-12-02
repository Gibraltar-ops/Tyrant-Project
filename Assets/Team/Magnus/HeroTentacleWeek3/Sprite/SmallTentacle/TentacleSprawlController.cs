using System.Collections.Generic;
using UnityEngine;

public class TentacleSprawlController : MonoBehaviour
{
    [Header("Settings")]
    public GameObject legPrefab;
    public int numberOfLegs = 8;

    public float maxReach = 30f;

    public float attackSpeed = 1f;

    [Header("Random Variance (Min/Max)")]
    public float minTriggerDist = 45f;
    public float maxTriggerDist = 45f;

    public float minArch = 0.2f;
    public float maxArch = 0.8f;

    [Tooltip("Slumpa hur mycket de sprider sig vid målet.")]
    public float minGripRadius = 0.5f;
    public float maxGripRadius = 0.8f;

    [Header("Leg animation")]
    [SerializeField]
    private float legAnimSpeed = 5f;

    [Header("References")]
    public TentacleControllerIK mainController;

    private List<SprawlLeg> legs = new List<SprawlLeg>();
    private IGrabbable itemToPull;

    private bool isAttacking = false;

    private bool isRetracting = false;
    private bool hasActiveTarget = false;
    private Vector3 currentTarget;

    /* private CaptureSprite activeCaptureSprite; */

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
        if (hasActiveTarget && !isAttacking && !isRetracting)
        {
            CheckDistanceAndTrigger();
        }

        if (isAttacking || isRetracting)
        {
            UpdateLegsPositions();
        }
    }

    public void SetNewTarget(Vector3 targetPosition, IGrabbable item)
    {
        currentTarget = targetPosition;
        itemToPull = item;
        hasActiveTarget = true;

        currentTriggerDistance = Random.Range(minTriggerDist, maxTriggerDist);
        currentArchAmount = Random.Range(minArch, maxArch);
        float currentGrip = Random.Range(minGripRadius, maxGripRadius);
        RecalculateLegOffsets(currentGrip);

        ResetLegsForNewAttack();
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

        // check distances
        Debug.Log(
            $"Checking distance: {distanceToTarget} vs Trigger distance: {currentTriggerDistance}"
        );

        // use random distance
        if (distanceToTarget <= currentTriggerDistance)
        {
            // see if sprawl activate
            Debug.Log("!!! STARTING SPRAWL ATTACK !!!");
            StartAttack();
        }
    }

    void StartAttack()
    {
        isAttacking = true;

        isRetracting = false;
        hasActiveTarget = false;
        foreach (var leg in legs)
        {
            leg.SetVisibility(true);
        }
    }

    public void StartRetracting()
    {
        isRetracting = true;

        isAttacking = false;
    }

    public void StopAttack()
    {
        isAttacking = false;
        isRetracting = false;
        hasActiveTarget = false;
        itemToPull = null;

        foreach (var leg in legs)
        {
            leg.ResetLeg();
        }
    }

    private void ResetLegsForNewAttack()
    {
        isAttacking = false;
        isRetracting = false;
        foreach (var leg in legs)
        {
            leg.ResetProgress();
        }
    }

    /* void UpdateLegsPositions()
    {
        Vector3 startPos = mainController.effectorBody.position;
        Vector3 targetForLegs;

        if (isRetracting)
        {
            if (activeCaptureSprite != null)
            {
                targetForLegs = activeCaptureSprite.transform.position;
                float distanceToEffector = Vector3.Distance(targetForLegs, startPos);
                if (distanceToEffector < 0.05f)
                {
                    StopAttack();
                    return;
                }
            }
            else
            {
                targetForLegs = startPos;
            }
        }
        else
        {
            if (itemToPull != null)
            {
                targetForLegs = itemToPull.Position;
            }
            else
            {
                targetForLegs = currentTarget;
            }
        }

        for (int i = 0; i < legs.Count; i++)
        {
            Vector3 finalTarget = targetForLegs + legTargetOffsets[i];
            Vector3 archOffset = legArchDirections[i] * currentArchAmount;
            legs[i].UpdateLeg(startPos, finalTarget, archOffset, legAnimSpeed);
        }

        if (isAttacking && AllLegsExtended())
        {
            Debug.Log("!!! SWAP !!! Legs extended. Handing over to main tentacle.");

            if (mainController != null && itemToPull != null)
            {
                activeCaptureSprite = mainController.InitiateCapture(itemToPull);
            }

            isAttacking = false;
            isRetracting = true;
        }
    }
 */

    void UpdateLegsPositions()
    {
        Vector3 startPos = mainController.effectorBody.position;
        Vector3 targetForLegs;

        if (isRetracting)
        {
            targetForLegs = startPos;
            if (AllLegsRetracted())
            {
                StopAttack();
                return;
            }
        }
        else
        {
            if (itemToPull != null)
            {
                targetForLegs = itemToPull.Position;
            }
            else
            {
                targetForLegs = currentTarget;
            }
        }

        for (int i = 0; i < legs.Count; i++)
        {
            Vector3 finalTarget = targetForLegs + legTargetOffsets[i];
            Vector3 archOffset = legArchDirections[i] * currentArchAmount;
            legs[i].UpdateLeg(startPos, finalTarget, archOffset, legAnimSpeed);
        }

        if (isAttacking && AllLegsExtended())
        {
            Debug.Log("!!! SWAP !!! Legs extended. Handing over to main tentacle.");

            if (mainController != null && itemToPull != null)
            {
                mainController.InitiateCapture(itemToPull);
            }

            isAttacking = false;
            isRetracting = true;
        }
    }

    private bool AllLegsExtended()
    {
        foreach (var leg in legs)
        {
            if (!leg.IsFullyExtended())
            {
                return false;
            }
        }
        return true;
    }

    private bool AllLegsRetracted()
    {
        foreach (var leg in legs)
        {
            if (!leg.IsRetracted())
            {
                return false;
            }
        }
        return true;
    }
}
