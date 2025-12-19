using UnityEngine;

public class UnitVisual : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    public Transform gfx;
    public GameObject dummyVisual;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void ApplyScale(float scale)
    {
        gfx.localScale = Vector3.one * scale; // scaling only on child
    }

    // Called by VictimRenderer every frame
    public void UpdateVisual(Vector2 position, Vector2 movementDirection)
    {
        transform.position = position;

        // Flip left/right
        if (movementDirection.x < -0.08f)
            spriteRenderer.flipX = false;
        else if (movementDirection.x > 0.08f)
            spriteRenderer.flipX = true;
    }
}

