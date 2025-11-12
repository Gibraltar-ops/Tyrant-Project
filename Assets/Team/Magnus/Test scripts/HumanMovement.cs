using UnityEngine;

public class HumanMovement : MonoBehaviour
{
    [Header("Normal movement")]
    public Transform target;
    public float moveSpeed = 2f;

    [Header("Absorption")]
    [SerializeField] private float absorbSpeed = 6f;
    public bool IsBeingAbsorbed { get; private set; }

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

        if (IsBeingAbsorbed)
        {
            float speed = absorbSpeed;
            Vector2 next = Vector2.MoveTowards(rb.position, (Vector2)target.position, speed * Time.fixedDeltaTime);
            rb.MovePosition(next);


        }
        else
        {
            Vector2 direction = ((Vector2)target.position - rb.position).normalized;
            rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == tyrantController.mouthPoint.gameObject)
        {
            Destroy(gameObject);
        }
    }


    public void StartAbsorption(Transform suctionTarget, float suctionSpeed, double biomass)
    {
        target = suctionTarget;
        absorbSpeed = suctionSpeed;
        biomassValue = biomass;
        IsBeingAbsorbed = true;

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }
}
