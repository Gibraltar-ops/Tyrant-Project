using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CollectorObjectSpawner))]
[RequireComponent(typeof(BuyCollector))]
public class Harvester : MonoBehaviour
{

    public HarvesterData Data;

    public bool Active = false;
    public bool SpawnObject = true;

    private CollectorObjectSpawner _objectSpawner;
    private List<Collector> _collectors;

    void Start()
    {
        _objectSpawner = GetComponent<CollectorObjectSpawner>();
        _collectors = new List<Collector>();
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
        float _ticksPerSecondCurrent = Data.TicksPerSecondBase * Data.TicksPerSecondMultiplier;
        return _ticksPerSecondCurrent;
    }
    
    public int ModifiedCollectAmount(CollectorType type)
    {
        float finalCurrency = type.BaseCollect;

        finalCurrency += type.CollectAdd;

        finalCurrency *= type.CollectMultiplier;

        return (int)finalCurrency;
    }

    public void CollectCurrency(Currency collectedCurrency, int amount)
    {
        EconomyManager.Instance.IncrementCurrency(collectedCurrency, amount);
    }

    // Update is called once per frame
    void Update()
    {
          if (Active)
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
}
