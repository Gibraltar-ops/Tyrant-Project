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

    void Start()
    {
        if (larynxUpgradeButton != null)
        {
            larynxUpgradeButton.SetActive(false);
        }
    }

    void Update()
    {
        if (!larynxIsPurchased && gameManager.biomass >= larynxCost)
        {
            larynxUpgradeButton.SetActive(true);
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
