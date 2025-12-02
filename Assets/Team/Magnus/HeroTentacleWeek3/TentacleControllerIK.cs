using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class TentacleControllerIK : MonoBehaviour
{
    [Header("Capture Sprite")]
    [SerializeField]
    private CaptureSprite captureSprite;

    private enum TentacleState
    {
        Idle,
        Reaching,
        PullingSprite,
        Retracting,
        ReturningToIdle,
    }

    private TentacleState currentState = TentacleState.Idle;

    private VictimData currentVictim;
    private Transform mouthPoint;
    private Vector3 idleAnchorPosition;
    private bool isBusy = false;

    [Header("Grabbing")]
    public float reachSpeed = 20f;
    public float retractSpeed = 5f;

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
        else
        {
            Debug.LogError("BonesToStretch är inte tilldelad i inspectorn!");
        }

        idleAnchorPosition = effectorBody.position; // Set idle starting point
        currentTarget = idleAnchorPosition;

        if (sprawlController == null)
            sprawlController = GetComponentInChildren<TentacleSprawlController>();
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
                // Sätt target to victim pos
                currentTarget = currentVictim.position;

                currentTarget = currentVictim.position;
                break;

            case TentacleState.PullingSprite:
                // pull towards tentacle main
                captureSprite.transform.position = Vector3.MoveTowards(
                    captureSprite.transform.position,
                    effectorBody.position,
                    Time.deltaTime * reachSpeed * 2f
                );

                // if sprite reached
                if (
                    Vector3.Distance(captureSprite.transform.position, effectorBody.position) < 0.1f
                )
                {
                    // attach sprite to main
                    captureSprite.transform.SetParent(effectorBody.transform);
                    currentState = TentacleState.Retracting;
                }
                break;

            case TentacleState.Retracting:
                currentTarget = mouthPoint.position;

                if (captureSprite != null && !captureSprite.gameObject.activeSelf)
                {
                    Debug.Log("Capture sprite consumed. Finalizing.");

                    ConsumeVictim();
                    currentState = TentacleState.ReturningToIdle;
                }
                break;

            case TentacleState.ReturningToIdle:
                // return to idle position
                currentTarget = Vector3.MoveTowards(
                    currentTarget,
                    idleAnchorPosition,
                    Time.deltaTime * reachSpeed
                );
                if (Vector3.Distance(currentTarget, idleAnchorPosition) < 0.1f)
                {
                    currentState = TentacleState.Idle;
                    isBusy = false;
                }
                break;
        }
    }

    public CaptureSprite InitiateCapture(Vector3 victimPosition)
    {
        if (captureSprite != null)
        {
            captureSprite.gameObject.SetActive(true);
            captureSprite.transform.SetParent(null); // release from sprawl
            captureSprite.transform.position = victimPosition;
            currentState = TentacleState.PullingSprite; // start pulling in
            return captureSprite;
        }

        return null;
    }

    public void OnVictimCapturedBySprawl()
    {
        if (currentState == TentacleState.Reaching)
        {
            Debug.Log("Sprawl has captured victim, main tentacle retracting");
            currentState = TentacleState.Retracting;
        }
    }

    private void AttachVictim() { }

    public void StartGrab(VictimData victim, Transform mouth)
    {
        if (isBusy)
            return;

        currentVictim = victim;
        mouthPoint = mouth;
        isBusy = true;
        currentState = TentacleState.Reaching;

        // Activate sprawl tentacles
        if (sprawlController != null)
        {
            sprawlController.SetNewTarget(victim.position, victim);
        }
    }

    public bool IsBusy()
    {
        return isBusy;
    }

    private void ConsumeVictim()
    {
        if (captureSprite != null)
        {
            captureSprite.gameObject.SetActive(false);
        }

        if (currentVictim != null)
        {
            Debug.Log("Giving currency for victim.");

            currentVictim = null;
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
