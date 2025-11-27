using System.Collections.Generic;
using UnityEngine;

public enum UpgradeType {Increment, Link}
[CreateAssetMenu(menuName = "Upgrade/StatLink")]
public class StatLinkSO : ScriptableObject
{
    public UpgradeType upgradeType;
    public string sourceCurrency;
    public string targetCurrency;
    public float baseMultiplier;
    public float incrementPerLevel;
    public int maxLevel = 10;
    public UpgradeCondition condition;

    private int currentLevel = 0;

    public void LevelUp()
    {
        if (currentLevel < maxLevel)
            currentLevel++;
    }

    public void Apply(Dictionary<string, StatNode> stats, List<StatLinkSO> upgrades)
    {
        
        if (!condition.IsMet(stats, upgrades)) return;
        if (!stats.ContainsKey(sourceCurrency) || !stats.ContainsKey(targetCurrency)) return;

        float sourceValue = stats[sourceCurrency].Value;
        float multiplier = baseMultiplier + (incrementPerLevel * currentLevel);

        stats[targetCurrency].Value += sourceValue * multiplier;
    }
}

public enum ConditionType { None, MinCurrency, HasUpgrade }

[System.Serializable]
public class UpgradeCondition
{
    public ConditionType type;
    public string key;       // e.g. "wood" or another upgrade name
    public float threshold;  // e.g. 100

    public bool IsMet(Dictionary<string, StatNode> stats, List<StatLinkSO> upgrades)
    {
        switch (type)
        {
            case ConditionType.MinCurrency:
                return stats.ContainsKey(key) && stats[key].Value >= threshold;
            case ConditionType.HasUpgrade:
                return upgrades.Exists(u => u.name == key);
            default:
                return true;
        }
    }
}

public class StatNode
{
    public string Key { get; private set; }
    public float Value { get; set; }

    public StatNode(string key, float initialValue = 0f)
    {
        Key = key;
        Value = initialValue;
    }
}


