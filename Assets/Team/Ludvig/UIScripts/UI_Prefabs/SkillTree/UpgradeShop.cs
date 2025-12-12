using UnityEngine;
using Lood;

[RequireComponent(typeof(GenericUpgrader))]

public class UpgradeShop : MonoBehaviour
{

    [HideInInspector]
    public string UpgradeTag;
    public UpgradeDataSO upgradeData;
    public bool Active = false;
    void OnEnable()
    {   
        UpgradeShopButton.onUpgradePurchased += TryPurchase;
        EconomyManager.onCurrencyChanged += CheckAffordability;
    }
    void OnDisable()
    {
        UpgradeShopButton.onUpgradePurchased -= TryPurchase;
        EconomyManager.onCurrencyChanged -= CheckAffordability;
    }
    void Start()
    {
        Lood.UpgradeManager.Instance.AddUpgrade(upgradeData);
        UpgradeTag = upgradeData.Tag;
    }

    private void TryPurchase(string tag)
    {
        if(tag != upgradeData.Tag)
        {
            return;
        }
        PurchaseUpgrade();
    }

    public void IncrementPrice()
    {
        UpgradeStatBlock stats = Lood.UpgradeManager.Instance.Upgrades[upgradeData.Tag];
        stats.Shop.NextPrice = (int)(stats.Shop.StartingPrice * Mathf.Pow(stats.Shop.PriceIncrease, stats.Shop.CurrentOwned));
    }

    public void CheckAffordability(Currency currency)
    { 
            UpgradeStatBlock stats = Lood.UpgradeManager.Instance.Upgrades[upgradeData.Tag];
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
        UpgradeStatBlock stats = Lood.UpgradeManager.Instance.Upgrades[upgradeData.Tag];
      
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
