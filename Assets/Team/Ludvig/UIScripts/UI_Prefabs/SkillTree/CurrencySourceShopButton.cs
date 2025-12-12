using System.Collections.Generic;
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
    public bool Active = false;
    public List<GameObject> ProgressBars;

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

    public void Activate()
        {
            Active = true;
            foreach (GameObject bar in ProgressBars)
            {
                Image image = bar.GetComponent<Image>();
                bar.GetComponent<Image>().color = new Color(image.color.r, image.color.g, image.color.b, 1f);
            }
        }
    
    private void Initialize()
        {
            
            CheckInteractable(UpgradeManager.Instance.CurrencySources[currencySourceTag].Shop.BoughtWithCurrency);
            if (!Active)
            {
                foreach(GameObject bar in ProgressBars)
                {
                    Image image = bar.GetComponent<Image>();
                bar.GetComponent<Image>().color = new Color(image.color.r, image.color.g, image.color.b, 0f);
                }
            }
        }
    
    private void CheckInteractable(Currency currency)
    {
        if (!Active)
        {
            GetComponent<Button>().interactable = false;
            return; 
        }

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

