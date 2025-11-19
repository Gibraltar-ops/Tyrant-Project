using UnityEngine;

[System.Serializable]
public class SpawnRegion
{
    public Vector2 center;
    public Vector2 size;

    public Vector2 RandomPointInside()
    {
        float x = Random.Range(center.x - size.x / 2f, center.x + size.x / 2f);
        float y = Random.Range(center.y - size.y / 2f, center.y + size.y / 2f);

        return new Vector2(x, y);
    }

    public void DrawGizmo(Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawWireCube(center, size);
    }
}
