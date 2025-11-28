using System.Collections.Generic;
using UnityEngine;

public class HarvesterUpgradeManager : MonoBehaviour
{
  [SerializeField] private List<StatLinkSO> purchasedUpgrades = new List<StatLinkSO>();
    private Dictionary<string, StatNode> stats = new Dictionary<string, StatNode>();

    public void RegisterStat(string key, float initialValue = 0f)
    {
        if (!stats.ContainsKey(key))
            stats[key] = new StatNode(key, initialValue);
    }

    public void BuyUpgrade(StatLinkSO upgrade)
    {
        purchasedUpgrades.Add(upgrade);
        upgrade.LevelUp();
    }

    public void ApplyUpgrades()
    {
        foreach (var upgrade in purchasedUpgrades)
        {
            upgrade.Apply(stats, purchasedUpgrades);
        }
    }

    public float GetStat(string key) => stats.ContainsKey(key) ? stats[key].Value : 0f;
}
