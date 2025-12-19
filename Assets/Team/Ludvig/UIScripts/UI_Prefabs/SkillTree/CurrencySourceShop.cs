using UnityEngine;
using Lood;
using System.Collections.Generic;

public class CurrencySourceShop : MonoBehaviour
{

    public CurrencySourceSO CurrencySourceData;

    public int Level1Max = 3;
    public int Level2Max = 5;

    public int Level3Max = 7;

    public bool Active = false;
    void OnEnable()
    {
        CurrencySourceShopButton.onCollectorPurchased += TryPurchase;
        EconomyManager.onCurrencyChanged += CheckAffordability;
        ProgressEventTrigger.onProgressEvent += ProgressEventHandler;
    }
    void OnDisable()
    {
        CurrencySourceShopButton.onCollectorPurchased -= TryPurchase;
        EconomyManager.onCurrencyChanged -= CheckAffordability;
    }

    void Awake()
    {
        CurrencySourceData.Shop.MaxOwned = Level1Max;
        Lood.UpgradeManager.Instance.AddUpgradeableObject(CurrencySourceData);
    }

    private void TryPurchase(string tag)
    {
        if (tag != CurrencySourceData.Tag)
        {
            return;
        }
        PurchaseUpgrade();
    }
    public void ProgressEventHandler(ProgressEvent progressEvent)
    {
        switch (progressEvent)
        {
            case ProgressEvent.SetStage2:
                Lood.UpgradeManager.Instance.CurrencySources[CurrencySourceData.Tag].Shop.MaxOwned = Level2Max;
                break;
            case ProgressEvent.SetStage3:
                Lood.UpgradeManager.Instance.CurrencySources[CurrencySourceData.Tag].Shop.MaxOwned = Level3Max;
                break;
            default: break;
        }
    }

    public void IncrementPrice()
    {
        CurrencySourceStatBlock stats = Lood.UpgradeManager.Instance.CurrencySources[CurrencySourceData.Tag];
        stats.Shop.NextPrice = (int)(stats.Shop.StartingPrice * Mathf.Pow(stats.Shop.PriceIncrease, stats.Shop.CurrentOwned));
    }

    public void CheckAffordability(Currency currency)
    {
        CurrencySourceStatBlock stats = Lood.UpgradeManager.Instance.CurrencySources[CurrencySourceData.Tag];
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
        CurrencySourceStatBlock stats = Lood.UpgradeManager.Instance.CurrencySources[CurrencySourceData.Tag];

        stats.Shop.MaxedOut = false;
        stats.Shop.CurrentOwned++;
        EconomyManager.Instance.IncrementCurrency(stats.Shop.BoughtWithCurrency, -stats.Shop.NextPrice);
        IncrementPrice();
        // IncrementPrice();
        // if (stats.Shop.CurrentOwned < stats.Shop.MaxOwned)
        // {
        //     stats.Shop.MaxedOut = false;
        //     stats.Shop.CurrentOwned++;
        //     EconomyManager.Instance.IncrementCurrency(stats.Shop.BoughtWithCurrency, -stats.Shop.NextPrice);
        //     IncrementPrice();

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
