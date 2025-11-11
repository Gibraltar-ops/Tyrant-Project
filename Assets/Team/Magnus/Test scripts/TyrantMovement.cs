using UnityEngine;

public class TyrantMovement : MonoBehaviour
{

    [SerializeField] private float hoverSpeed = 1f;
    [SerializeField] private float hoverAmplitude = 0.5f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        transform.position = startPosition + new Vector3(0, Mathf.Sin(Time.time * hoverSpeed) * hoverAmplitude, 0);
    }
}
