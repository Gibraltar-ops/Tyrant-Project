using UnityEngine;

public class DieInTrigger : MonoBehaviour
{
    [SerializeField]
    private string triggerTag = "MouthZone";

    private double biomassValue = 10;

    private GameManager gameManager;

    private void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(triggerTag))
        {
            Debug.Log("Tyrant ate " + gameObject.name + "... Om nom nom");
            Destroy(gameObject);
            gameManager?.AddBiomass(biomassValue);
        }
    }
}