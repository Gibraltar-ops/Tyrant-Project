using UnityEngine;
using Lood;

[RequireComponent(typeof(GenericUpgrader))]

public class UpgradeShop : MonoBehaviour
{

    [HideInInspector]
    public string UpgradeTag;
    public UpgradeDataSO upgradeData;
    public bool Active = false;
    public int Level1Max = 3;
    public int Level2Max = 5;

    public int Level3Max = 7;
    void OnEnable()
    {
        UpgradeShopButton.onUpgradePurchased += TryPurchase;
        EconomyManager.onCurrencyChanged += CheckAffordability;
        ProgressEventTrigger.onProgressEvent += ProgressEventHandler;
    }
    void OnDisable()
    {
        UpgradeShopButton.onUpgradePurchased -= TryPurchase;
        EconomyManager.onCurrencyChanged -= CheckAffordability;
        ProgressEventTrigger.onProgressEvent -= ProgressEventHandler;
    }
    void Awake()
    {
        upgradeData.Shop.MaxOwned = Level1Max;
        Lood.UpgradeManager.Instance.AddUpgrade(upgradeData);
        UpgradeTag = upgradeData.Tag;
    }

    public void ProgressEventHandler(ProgressEvent progressEvent)
    {
        switch (progressEvent)
        {
            case ProgressEvent.SetStage2:
                Lood.UpgradeManager.Instance.Upgrades[upgradeData.Tag].Shop.MaxOwned = Level2Max;
                break;
            case ProgressEvent.SetStage3:
                Lood.UpgradeManager.Instance.Upgrades[upgradeData.Tag].Shop.MaxOwned = Level3Max;
                break;
            default: break;
        }
    }

    private void TryPurchase(string tag)
    {
        if (tag != upgradeData.Tag)
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


        // stats.Shop.MaxedOut = false;
        stats.Shop.CurrentOwned++;
        EconomyManager.Instance.IncrementCurrency(stats.Shop.BoughtWithCurrency, -stats.Shop.NextPrice);
        IncrementPrice();

        // if (stats.Shop.CurrentOwned < stats.Shop.MaxOwned)
        // {
        //     EconomyManager.Instance.IncrementCurrency(stats.Shop.BoughtWithCurrency, -stats.Shop.NextPrice);
        //     IncrementPrice();
        //     stats.Shop.MaxedOut = false;
        //     stats.Shop.CurrentOwned++;
        // }

        // if (stats.Shop.CurrentOwned == stats.Shop.MaxOwned)
        // {
        //     EconomyManager.Instance.IncrementCurrency(stats.Shop.BoughtWithCurrency, -stats.Shop.NextPrice);
        //     IncrementPrice();
        //     //stats.Shop.CurrentOwned++;
        //     stats.Shop.MaxedOut = true;
        // }


        Upgrader[] upgrades = GetComponents<Upgrader>();
        foreach (Upgrader upgrade in upgrades)
        {
            upgrade.ApplyUpgrade(stats.Shop.MaxedOut);
        }



    }



}
