using UnityEngine;

public class DieInTrigger : MonoBehaviour
{
    [SerializeField]
    private string triggerTag = "MouthZone";

    private float biomassValue = 10;

    private GameManager gameManager;
    private TyrantController tyrantController;

    public delegate void OnHumanEaten();
    public static event OnHumanEaten onHumanEaten;

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
            onHumanEaten?.Invoke();
            Destroy(gameObject);
            gameManager?.AddBiomass(tyrantController.biomassPerHuman);

        }
    }
}