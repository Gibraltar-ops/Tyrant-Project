using UnityEngine;

public class HumanMovement : MonoBehaviour
{
    [Header("Normal movement")]
    public Transform target;
    public float moveSpeed = 2f;

    [Header("Grabbing")]
    [SerializeField] private float grabSpeed = 6f;
    public bool IsBeingGrabbed { get; private set; }

    private GameManager gameManager;
    private TyrantController tyrantController;
    private double biomassValue;

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;

        gameManager = FindFirstObjectByType<GameManager>();

    }
    void Start()
    {
        tyrantController = FindFirstObjectByType<TyrantController>();
    }

    void FixedUpdate()
    {
        if (target == null) return;

        {
            Vector2 direction = ((Vector2)target.position - rb.position).normalized;
            rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);
        }
    }

    public void MarkAsGrabbed()
    {
        IsBeingGrabbed = true;
    }

}
