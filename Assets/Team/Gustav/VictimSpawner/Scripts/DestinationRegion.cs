using UnityEngine;

[System.Serializable]
public class DestinationRegion
{
    public Vector2 center = Vector2.zero;
    public float radius = 5f;

    // Returns a random point inside the circle (uniform distribution)
    public Vector2 RandomPointInside()
    {
        // Use Random.insideUnitCircle for perfect circular distribution
        return center + (Random.insideUnitCircle * radius);
    }

    public void DrawGizmo(Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawWireSphere(center, radius);
    }
}
