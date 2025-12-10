using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Lood
{
    public class UpgradeableObjectShopButton: MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public UpgradeableObjectTag upgradeableObjectTag;

    //Event is received on the shop objects in the scene.
    public delegate void OnCollectorPurchased(UpgradeableObjectTag tag);
    public static event OnCollectorPurchased onCollectorPurchased;

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
            CheckInteractable(UpgradeManager.Instance.UpgradeableObjects[upgradeableObjectTag].Shop.BoughtWithCurrency);
        }
    
    private void CheckInteractable(Currency currency)
    {
        if(currency != UpgradeManager.Instance.UpgradeableObjects[upgradeableObjectTag].Shop.BoughtWithCurrency)
        {
            return;
        }

        if (UpgradeManager.Instance.UpgradeableObjects[upgradeableObjectTag].Shop.MaxedOut)
        {
            return;
        }

        if(EconomyManager.Instance.CanAfford(currency, (int)UpgradeManager.Instance.UpgradeableObjects[upgradeableObjectTag].Shop.NextPrice))
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
        onCollectorPurchased?.Invoke(upgradeableObjectTag);
    }
}
}

