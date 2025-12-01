using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SprawlLeg : MonoBehaviour
{
    private LineRenderer lr;
    private int pointsCount = 20;
    private float extensionProgress = 0f;

    private float currentDelay = 0f;

    private Transform lockedTarget;
    private Vector3 localHitOffset;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = pointsCount;
        lr.enabled = false;
    }

    public void SetVisibility(bool visible)
    {
        if (!visible)
        {
            lr.enabled = false;
            extensionProgress = 0f;
            lockedTarget = null;
            currentDelay = 0f;
        }
    }

    public void BeginAttack(float delay)
    {
        currentDelay = delay;
        extensionProgress = 0f;

        lr.enabled = false;
    }

    public void LockToTarget(Transform target, Vector3 offset)
    {
        lockedTarget = target;
        localHitOffset = offset;
    }

    public void UpdateLeg(
        Vector3 startPos,
        Vector3 defaultTargetPos,
        Vector3 archOffset,
        float speed
    )
    {
        if (currentDelay > 0)
        {
            currentDelay -= Time.deltaTime;
            return;
        }

        if (!lr.enabled)
            lr.enabled = true;

        Vector3 finalEndPos = defaultTargetPos;
        if (lockedTarget != null)
        {
            finalEndPos = lockedTarget.position + localHitOffset;
        }

        extensionProgress = Mathf.MoveTowards(extensionProgress, 1f, Time.deltaTime * speed);

        if (extensionProgress <= 0.001f)
            return;

        Vector3 currentEndPos = Vector3.Lerp(startPos, finalEndPos, extensionProgress);
        Vector3 midPoint = (startPos + currentEndPos) / 2f;

        float wiggle = Mathf.Sin(Time.time * 10f) * 0.1f;
        Vector3 controlPoint = midPoint + archOffset + (Vector3.up * wiggle);

        for (int i = 0; i < pointsCount; i++)
        {
            float t = i / (float)(pointsCount - 1);
            Vector3 pixelPos = CalculateQuadraticBezier(t, startPos, controlPoint, currentEndPos);
            lr.SetPosition(i, pixelPos);
        }
    }

    private Vector3 CalculateQuadraticBezier(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        return (uu * p0) + (2 * u * t * p1) + (tt * p2);
    }
}
