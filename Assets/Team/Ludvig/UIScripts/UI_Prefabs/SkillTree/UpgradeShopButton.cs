using UnityEngine;
using UnityEngine.UI;
using Lood;

public class UpgradeShopButton: MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public UpgradeTag upgradeTag;

    public delegate void OnUpgradePurchased(UpgradeTag tag);
    public static event OnUpgradePurchased onUpgradePurchased;

    void OnEnable()
    {
        EconomyManager.onCurrencyChanged += CheckInteractable;
        
    }
    void OnDisable()
    {
        EconomyManager.onCurrencyChanged -= CheckInteractable;
    }

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(Buy);
        Invoke("Initialize", 0.01f);
        
    }

    private void Initialize()
        {
        CheckInteractable(Lood.UpgradeManager.Instance.Upgrades[upgradeTag].Shop.BoughtWithCurrency);
        }
    
    private void CheckInteractable(Currency currency)
    {
        if(currency != Lood.UpgradeManager.Instance.Upgrades[upgradeTag].Shop.BoughtWithCurrency)
        {
            return;
        }
        if (Lood.UpgradeManager.Instance.Upgrades[upgradeTag].Shop.MaxedOut)
        {
            return;
        }
        
        if(EconomyManager.Instance.CanAfford(currency, (int)Lood.UpgradeManager.Instance.Upgrades[upgradeTag].Shop.NextPrice))
        {
            GetComponent<Button>().interactable = true;
        }
        else
        {
            GetComponent<Button>().interactable = false;
        }
    }

    private void Buy()
    {
        onUpgradePurchased?.Invoke(upgradeTag);
    }
}
