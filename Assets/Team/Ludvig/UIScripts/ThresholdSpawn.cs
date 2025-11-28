using System;
using UnityEngine;

public class ThresholdSpawn : MonoBehaviour
{
    public bool HasSpawned = false;
    public long Threshold;
    private Currency ThresholdCurrency;

    void OnEnable()
    {
        EconomyManager.onCurrencyChanged += CheckThreshold;
    }
    void OnDisable()
    {
        EconomyManager.onCurrencyChanged += CheckThreshold;
    }

    void Start()
    {
        ThresholdCurrency = GetComponent<Shop>().BoughtWithCurrency;
        if(Threshold == 0)
        {
            HasSpawned = true;
            //EconomyManager.onCurrencyChanged -= CheckThreshold;
            Invoke("SpawnUI", 0.2f);
            
        }
    }

    public void CheckThreshold(Currency currency)
    {
        if (HasSpawned)
        {
            return;
        }

        if(currency == ThresholdCurrency)
        {
            if(EconomyManager.Instance.GetCurrent(ThresholdCurrency) >= Threshold)
            {
                SpawnUI();
                //EconomyManager.onCurrencyChanged -= CheckThreshold;
                HasSpawned = true;
            }
            
        }
        else
        {
            return;
        }
    }

    public void SpawnUI()
    {
        GetComponent<Shop>().CreateStoreItem();
    }

}
