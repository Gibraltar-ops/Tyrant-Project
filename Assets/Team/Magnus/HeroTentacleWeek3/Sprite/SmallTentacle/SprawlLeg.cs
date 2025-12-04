using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SprawlLeg : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Transform startBone;

    [Header("Settings")]
    [SerializeField]
    private int pointsCount = 20;

    private LineRenderer lr;
    private float extensionProgress = 0f;
    private float currentStartDelay = 0f;
    private float personalSpeed = 1f;
    private bool isWaitingToStart = false;
    private float randomWiggleOffset = 0f;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = pointsCount;
        lr.enabled = false;
    }

    public void SetStartBone(Transform boneTransform)
    {
        startBone = boneTransform;
    }

    public void InitializeAttack(float delay, float speed)
    {
        currentStartDelay = delay;
        personalSpeed = speed;
        extensionProgress = 0f;
        randomWiggleOffset = Random.Range(0f, Mathf.PI * 2);

        if (currentStartDelay > 0f)
        {
            isWaitingToStart = true;
            lr.enabled = false;
        }
        else
        {
            isWaitingToStart = false;
            SetVisibility(true);
        }
    }

    public void SetIdlePose(
        Vector3 startPos,
        Vector3 targetPos,
        Vector3 archOffset,
        float extensionFraction
    )
    {
        extensionFraction = Mathf.Clamp01(extensionFraction);
        isWaitingToStart = false;
        lr.enabled = true;

        // ANVÄND STARTBONE OM DET FINNS
        Vector3 actualStart = (startBone != null) ? startBone.position : startPos;

        Vector3 currentEndPos = Vector3.Lerp(actualStart, targetPos, extensionFraction);
        Vector3 midPoint = (actualStart + currentEndPos) / 2f;
        Vector3 controlPoint = midPoint + archOffset;

        DrawBezier(actualStart, controlPoint, currentEndPos);
    }

    public void UpdateLeg(
        Vector3 startPos,
        Vector3 targetPos,
        Vector3 archOffset,
        bool isRetracting
    )
    {
        if (isRetracting && isWaitingToStart)
        {
            ResetLeg();
            return;
        }

        if (!isRetracting && isWaitingToStart)
        {
            currentStartDelay -= Time.deltaTime;
            if (currentStartDelay <= 0f)
            {
                isWaitingToStart = false;
                lr.enabled = true;
            }
            else
                return;
        }

        // Hantera progress
        float targetProgress = isRetracting ? 0f : 1f;
        float speedMult = isRetracting ? 2.0f : 1.0f;

        extensionProgress = Mathf.MoveTowards(
            extensionProgress,
            targetProgress,
            Time.deltaTime * personalSpeed * speedMult
        );

        if (extensionProgress <= 0.001f && isRetracting)
        {
            SetVisibility(false);
            return;
        }

        Vector3 actualStart = (startBone != null) ? startBone.position : startPos;

        Vector3 currentEndPos = Vector3.Lerp(actualStart, targetPos, extensionProgress);
        Vector3 midPoint = (actualStart + currentEndPos) / 2f;

        // Wiggle math
        float wiggleFrequency = 10f * personalSpeed;
        float timeValue = (Time.time * wiggleFrequency) + randomWiggleOffset;
        float wiggle = Mathf.Sin(timeValue) * 0.1f;

        Vector3 wiggleDir = (startBone != null) ? startBone.up : Vector3.up;
        Vector3 controlPoint = midPoint + archOffset + (wiggleDir * wiggle);

        DrawBezier(actualStart, controlPoint, currentEndPos);
    }

    private void DrawBezier(Vector3 p0, Vector3 p1, Vector3 p2)
    {
        for (int i = 0; i < pointsCount; i++)
        {
            float t = i / (float)(pointsCount - 1);
            Vector3 pixelPos = CalculateQuadraticBezier(t, p0, p1, p2);
            lr.SetPosition(i, pixelPos);
        }
    }

    public void ResetLeg()
    {
        extensionProgress = 0f;
        isWaitingToStart = false;
        SetVisibility(false);
    }

    public void SetVisibility(bool visible)
    {
        if (visible && isWaitingToStart)
            return;
        lr.enabled = visible;
        if (!visible)
            extensionProgress = 0f;
    }

    public bool IsRetracted() => extensionProgress <= 0.001f && !isWaitingToStart;

    public bool IsFullyExtended() => extensionProgress >= 1f;

    private Vector3 CalculateQuadraticBezier(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        return (uu * p0) + (2 * u * t * p1) + (tt * p2);
    }
}
