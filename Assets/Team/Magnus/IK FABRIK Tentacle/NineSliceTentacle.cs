using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class NineSliceWorldObject : MonoBehaviour
{
    [Header("Storlek i world units")]
    public Vector2 targetSize = new Vector2(4f, 2f); // bredd x höjd i världsenheter

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

        // Se till att rätt läge används
        sr.drawMode = SpriteDrawMode.Sliced;

        // Sätt storleken (world units)
        sr.size = targetSize;
    }

    // Om du vill kunna ändra live i Editorn:
    private void OnValidate()
    {
        if (sr == null)
            sr = GetComponent<SpriteRenderer>();

        sr.drawMode = SpriteDrawMode.Sliced;
        sr.size = targetSize;
    }
}
