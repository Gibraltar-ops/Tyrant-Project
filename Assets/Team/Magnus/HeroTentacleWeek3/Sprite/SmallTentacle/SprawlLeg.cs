using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SprawlLeg : MonoBehaviour
{
    private LineRenderer lr;
    private int pointsCount = 20;

    // Check how far the bone has stretched
    private float extensionProgress = 0f;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = pointsCount;
        lr.enabled = false; // invisible from start
    }

    public void ResetLeg()
    {
        extensionProgress = 0f;
        SetVisibility(false);
    }

    public void ResetProgress()
    {
        extensionProgress = 0f;
    }

    public void SetVisibility(bool visible)
    {
        lr.enabled = visible;
        if (!visible)
            extensionProgress = 0f;
    }

    public void UpdateLeg(Vector3 startPos, Vector3 targetPos, Vector3 archOffset, float speed)
    {
        bool shouldRetract = (targetPos == startPos);
        if (shouldRetract)
        {
            extensionProgress = Mathf.MoveTowards(
                extensionProgress,
                0f,
                Time.deltaTime * speed * 1.5f
            );
        }
        else
        {
            //Animate tentacle attac
            extensionProgress = Mathf.MoveTowards(extensionProgress, 1f, Time.deltaTime * speed);
        }
        if (extensionProgress <= 0.01f && shouldRetract)
        {
            SetVisibility(false);
            return;
        }

        // calculate end position
        Vector3 currentEndPos = Vector3.Lerp(startPos, targetPos, extensionProgress);

        //Bezier control calculation
        Vector3 midPoint = (startPos + currentEndPos) / 2f;

        // Wiggle
        float wiggle = Mathf.Sin(Time.time * 10f) * 0.1f;
        Vector3 controlPoint = midPoint + archOffset + (Vector3.up * wiggle);

        // draw curve
        for (int i = 0; i < pointsCount; i++)
        {
            float t = i / (float)(pointsCount - 1);
            Vector3 pixelPos = CalculateQuadraticBezier(t, startPos, controlPoint, currentEndPos);
            lr.SetPosition(i, pixelPos);
        }
    }

    public bool IsRetracted()
    {
        return extensionProgress <= 0.01f;
    }

    public bool IsFullyExtended()
    {
        return extensionProgress >= 1f;
    }

    // bezier math
    private Vector3 CalculateQuadraticBezier(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        return (uu * p0) + (2 * u * t * p1) + (tt * p2);
    }
}
