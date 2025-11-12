using UnityEngine;

public class TentacleGrabber : MonoBehaviour
{
    public string targetTag = "Human"; // Tag of objects to pull
    private Transform grabbedHuman = null;

    private void OnTriggerEnter2D(Collider2D Human)
    {
        if (grabbedHuman == null && Human.CompareTag(targetTag))
        {
            grabbedHuman = Human.transform;

            grabbedHuman.SetParent(transform);

            if (Human.TryGetComponent<HumanMovement>(out var humanMovement))
            {
                humanMovement.enabled = false;
            }

            if (Human.TryGetComponent<Rigidbody2D>(out var rb))
            {
                rb.bodyType = RigidbodyType2D.Kinematic;
            }
        }
    }
}
