using UnityEngine;

[System.Serializable]
public class SpawnRegion
{
    public Vector2 center = Vector2.zero;
    public Vector2 size = new Vector2(2f, 2f);

    // Returns a random point inside the rectangle
    public Vector2 RandomPointInside()
    {
        float x = Random.Range(center.x - size.x * 0.5f, center.x + size.x * 0.5f);
        float y = Random.Range(center.y - size.y * 0.5f, center.y + size.y * 0.5f);
        return new Vector2(x, y);
    }

    public void DrawGizmo(Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawWireCube(center, size);
    }
}
