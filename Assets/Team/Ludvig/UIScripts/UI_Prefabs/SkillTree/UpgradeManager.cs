using System;
using System.Collections.Generic;

using UnityEngine;

// The upgrade system works with "Currency Sources" which hold collection values and stats 
//and "Upgrades" which hold upgrade values.

//A currency source can be anything that needs to be upgraded: e.g a source of healing for the hp system,
//a source of damage for enemies etc. It's just a universal object that can hold different values.
//The upgrade manager makes these values globally
//accessible through a tag system.
//___________________________________
//TO ADD A NEW Currency Source MAIN STAT (EG. "SPEED", "AGRESSION" AND SO ON):
//LOOK FOR "01." IN THE COMMENTS BELOW, AND FOLLOW THE STEPS.

//TO ADD A NEW Currency Source COLLECT STAT (EG. "MULTIPLERS" AND SO ON):
//LOOK FOR "001." IN THE COMMENTS BELOW, AND FOLLOW THE STEPS.
//
//_____________v___v___v_____________

//TO ADD A NEW CURRENCY SOURCE OBJECT.
//Create a new "CurrencySourceSO" in the editor and tweak the values.
//Make sure the scriptable object's "Tag" field reflects what you're building. 
//This tag must be unique per CurrencySourceSO.
//LOOK FOR "1." IN THE COMMENTS BELOW AND ADD A NEW TAG THERE.

//If your new object needs any new stats - follow the checklist below (from 01. to 04.) to add this value.

//Create an empty game object and add an "UpgradeableObjectShop.cs" to it.
//The shop takes care of things like incrementing count and price, max count.
//Assign your new CurrencySourceSO to the field in the shop script.
//Create your upgrader script and put it on the same object. Make sure it inherits from "Upgrader.cs".
//Use BuyCollectorUpgrader.cs as a reference.
//Override the ApplyUpgrade method. Make sure to run "base.ApplyUpgrade(maxedOut)" at the end of your custom code;
//This should be enough.

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

        public Dictionary<string, CurrencySourceStatBlock> CurrencySources = new();

        public Dictionary<string, UpgradeStatBlock> Upgrades = new();

        public void AddUpgradeableObject(CurrencySourceSO data)
        {
            if (!CurrencySources.ContainsKey(data.Tag))
            {
                CurrencySources.Add(data.Tag, new CurrencySourceStatBlock(data));
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

        public void IncreaseUpgradeableStat(string tag, MainStat stat, float value)
        {
            if (!CurrencySources.ContainsKey(tag))
            {
                Debug.LogWarning("Key " + tag.ToString() + " does not exist in upgrade manager.");
                return;
            }

            CurrencySources[tag].MainStats[stat] += value;
        }

        public float GetCurrencySourceStat(string tag, MainStat stat)
        {
            if (!CurrencySources.ContainsKey(tag))
            {
                Debug.LogWarning("Key " + tag.ToString() + " does not exist in upgrade manager.");
                return 0f;
            }
            return CurrencySources[tag].MainStats[stat];
        }

        public void IncrementCurrencySourceCount(string tag)
        {
            if (!CurrencySources.ContainsKey(tag))
            {
                Debug.LogWarning("Key " + tag.ToString() + " does not exist in upgrade manager.");
                return;
            }
            CurrencySources[tag].Shop.CurrentOwned++;
        }

        public int GetCurrencySourceCount(string tag)
        {
            if (!CurrencySources.ContainsKey(tag))
            {
                Debug.LogWarning("Key " + tag.ToString() + " does not exist in upgrade manager.");
                return 0;
            }
            return CurrencySources[tag].Shop.CurrentOwned;
        }

        public void IncrementUpgradeCount(string tag)
        {
            if (!Upgrades.ContainsKey(tag))
            {
                Debug.LogWarning("Key " + tag.ToString() + " does not exist in upgrade manager.");
                return;
            }
            Upgrades[tag].Shop.CurrentOwned++;
        }

        public int GetUpgradeCount(string tag)
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


//1.___________________________________
//WHEN CREATING A NEW SOURCE OF CURRENCY (AN "UPGRADEABLE OBJECT" IN THE LANGUAGE OF THIS CODE)
//ADD NEW VARIABLE TO THIS ENUM!!!
//_____________v___v___v_____________
public enum CurrencySourceTag
{
    AutoTentacle,
    Gadfly,
    Suicider
}

//2.___________________________________
//WHEN CREATING A NEW VALUE UPGRADE (AN "UPGRADE" IN THE LANGUAGE OF THIS CODE)
//ADD NEW VARIABLE TO THIS ENUM!!!
//_____________v___v___v_____________
public enum UpgradeTag
{
    AutoTentacleSpeedUp,
    TentacleEfficiencyUp,
    None

}

//01.
//WHEN ADDING A NEW UPGRADEABLE STAT
//___________________________________
//ADD NEW VARIABLE TO THIS ENUM!!!
//_____________v___v___v_____________
public enum MainStat
{
    None,
    //Speed variables
    BaseSpeed,
    SpeedAdd,
    SpeedMultiply,
    SpeedAdditivePercentage,
    SpeedMultiplicativePercentage,
    //Gadfly variables
    GadflyCooldown,
    GadflyHarvestDuration
}


public class CurrencySourceStatBlock
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
        {Currency.Keratin, new CollectTypeStatBlock()},
        {Currency.HealthPoints, new CollectTypeStatBlock()}
    };

    //02.___________________________________
    //ADD NEW ENTRY TO THIS DICTIONARY!!!
    //_____________v___v___v_____________
    public Dictionary<MainStat, float> MainStats = new()
    {   
        //Auto Tentacle Stats
        {MainStat.BaseSpeed, 0f},
        {MainStat.SpeedAdd, 0f},
        {MainStat.SpeedMultiply, 1f},
        {MainStat.SpeedAdditivePercentage, 0f},
        {MainStat.SpeedMultiplicativePercentage, 0f},
        {MainStat.GadflyCooldown, 10f},
        {MainStat.GadflyCooldown, 6f},


    };
    //03._________________________________________________________________
    //The constructor needs an "UpgradeableObjectDataSO" scriptable object as input.
    //ADD YOUR NEW VARIABLE TO THE "UpgradeableObjectDataSO.cs" script. 
    //________________________________________________________
    public CurrencySourceStatBlock(CurrencySourceSO data)
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
        CollectStats[Currency.HealthPoints] = new CollectTypeStatBlock(data.HealthPointsCollect);

        //04._________________________________________________
        //ASSIGN THE VALUE FROM THE INPUT SCRIPTABLE OBJECT THAT YOU ADDED IN STEP 03.!!!
        //_____________v___v___v_____________
        //Source stats
        MainStats[MainStat.BaseSpeed] = data.BaseSpeed;
        MainStats[MainStat.SpeedAdd] = data.SpeedAdd;
        MainStats[MainStat.SpeedMultiply] = data.SpeedMultiply;
        MainStats[MainStat.SpeedAdditivePercentage] = data.SpeedAdditivePercentage;
        MainStats[MainStat.SpeedMultiplicativePercentage] = data.SpeedMultiplicativePercentage;
        MainStats[MainStat.GadflyCooldown] = data.GadflyCooldown;
        MainStats[MainStat.GadflyHarvestDuration] = data.GadflyHarvestDuration;

    }
}

//_________________________________________________
//HOW TO ADD COLLECT STATS.!!!
//DON'T ADD NEW COLLECT STATS UNLESS YOU NEED A COMPLETELY NEW BEHAVIOR FOR COLLECTION,
//TRY USING THE EXISTING VALUES:

//HERE IS AN EXAMPLE OF USE FROM THE HARVESTER SCRIPT:
//
// public int GetCurrentCollectAmount(Currency currency, UpgradeableObjectTag tag)
// {   
//     //
//     UpgradeableStatBlock data = UpgradeManager.Instance.UpgradeableObjects[tag];
//     //Apply base speed (in ticks per second)
//     float finalCollectAmount = data.CollectStats[currency].Stats[CollectStat.BaseCollect];
//     //Apply simple add
//     finalCollectAmount += data.CollectStats[currency].Stats[CollectStat.CollectAdd];
//     //Apply simple multiply
//     finalCollectAmount *= data.CollectStats[currency].Stats[CollectStat.CollectMultiplier];
//     //Apply additive percentage
//     finalCollectAmount += finalCollectAmount * data.CollectStats[currency].Stats[CollectStat.CollectAdditivePercentage] / 100;
//     //Apply multiplicative percentage
//     finalCollectAmount += finalCollectAmount * data.CollectStats[currency].Stats[CollectStat.CollectMultiplicativePercentage] / 100;

//     return (int)finalCollectAmount;

// }
//_____________v___v___v_____________

//001.________________________________
//::::TO ADD NEW COLLECT VARIABLE::::
//ADD NEW VARIABLE TO THIS ENUM.!!!
//_____________v___v___v_____________
#region Collect Stats
public enum CollectStat
{
    None,
    BaseCollect,
    CollectMultiplier,
    CollectAdd,
    CollectAdditivePercentage,
    CollectMultiplicativePercentage,
    CollectTotalCurrent
}
public class CollectTypeStatBlock
{
    //002.________________________________
    //TO ADD NEW COLLECT VARIABLE
    //ADD NEW ENTRY TO THIS DICTIONARY.!!!
    //_____________v___v___v_____________
    public Dictionary<CollectStat, float> Stats = new()
    {   
        //Auto Tentacle Stats
        {CollectStat.BaseCollect, 0f },
        {CollectStat.CollectMultiplier, 1f },
        {CollectStat.CollectAdd, 0f },
        {CollectStat.CollectAdditivePercentage, 0f},
        {CollectStat.CollectMultiplicativePercentage, 0f},
        {CollectStat.CollectTotalCurrent, 0f},

    };
    public CollectTypeStatBlock()
    {

    }
    //003.________________________________
    //TO ADD NEW COLLECT VARIABLE
    //ADD NEW ASSIGNMENT TO THIS CONSTRUCTOR.!!!
    //_____________v___v___v_____________
    public CollectTypeStatBlock(CollectType stats)
    {
        Stats[CollectStat.BaseCollect] = stats.BaseCollect;
        Stats[CollectStat.CollectMultiplier] = stats.CollectMultiplier;
        Stats[CollectStat.CollectAdd] = stats.CollectAdd;
        Stats[CollectStat.CollectAdditivePercentage] = stats.CollectAdditivePercentage;
        Stats[CollectStat.CollectMultiplicativePercentage] = stats.CollectMultiplicativePercentage;
        Stats[CollectStat.CollectTotalCurrent] = stats.BaseCollect;
    }
}

//004.________________________________
//FINAL STEP
//TO ADD NEW COLLECT VARIABLE
//ADD NEW VARIABLE TO THIS CLASS!!!
//_____________v___v___v_____________
[Serializable]
public class CollectType
{
    public float BaseCollect = 0f;
    public float CollectMultiplier = 1f;
    public float CollectAdd = 0f;
    public float CollectAdditivePercentage = 0f;
    public float CollectMultiplicativePercentage = 0f;
    public float CollectTotalCurrent = 0f;
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
    public CurrencySourceSO CurrencySource;
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