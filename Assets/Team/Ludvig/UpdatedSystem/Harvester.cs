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

    private CollectorObjectSpawner _objectSpawner;
    private List<Collector> _collectors;
    private ValueLinker valueLinker; 


    void Start()
    {
        _objectSpawner = GetComponent<CollectorObjectSpawner>();
        _collectors = new List<Collector>();
        valueLinker = GetComponent<ValueLinker>();
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

        finalCurrency += EconomyManager.Instance.Victims * type.Upgrade_perhumanmultiplier;

        return (int)finalCurrency;
    }

    private void CollectCurrency(Currency collectedCurrency, int amount)
    {
        EconomyManager.Instance.IncrementCurrency(collectedCurrency, amount);
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
                        CollectCurrency(type.CollectedCurrency, ModifiedCollectAmount(type));
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
            default: break;
        }
    }

}

public enum HarvesterBaseValue
{
    CollectAdd,
    CollectMultiplier,
    TicksPerSecondMultiplier,
    PerHumanMultiplier
}
