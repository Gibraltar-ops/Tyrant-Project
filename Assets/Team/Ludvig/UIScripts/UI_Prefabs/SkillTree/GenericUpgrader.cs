
using UnityEngine;

public class GenericUpgrader : Upgrader
{

    private UpgradeTag _upgradeTag;
    private UpgradeDataSO _upgradeData;
    private bool _canBuy = true;
    void Start()
    {
        _upgradeData = GetComponent<UpgradeShop>().upgradeData;
    }

    public override void ApplyUpgrade(bool maxedOut)
    {
        // if (!_canBuy)
        // {
        //     return;
        // }

        // if (maxedOut)
        // {
        //     _canBuy = false;
        // }

        if (!Lood.UpgradeManager.Instance.Upgrades.ContainsKey(_upgradeData.Tag))
        {
            Debug.LogWarning("No such Upgradeable Object key in Upgrade Manager: " + _upgradeTag.ToString());
            return;
        }


        UpgradeStatBlock stats = Lood.UpgradeManager.Instance.Upgrades[_upgradeData.Tag];

        Lood.UpgradeManager.Instance.Upgrades[_upgradeData.Tag].CurrentModifier += stats.AddValue;
        foreach (var item in stats.UpgradeEffects)
        {
            if (item.StatCategory == UpgradeStatCategory.Collection)
            {
                //Upgrade the collection of a specific currency.
                Lood.UpgradeManager.Instance.CurrencySources[item.CurrencySource.Tag].CollectStats[item.CurrencyToAffect].Stats[item.CollectType] += stats.AddValue;
            }
            if (item.StatCategory == UpgradeStatCategory.MainStat)
            {
                //Upgrade the collection of a specific "main stat" (e.g. "Speed" or "Aggression").
                Lood.UpgradeManager.Instance.CurrencySources[item.CurrencySource.Tag].MainStats[item.UpgradeStat] += stats.AddValue;
            }
        }


        //Once everything is finished, calls the onUpgradeApplied event from the base class.
        base.ApplyUpgrade(maxedOut);
    }


}

