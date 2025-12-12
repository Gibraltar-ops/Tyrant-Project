using UnityEngine;
using UnityEngine.UI;
using Lood;
using System.Collections.Generic;

public class UpgradeShopButton: MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public UpgradeDataSO upgradeData;
    public bool _maxedOut = false;
    public bool Active = false;
    public List<GameObject> ProgressBars;

    public delegate void OnUpgradePurchased(string tag);
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
        GetComponentInChildren<UpgradeShopButton>().upgradeData = upgradeData;
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
        
        CheckInteractable(Lood.UpgradeManager.Instance.Upgrades[upgradeData.Tag].Shop.BoughtWithCurrency);

        if (!Active)
        {
            foreach (GameObject bar in ProgressBars)
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

        if (Lood.UpgradeManager.Instance.Upgrades[upgradeData.Tag].Shop.MaxedOut)
        {   
            GetComponent<Button>().interactable = false;
            _maxedOut = true;
            return;
        }

        if(currency != Lood.UpgradeManager.Instance.Upgrades[upgradeData.Tag].Shop.BoughtWithCurrency)
        {
            return;
        }
        if (Lood.UpgradeManager.Instance.Upgrades[upgradeData.Tag].Shop.MaxedOut)
        {
            return;
        }
        
        if(EconomyManager.Instance.CanAfford(currency, (int)Lood.UpgradeManager.Instance.Upgrades[upgradeData.Tag].Shop.NextPrice))
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
        onUpgradePurchased?.Invoke(upgradeData.Tag);
    }
}
