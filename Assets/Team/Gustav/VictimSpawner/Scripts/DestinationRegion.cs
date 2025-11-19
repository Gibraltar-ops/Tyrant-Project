using UnityEngine;

[System.Serializable]
public class DestinationRegion
{
    public Vector2 center = Vector2.zero;
    public float radius = 5f;

    // Returns a random point inside the circle (uniform distribution)
    public Vector2 RandomPointInside()
    {
        return center + (Random.insideUnitCircle * radius);
    }
    public bool Contains(Vector2 point)
    {
        return Vector2.Distance(point, center) <= radius;
    }

    public void DrawGizmo(Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawWireSphere(center, radius);
    }
}
