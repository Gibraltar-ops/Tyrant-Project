using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TyrantMovement : MonoBehaviour
{
    [SerializeField]
    private float hoverSpeed = 1f;

    [SerializeField]
    private float hoverAmplitude = 0.5f;

    private Vector3 startPosition;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
    }

    void FixedUpdate()
    {
        Vector2 newPosition =
            startPosition + new Vector3(0, Mathf.Sin(Time.time * hoverSpeed) * hoverAmplitude, 0);
        rb.MovePosition(newPosition);
    }
}
