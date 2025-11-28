using UnityEngine;

[System.Serializable]
public class DestinationRegion
{
    [Tooltip("Spawn Region center position.")]
    public Vector2 center = Vector2.zero;

    [Tooltip("Radius of the rounded capsule ends.")]
    public float radius = 10f;

    [Tooltip("Vertical height of the capsule (excluding the caps).")]
    public float length = 30f;

    // Total width = height + 2 * radius

    // -------------------------------------------------------------
    // Random point inside a horizontal capsule
    // -------------------------------------------------------------
    public Vector2 RandomPointInside()
    {
        float halfLen = length * 0.5f;

        // Choose between rectangle or circular ends
        if (Random.value < 0.5f)
        {
            // Rectangle area
            float x = Random.Range(-halfLen, halfLen);
            float y = Random.Range(-radius, radius);
            return center + new Vector2(x, y);
        }
        else
        {
            // Circle caps left or right
            bool left = Random.value < 0.5f;

            Vector2 capCenter = center + new Vector2(
                left ? -halfLen : halfLen,
                0
            );

            return capCenter + Random.insideUnitCircle * radius;
        }
    }

    // -------------------------------------------------------------
    // Check if point is inside this horizontal capsule
    // -------------------------------------------------------------
    public bool Contains(Vector2 point)
    {
        float halfLen = length * 0.5f;

        // Local relative space
        Vector2 local = point - center;

        // Inside the central rectangle?
        if (local.x >= -halfLen && local.x <= halfLen &&
            Mathf.Abs(local.y) <= radius)
        {
            return true;
        }

        // Left/right cap positions
        Vector2 leftCap = new Vector2(-halfLen, 0);
        Vector2 rightCap = new Vector2(halfLen, 0);

        // Inside circular caps?
        if (Vector2.Distance(local, leftCap) <= radius) return true;
        if (Vector2.Distance(local, rightCap) <= radius) return true;

        return false;
    }

    // -------------------------------------------------------------
    // Gizmo Drawing (horizontal capsule)
    // -------------------------------------------------------------
    public void DrawGizmo(Color color)
    {
        Gizmos.color = color;

        float halfLen = length * 0.5f;

        // Rectangle corners
        Vector2 topLeft = center + new Vector2(-halfLen, radius);
        Vector2 topRight = center + new Vector2(halfLen, radius);
        Vector2 bottomLeft = center + new Vector2(-halfLen, -radius);
        Vector2 bottomRight = center + new Vector2(halfLen, -radius);

        // Rectangle lines
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(topLeft, bottomLeft);
        Gizmos.DrawLine(topRight, bottomRight);

        // Caps
        Gizmos.DrawWireSphere(center + new Vector2(-halfLen, 0), radius);
        Gizmos.DrawWireSphere(center + new Vector2(halfLen, 0), radius);
    }
}
