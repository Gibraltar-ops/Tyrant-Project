using UnityEngine;

public class UpgradeShop : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private TyrantController tyrantController;

    [Header("Larynx Ability Upgrade")]
    [SerializeField] private GameObject larynxUpgradeButton;
    [SerializeField] private int larynxCost = 150;
    private bool larynxIsPurchased = false;
    private bool initialized = false;

    void Start()
    {
        if (larynxUpgradeButton != null)
        {
            larynxUpgradeButton.SetActive(false);
        }

        Invoke("SetLarynxActive", 0.05f);
    }

    public void SetLarynxActive()
    {
        larynxIsPurchased = tyrantController.larynxIsActive;
        Debug.Log("Larynx is purchased = " + larynxIsPurchased);
        initialized = true;
    }

    void Update()
    {
        if (!larynxIsPurchased && gameManager.biomass >= larynxCost && initialized)
        {
            larynxUpgradeButton.SetActive(true);
        }
        else
        {
            larynxUpgradeButton.SetActive(false);
        }
    }

    public void PurchaseLarynx()
    {
        if (!larynxIsPurchased && gameManager.biomass >= larynxCost)
        {
            gameManager.biomass -= larynxCost;

            tyrantController.ActivateLarynx();

            larynxIsPurchased = true;
            larynxUpgradeButton.SetActive(false);
        }
    }
}
