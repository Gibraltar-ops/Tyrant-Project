using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class TentacleControllerIK : MonoBehaviour
{
    [Header("Capture Visuals")]
    [Tooltip("Dra hit ditt 'VictimVisuals' GameObject")]
    [SerializeField]
    private GameObject victimCaptureVisual;

    [Tooltip("Dra hit ditt 'FighterVisuals' GameObject")]
    [SerializeField]
    private GameObject fighterCaptureVisual;

    private GameObject activeCaptureVisual;
    public GameObject ActiveCaptureVisual => activeCaptureVisual;

    private enum TentacleState
    {
        Idle,
        Reaching,
        PullingSprite,
        Retracting,
        ReturningToIdle,
    }

    private TentacleState currentState = TentacleState.Idle;
    private IGrabbable currentItem;
    private VictimData currentVictim;
    private Transform mouthPoint;
    private Vector3 idleAnchorPosition;
    private bool isBusy = false;

    private Vector3 retractStartPosition;
    private Vector3 retractControlPoint;
    private float retractTimer = 0f;
    public float retractDuration = 1.0f;

    [Header("Grabbing")]
    public float reachSpeed = 20f;
    public float retractSpeed = 5f;

    [SerializeField]
    private float returnToIdleSpeed = 10f;

    [Header("Dependencies")]
    [SerializeField]
    private TentacleSprawlController sprawlController;

    [Header("IK Setup")]
    public TargetJoint2D effectorJoint;
    public Rigidbody2D effectorBody;
    public Rigidbody2D rootBody;

    private Vector3 currentTarget;

    [Header("Visual Tip")]
    public Transform tentacleTip;

    [Header("Stretching Settings")]
    public bool enableStretching = true;
    public float totalUnstretchedLength = 9.5f;
    public Transform rootBone;
    public Transform[] bonesToStretch;

    public float stretchSmoothSpeed = 10f;

    private List<Vector3> initialBoneScales = new List<Vector3>();
    private float currentStretchFactor = 1.0f;

    [Header("IK Dead Zone & Response")]
    public float targetDeadZone = 0.5f;

    [Header("Target Joint Dynamics")]
    public float maxJointFrequency = 80f;
    public float minJointFrequency = 0.5f;

    [Tooltip("relaxation distance")]
    public float relaxationDistance = 2.0f;

    [Tooltip("Hävstångs-counter")]
    public float stretchedForceMultiplier = 1.5f;

    void Start()
    {
        if (bonesToStretch != null)
        {
            foreach (Transform bone in bonesToStretch)
            {
                initialBoneScales.Add(bone.localScale);
            }
        }
        else { }

        idleAnchorPosition = effectorBody.position; // Set idle starting point
        currentTarget = idleAnchorPosition;

        if (sprawlController == null)
            sprawlController = GetComponentInChildren<TentacleSprawlController>();

        if (victimCaptureVisual != null)
            victimCaptureVisual.SetActive(false);
        if (fighterCaptureVisual != null)
            fighterCaptureVisual.SetActive(false);
    }

    void LateUpdate()
    {
        UpdateState();

        // Target Joint logic
        if (effectorJoint != null)
        {
            effectorJoint.target = currentTarget;
        }

        // stretch?
        HandleStretching();

        // apply force / tension on length
        AdjustTargetJointTension();

        // rotate tip
        if (tentacleTip != null && effectorBody != null)
        {
            Vector3 directionToTarget = currentTarget - (Vector3)effectorBody.position;
            float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
            tentacleTip.rotation = Quaternion.Euler(0f, 0f, angle - -90f);
        }
    }

    private void UpdateState()
    {
        switch (currentState)
        {
            case TentacleState.Idle:
                // Move tentacle around their idle position
                float noiseX = (Mathf.PerlinNoise(Time.time * 0.5f, 0) - 0.5f) * 2f;
                float noiseY = (Mathf.PerlinNoise(0, Time.time * 0.5f) - 0.5f) * 2f;
                currentTarget = idleAnchorPosition + new Vector3(noiseX, noiseY, 0);
                break;

            case TentacleState.Reaching:
                // set target to grabbable pos
                if (currentItem != null)
                {
                    currentTarget = currentItem.Position;
                }
                break;

            case TentacleState.PullingSprite:
                if (activeCaptureVisual == null)
                    break;

                activeCaptureVisual.transform.position = Vector3.MoveTowards(
                    activeCaptureVisual.transform.position,
                    effectorBody.position,
                    Time.deltaTime * reachSpeed * 2f
                );

                if (
                    Vector3.Distance(activeCaptureVisual.transform.position, effectorBody.position)
                    < 0.1f
                )
                {
                    activeCaptureVisual.transform.SetParent(effectorBody.transform);
                    retractStartPosition = effectorBody.position;
                    retractTimer = 0f;
                    currentState = TentacleState.Retracting;
                }
                break;

            case TentacleState.Retracting:
                currentTarget = mouthPoint.position;

                if (Vector3.Distance(effectorBody.position, mouthPoint.position) < 0.5f)
                {
                    ConsumeItem();
                    currentState = TentacleState.ReturningToIdle;
                }
                break;

            case TentacleState.ReturningToIdle:
                // return to idle position
                currentTarget = Vector3.MoveTowards(
                    currentTarget,
                    idleAnchorPosition,
                    Time.deltaTime * returnToIdleSpeed
                );
                if (Vector3.Distance(currentTarget, idleAnchorPosition) < 0.1f)
                {
                    currentState = TentacleState.Idle;
                    isBusy = false;
                }
                break;
        }
    }

    public void InitiateCapture(IGrabbable capturedItem)
    {
        if (capturedItem.Type == GrabbableType.Victim)
        {
            activeCaptureVisual = victimCaptureVisual;
        }
        else if (capturedItem.Type == GrabbableType.Fighter)
        {
            activeCaptureVisual = fighterCaptureVisual;
        }

        if (activeCaptureVisual != null)
        {
            activeCaptureVisual.SetActive(true);
            activeCaptureVisual.transform.SetParent(null);
            activeCaptureVisual.transform.position = capturedItem.Position;

            TyrantTentacleManager manager = FindObjectOfType<TyrantTentacleManager>();
            if (manager != null)
            {
                manager.DestroyGrabbable(capturedItem);
            }

            currentState = TentacleState.PullingSprite;
        }
        else { }
    }

    public void OnVictimCapturedBySprawl()
    {
        if (currentState == TentacleState.Reaching)
        {
            currentState = TentacleState.Retracting;
        }
    }

    private void AttachVictim() { }

    public void StartGrab(IGrabbable item, Transform mouth)
    {
        if (isBusy)
            return;

        currentItem = item;
        mouthPoint = mouth;
        isBusy = true;
        currentState = TentacleState.Reaching;

        // Activate sprawl tentacles
        if (sprawlController != null)
        {
            sprawlController.SetNewTarget(item.Position, item);
        }
    }

    public bool IsBusy()
    {
        return isBusy;
    }

    private void ConsumeItem()
    {
        if (activeCaptureVisual != null)
        {
            activeCaptureVisual.SetActive(false);
            activeCaptureVisual = null;
        }

        if (currentItem != null)
        {
            currentItem = null;
        }
    }

    private void AdjustTargetJointTension()
    {
        if (effectorJoint == null || effectorBody == null)
            return;

        float distanceToTarget = Vector3.Distance(effectorBody.position, effectorJoint.target);

        // Normalize tension
        float tensionFactor = Mathf.Clamp01(distanceToTarget / relaxationDistance);

        // calulate frequency
        float targetFreq = Mathf.Lerp(minJointFrequency, maxJointFrequency, tensionFactor);

        if (currentStretchFactor > 1.1f)
        {
            targetFreq *= stretchedForceMultiplier;
        }

        effectorJoint.frequency = targetFreq;
    }

    void HandleStretching()
    {
        if (
            rootBone == null
            || bonesToStretch == null
            || initialBoneScales.Count != bonesToStretch.Length
        )
            return;

        float targetStretch = 1.0f;

        if (enableStretching)
        {
            float distanceToTarget = Vector3.Distance(rootBone.position, currentTarget);

            if (distanceToTarget > totalUnstretchedLength)
            {
                targetStretch = distanceToTarget / totalUnstretchedLength;
            }
        }

        // Lerp softness
        currentStretchFactor = Mathf.Lerp(
            currentStretchFactor,
            targetStretch,
            Time.deltaTime * stretchSmoothSpeed
        );

        for (int i = 0; i < bonesToStretch.Length; i++)
        {
            Transform bone = bonesToStretch[i];
            Vector3 originalScale = initialBoneScales[i];

            // apply scale
            bone.localScale = new Vector3(
                originalScale.x,
                originalScale.y * currentStretchFactor,
                originalScale.z
            );
        }
    }
}
