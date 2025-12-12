using UnityEngine;
using Lood;
using System.Collections.Generic;

public class CurrencySourceShop : MonoBehaviour
{
    [HideInInspector]
    public CurrencySourceTag CurrencySource;
    public CurrencySourceSO objectDataSO;
    
    public bool Active = false;
    void OnEnable()
    {   
        CurrencySourceShopButton.onCollectorPurchased += TryPurchase;
        EconomyManager.onCurrencyChanged += CheckAffordability;
    }
    void OnDisable()
    {
        CurrencySourceShopButton.onCollectorPurchased -= TryPurchase;
        EconomyManager.onCurrencyChanged -= CheckAffordability;
    }

    void Start()
    {
        Lood.UpgradeManager.Instance.AddUpgradeableObject(objectDataSO);
        CurrencySource = objectDataSO.Tag;
    } 
    
    private void TryPurchase(CurrencySourceTag tag)
    {
        if(tag != CurrencySource)
        {
            return;
        }
        PurchaseUpgrade();
    }

    public void IncrementPrice()
    {
        CurrencySourceStatBlock stats = Lood.UpgradeManager.Instance.CurrencySources[CurrencySource];
        stats.Shop.NextPrice = (int)(stats.Shop.StartingPrice * Mathf.Pow(stats.Shop.PriceIncrease, stats.Shop.CurrentOwned));
    }

    public void CheckAffordability(Currency currency)
    { 
            CurrencySourceStatBlock stats = Lood.UpgradeManager.Instance.CurrencySources[CurrencySource];
            if (EconomyManager.Instance.CanAfford(stats.Shop.BoughtWithCurrency, (int)stats.Shop.NextPrice))
            {
               stats.Shop.CanAfford = true;
            }
            else
            {
                stats.Shop.CanAfford = false;
            }        
    }

    public void PurchaseUpgrade()
    { 
        CurrencySourceStatBlock stats = Lood.UpgradeManager.Instance.CurrencySources[CurrencySource];
        
        if(stats.Shop.CurrentOwned < stats.Shop.MaxOwned)
        {
        EconomyManager.Instance.IncrementCurrency(stats.Shop.BoughtWithCurrency, -stats.Shop.NextPrice);
        IncrementPrice();
        stats.Shop.CurrentOwned++;
        }

        if(stats.Shop.CurrentOwned == stats.Shop.MaxOwned)
        {
        EconomyManager.Instance.IncrementCurrency(stats.Shop.BoughtWithCurrency, -stats.Shop.NextPrice);
        IncrementPrice();
        //stats.Shop.CurrentOwned++;
        stats.Shop.MaxedOut = true;
        }

        Upgrader[] upgrades = GetComponents<Upgrader>();
        foreach (Upgrader upgrade in upgrades)
        {
            upgrade.ApplyUpgrade(stats.Shop.MaxedOut);
        } 


        
        
    }

    

}
