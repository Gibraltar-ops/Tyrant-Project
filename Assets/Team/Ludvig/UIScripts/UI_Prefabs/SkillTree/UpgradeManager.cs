using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Lood
{
    public class UpgradeManager : MonoBehaviour
{
    
    public delegate void OnUpgradeManagerInitialized();
    public static event OnUpgradeManagerInitialized onUpgradeManagerInitialized;
    private static Lood.UpgradeManager _instance;
    public static Lood.UpgradeManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<Lood.UpgradeManager>();
                if (_instance == null)
                {
                    GameObject singleton = new GameObject(typeof(Lood.UpgradeManager).ToString());
                    _instance = singleton.AddComponent<Lood.UpgradeManager>();
                    DontDestroyOnLoad(singleton);
                }
            }
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        onUpgradeManagerInitialized?.Invoke();
    }

    public Dictionary<UpgradeableObjectTag, UpgradeableStatBlock> UpgradeableObjects = new();

    public Dictionary<UpgradeTag, UpgradeStatBlock> Upgrades = new();

    public void AddUpgradeableObject(UpgradeableObjectDataSO data)
    {
        if (!UpgradeableObjects.ContainsKey(data.Tag))
            {
                UpgradeableObjects.Add(data.Tag, new UpgradeableStatBlock(data));
                Debug.Log("Upgrade manager added upgradeable object with key: " + data.Tag);
            }
    }

    public void AddUpgrade(UpgradeDataSO data)
    {
        if (!Upgrades.ContainsKey(data.Tag))
            {
                Upgrades.Add(data.Tag, new UpgradeStatBlock(data));
                Debug.Log("Upgrade manager added upgrade with key: " + data.Tag);
            }
    }

    public void IncreaseUpgradeableStat(UpgradeableObjectTag tag, MainStat stat, float value)
    {
        if (!UpgradeableObjects.ContainsKey(tag))
        {
            Debug.LogWarning("Key " + tag.ToString() + " does not exist in upgrade manager.");
            return;
        }

        UpgradeableObjects[tag].MainStats[stat] += value;
    }

    public float GetUpgradeableStat(UpgradeableObjectTag tag, MainStat stat)
    {
         if (!UpgradeableObjects.ContainsKey(tag))
        {
            Debug.LogWarning("Key " + tag.ToString() + " does not exist in upgrade manager.");
            return 0f;
        }
        return UpgradeableObjects[tag].MainStats[stat];
    }

     public void IncrementUpgradeableCount(UpgradeableObjectTag tag)
    {
         if (!UpgradeableObjects.ContainsKey(tag))
        {
            Debug.LogWarning("Key " + tag.ToString() + " does not exist in upgrade manager.");
            return;
        }
        UpgradeableObjects[tag].Shop.CurrentOwned ++;
    }

    public int GetUpgradeableCount(UpgradeableObjectTag tag)
    {
         if (!UpgradeableObjects.ContainsKey(tag))
        {
            Debug.LogWarning("Key " + tag.ToString() + " does not exist in upgrade manager.");
            return 0;
        }
        return UpgradeableObjects[tag].Shop.CurrentOwned;
    }

     public void IncrementUpgradeCount(UpgradeTag tag)
    {
         if (!Upgrades.ContainsKey(tag))
        {
            Debug.LogWarning("Key " + tag.ToString() + " does not exist in upgrade manager.");
            return;
        }
        Upgrades[tag].Shop.CurrentOwned ++;
    }

    public int GetUpgradeCount(UpgradeTag tag)
    {
         if (!Upgrades.ContainsKey(tag))
        {
            Debug.LogWarning("Key " + tag.ToString() + " does not exist in upgrade manager.");
            return 0;
        }
        return Upgrades[tag].Shop.CurrentOwned;
    }

}
}


public enum UpgradeableObjectTag
{
    PlayerTentacle,
    AutoTentacle,
    Gadfly,
    Suicider
}

public enum UpgradeTag
{
    AutoTentacleSpeedUp,
    TentacleEfficiencyUp,
    None

}

public enum MainStat
{
    None,
    //AutoTentacle variables
    BaseSpeed,
    SpeedAdd,
    SpeedMultiply,
    SpeedAdditivePercentage,
    SpeedMultiplicativePercentage,
    //Gadfly variables
    GadflyAggression
}


public class UpgradeableStatBlock
{  
    //This stat block contains all variables related to the shop (prices, count etc.)
    public ShopStats Shop = new();

    //Every currency has its own stat block, so their collection can be upgraded separately. 
    //Call example: 
    //UpgradeableObjects[UpgradeableObjectTag.AutoTentacle].CollectStats[Currency.Biomass].Stats[CollectStat.BaseCollect] += 1f;
    public Dictionary<Currency, CollectTypeStatBlock> CollectStats = new()
    {
        {Currency.Biomass, new CollectTypeStatBlock()},
        {Currency.Brains, new CollectTypeStatBlock()},
        {Currency.Keratin, new CollectTypeStatBlock()}
    };
    
    public Dictionary<MainStat, float> MainStats = new()
    {   
        //Auto Tentacle Stats
        {MainStat.BaseSpeed, 0f},
        {MainStat.SpeedAdd, 0f},
        {MainStat.SpeedMultiply, 1f},
        {MainStat.SpeedAdditivePercentage, 0f},
        {MainStat.SpeedMultiplicativePercentage, 0f},
        
    };

    public UpgradeableStatBlock(UpgradeableObjectDataSO data)
    {   
        //Shop
        Shop.IsActive = data.Shop.IsActive;
        Shop.BoughtWithCurrency = data.Shop.BoughtWithCurrency;
        Shop.CurrentOwned = data.Shop.CurrentOwned;
        Shop.MaxOwned = data.Shop.MaxOwned;
        Shop.StartingPrice = data.Shop.StartingPrice;
        Shop.NextPrice = data.Shop.NextPrice;
        Shop.PriceIncrease = data.Shop.PriceIncrease;
        Shop.CanAfford = data.Shop.CanAfford;
        Shop.Name = data.Shop.Name;
        Shop.Description = data.Shop.Description;
        Shop.Lore = data.Shop.Lore;
        Shop.CurrentModifierString = data.Shop.CurrentModifierString;
        //Collect types
        CollectStats[Currency.Biomass] = new CollectTypeStatBlock(data.BiomassCollect);
        CollectStats[Currency.Brains] = new CollectTypeStatBlock(data.BrainsCollect);
        CollectStats[Currency.Keratin] = new CollectTypeStatBlock(data.KeratinCollect);
       
        //Source stats
        MainStats[MainStat.BaseSpeed] = data.BaseSpeed;
        MainStats[MainStat.SpeedAdd] = data.SpeedAdd;
        MainStats[MainStat.SpeedMultiply] = data.SpeedMultiply;
        MainStats[MainStat.SpeedAdditivePercentage] = data.SpeedAdditivePercentage;
        MainStats[MainStat.SpeedMultiplicativePercentage] = data.SpeedMultiplicativePercentage;
        MainStats[MainStat.GadflyAggression] = data.GadflyAggression;
        
    }
}

# region Collect Stats
public enum CollectStat
{   None,
    BaseCollect,
    CollectMultiplier,
    CollectAdd,
    CollectAdditivePercentage,
    CollectMultiplicativePercentage,
}
public class CollectTypeStatBlock
{   
    public Dictionary<CollectStat, float> Stats = new()
    {   
        //Auto Tentacle Stats
        {CollectStat.BaseCollect, 0f },
        {CollectStat.CollectMultiplier, 1f },
        {CollectStat.CollectAdd, 0f },
        {CollectStat.CollectAdditivePercentage, 0f},
        {CollectStat.CollectMultiplicativePercentage, 0f},

    };
    public CollectTypeStatBlock()
    {
        
    }
    public CollectTypeStatBlock(CollectType stats)
    {
        Stats[CollectStat.BaseCollect] = stats.BaseCollect;
        Stats[CollectStat.CollectMultiplier] = stats.CollectMultiplier;
        Stats[CollectStat.CollectAdd] = stats.CollectAdd;
        Stats[CollectStat.CollectAdditivePercentage] = stats.CollectAdditivePercentage;
        Stats[CollectStat.CollectMultiplicativePercentage] = stats.CollectMultiplicativePercentage;
    }
}

[Serializable]
public class CollectType
{
    public float BaseCollect = 0f;
    public float CollectMultiplier = 1f;
    public float CollectAdd = 0f;
    public float CollectAdditivePercentage = 0f;
    public float CollectMultiplicativePercentage = 0f;
}
#endregion

public class UpgradeStatBlock
{  
    public ShopStats Shop = new();

    public List<UpgradeEffect> UpgradeEffects;
    public float AddValue = 0.1f;
    public float CurrentModifier = 0;

    public UpgradeStatBlock(UpgradeDataSO data)
    {
        Shop.IsActive = data.Shop.IsActive;
        Shop.BoughtWithCurrency = data.Shop.BoughtWithCurrency;
        Shop.CurrentOwned = data.Shop.CurrentOwned;
        Shop.MaxOwned = data.Shop.MaxOwned;
        Shop.StartingPrice = data.Shop.StartingPrice;
        Shop.NextPrice = data.Shop.NextPrice;
        Shop.PriceIncrease = data.Shop.PriceIncrease;
        Shop.CanAfford = data.Shop.CanAfford;
        Shop.Name = data.Shop.Name;
        Shop.Description = data.Shop.Description;
        Shop.Lore = data.Shop.Lore;
        Shop.CurrentModifierString = data.Shop.CurrentModifierString;
        Shop.MaxedOut = data.Shop.MaxedOut;

        UpgradeEffects = data.UpgradeEffects;
        AddValue = data.AddValue;
        CurrentModifier = data.CurrentModifier;
       
    }

}

[Serializable]
public class UpgradeEffect
{
    public UpgradeableObjectTag UpgradeableObjectTag;
    public UpgradeStatCategory StatCategory;

    [Header("Collection Upgrade")]
    public Currency CurrencyToAffect;
    public CollectStat CollectType;

    [Header("Stat Upgrade")]
    public MainStat UpgradeStat;
}

public enum UpgradeStatCategory
{
    Collection,
    MainStat,
}

[Serializable]
public class ShopStats
{   
    [Header("Tooltips")]
    public string Name;
    [TextArea(3, 10)]
    public string Description;
    [TextArea(3, 10)]
    public string EffectString;
    [TextArea(3, 10)]
    public string Lore;
    public string CurrentModifierString;

    [Header("Shop Stats")]
    public bool IsActive = true;
    public Currency BoughtWithCurrency;
    public Currency CollectsCurrency;
    public int CurrentOwned = 0;
    public int MaxOwned = 25;
    public long StartingPrice = 25;
    public long NextPrice = 25;
    public float PriceIncrease = 1.2f;
    public bool CanAfford = false;
    public bool MaxedOut = false;
  
}