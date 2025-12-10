using UnityEngine;
using Lood;

[RequireComponent(typeof(Lood.Harvester))]
[RequireComponent(typeof(CollectorObjectSpawner))]
[RequireComponent(typeof(BuyCollectorUpgrader))]
public class UpgradeableObjectShop : MonoBehaviour
{
    [HideInInspector]
    public UpgradeableObjectTag UpgradeableObjectTag;
    public UpgradeableObjectDataSO objectDataSO;
    public bool Active = false;
    void OnEnable()
    {   
        UpgradeableObjectShopButton.onCollectorPurchased += TryPurchase;
        EconomyManager.onCurrencyChanged += CheckAffordability;
    }
    void OnDisable()
    {
        UpgradeableObjectShopButton.onCollectorPurchased -= TryPurchase;
        EconomyManager.onCurrencyChanged -= CheckAffordability;
    }

    void Start()
    {
        Lood.UpgradeManager.Instance.AddUpgradeableObject(objectDataSO);
        UpgradeableObjectTag = objectDataSO.Tag;
    } 

    private void TryPurchase(UpgradeableObjectTag tag)
    {
        if(tag != UpgradeableObjectTag)
        {
            return;
        }
        PurchaseUpgrade();
    }

    public void IncrementPrice()
    {
        UpgradeableStatBlock stats = Lood.UpgradeManager.Instance.UpgradeableObjects[UpgradeableObjectTag];
        stats.Shop.NextPrice = (int)(stats.Shop.StartingPrice * Mathf.Pow(stats.Shop.PriceIncrease, stats.Shop.CurrentOwned));
    }

    public void CheckAffordability(Currency currency)
    { 
            UpgradeableStatBlock stats = Lood.UpgradeManager.Instance.UpgradeableObjects[UpgradeableObjectTag];
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
        UpgradeableStatBlock stats = Lood.UpgradeManager.Instance.UpgradeableObjects[UpgradeableObjectTag];
        
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
