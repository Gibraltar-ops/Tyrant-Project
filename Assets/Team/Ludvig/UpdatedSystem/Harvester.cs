using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CollectorObjectSpawner))]
[RequireComponent(typeof(BuyCollector))]
public class Harvester : MonoBehaviour
{
    public float TicksPerSecondBase = 1f;
    public HarvesterData Data;
    public bool SpawnObject = true;

    public Vector2 PopupNumberOffset;

    private CollectorObjectSpawner _objectSpawner;
    private List<Collector> _collectors;
    private ValueLinker valueLinker; 

    public delegate void OnCollectionPopup(PopupTextData data);
    public static event OnCollectionPopup onCollectionPopup;

    void Start()
    {
        _objectSpawner = GetComponent<CollectorObjectSpawner>();
        _collectors = new List<Collector>();
        //valueLinker = GetComponent<ValueLinker>();
    }

    public void SpawnCollector()
    {
        Collector newCollector = new Collector(0f);
        _collectors.Add(newCollector);

        if (SpawnObject)
        {
            _objectSpawner.SpawnObject(newCollector);
        }
    
    }

    public float CurrentTicksPerSecond()
    {
        float _ticksPerSecondCurrent = TicksPerSecondBase * Data.TicksPerSecondMultiplier;
        return _ticksPerSecondCurrent;
    }
    
    public int ModifiedCollectAmount(CollectorType type)
    {
        float finalCurrency = type.BaseCollect;

        finalCurrency += type.CollectAdd;

        finalCurrency *= type.CollectMultiplier;

        finalCurrency += finalCurrency * (type.CollectAdditivePercentage/100f);

        finalCurrency += finalCurrency * (type.CollectMultiplicativePercentage/100f);




        return (int)finalCurrency;
    }

    private void CollectCurrency(Currency collectedCurrency, int amount, Collector collector)
    {
        PopupTextData popupData = new PopupTextData(collector.position + PopupNumberOffset, "+" + amount.ToString(), Color.white);
        EconomyManager.Instance.IncrementCurrency(collectedCurrency, amount);
        EconomyManager.Instance.PopUp(popupData);
    }

    // Update is called once per frame
    void Update()
    {
          if (Data.Active)
        {
            foreach (var collector in _collectors)
            {
                if (collector.Collect(Time.deltaTime, CurrentTicksPerSecond()))
                {
                    foreach (var type in Data.CollectorTypes)
                    {   
                        int amount = ModifiedCollectAmount(type);
                        if(amount != 0)
                        {   
                            CollectCurrency(type.CollectedCurrency, amount, collector);
                        }               
                    }

                }

            }
        }
    }

    public void IncrementBaseValue(float amount, HarvesterBaseValue value, Currency currency)
    {
        CollectorType type = new CollectorType();
        foreach (CollectorType item in Data.CollectorTypes)
        {
            if (item.CollectedCurrency == currency)
            {
                type = item;
            }
        }

        switch (value)
        {
            case HarvesterBaseValue.CollectAdd:
                type.CollectAdd += (int)amount;
                break;
            case HarvesterBaseValue.CollectMultiplier:
                type.CollectMultiplier += amount;
                break;
            case HarvesterBaseValue.TicksPerSecondMultiplier:
                Data.TicksPerSecondMultiplier += amount;
                break;
            case HarvesterBaseValue.CollectAdditivePercentage:
                type.CollectAdditivePercentage += amount;
                break;
            case HarvesterBaseValue.CollectMultiplicativePercentage:
                type.CollectMultiplicativePercentage *= 1f + amount;
                break;
            default: break;
        }

        if(TryGetComponent(out Shop shop))
        {
            shop.UpdateTooltip(Upgrade.FormatValueString(ModifiedCollectAmount(type), UpgradeValueType.IntegerRaw));
        }
        
    }

}

public enum HarvesterBaseValue
{
    CollectAdd,
    CollectMultiplier,
    TicksPerSecondMultiplier,
    CollectAdditivePercentage,
    CollectMultiplicativePercentage,
    PerHumanMultiplier
}
