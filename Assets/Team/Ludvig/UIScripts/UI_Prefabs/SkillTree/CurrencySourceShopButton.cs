using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Lood
{
    public class CurrencySourceShopButton: MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public CurrencySourceTag currencySourceTag;
    public bool _maxedOut = false;

    //Event is received on the shop objects in the scene.
    public delegate void OnCollectorPurchased(CurrencySourceTag tag);
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
        GetComponentInChildren<CurrencySourceShopButton>().currencySourceTag = currencySourceTag;
        Invoke("Initialize", 0.01f);
        
    }

    private void Initialize()
        {
            
            CheckInteractable(UpgradeManager.Instance.CurrencySources[currencySourceTag].Shop.BoughtWithCurrency);
        }
    
    private void CheckInteractable(Currency currency)
    {
        if(_maxedOut) return;

        if (UpgradeManager.Instance.CurrencySources[currencySourceTag].Shop.MaxedOut)
        {   
            GetComponent<Button>().interactable = false;
            _maxedOut = true;
            return;
        }

        if(currency != UpgradeManager.Instance.CurrencySources[currencySourceTag].Shop.BoughtWithCurrency)
        {
            return;
        }

        if (UpgradeManager.Instance.CurrencySources[currencySourceTag].Shop.MaxedOut)
        {
            return;
        }

        if(EconomyManager.Instance.CanAfford(currency, (int)UpgradeManager.Instance.CurrencySources[currencySourceTag].Shop.NextPrice))
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
        onCollectorPurchased?.Invoke(currencySourceTag);
    }
}
}

