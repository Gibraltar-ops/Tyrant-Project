using UnityEngine;

public class DieInTrigger : MonoBehaviour
{
    [SerializeField]
    private string triggerTag = "MouthZone";

    private float biomassValue = 10;

    private GameManager gameManager;
    private TyrantController tyrantController;

    private void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        tyrantController = FindFirstObjectByType<TyrantController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(triggerTag))
        {
            Debug.Log("Tyrant ate " + gameObject.name + "... Om nom nom");
            Destroy(gameObject);
            gameManager?.AddBiomass(tyrantController.biomassPerHuman);

        }
    }
}