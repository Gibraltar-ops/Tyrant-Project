using UnityEngine;
using Lood;

[RequireComponent(typeof(GenericUpgrader))]

public class UpgradeShop : MonoBehaviour
{

    public UpgradeTag UpgradeTag;
    public UpgradeDataSO upgradeDataSO;
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
        Lood.UpgradeManager.Instance.AddUpgrade(upgradeDataSO);
        UpgradeTag = upgradeDataSO.Tag;
    }

    private void TryPurchase(UpgradeTag tag)
    {
        if(tag != UpgradeTag)
        {
            return;
        }
        PurchaseUpgrade();
    }

    public void IncrementPrice()
    {
        UpgradeStatBlock stats = Lood.UpgradeManager.Instance.Upgrades[UpgradeTag];
        stats.Shop.NextPrice = (int)(stats.Shop.StartingPrice * Mathf.Pow(stats.Shop.PriceIncrease, stats.Shop.CurrentOwned));
    }

    public void CheckAffordability(Currency currency)
    { 
            UpgradeStatBlock stats = Lood.UpgradeManager.Instance.Upgrades[UpgradeTag];
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
        UpgradeStatBlock stats = Lood.UpgradeManager.Instance.Upgrades[UpgradeTag];
      
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
