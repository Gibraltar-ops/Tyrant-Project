using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectorShop : MonoBehaviour
{
    public string Tag;
    public Currency CollectedCurrency;
    public Currency BoughtWithCurrency;

    //Labels
    public TMP_Text CurrentCountLabel;
    public TMP_Text NextPriceLabel;
    public Button PurchaseButton;

    //Shop settings and data for the collector
    public ShopSettings shopSettings;
    public CollectorData data;

    public bool Active = false;
    public bool SpawnObject = true;

    private CollectorObjectSpawner _objectSpawner;
    private float _ticksPerSecondCurrent;
    private List<Collector> _collectors;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _objectSpawner = GetComponent<CollectorObjectSpawner>();
        data.CurrentCollect = (int)(data.BaseCollect * data.CollectMultiplier);
        _collectors = new List<Collector>();
        _ticksPerSecondCurrent = data.TicksPerSecondBase * data.TicksPerSecondMultiplier;

        for (int i = 0; i < shopSettings.NumberOwned; i++)
        {
            StartupSpawn();
        }
        Active = true;
        CheckAffordability();
        UpdateLabels();
    }

    public void IncrementPrice()
    {
        shopSettings.NextPrice = (int)(shopSettings.PriceBase * Mathf.Pow(shopSettings.PriceGrowthRate, shopSettings.NumberOwned));
    }



    public void CollectCurrency(Currency collectedCurrency)
    {
        EconomyManager.Instance.IncrementCurrency(collectedCurrency, data.CurrentCollect);
    }

    public void StartupSpawn()
    {
        Collector newCollector = new Collector(0f);
        _collectors.Add(newCollector);

        if (SpawnObject)
        {
            _objectSpawner.SpawnObject(newCollector);
        }

        IncrementPrice();
    }

    public void UpdateLabels()
    {
        CurrentCountLabel.text = shopSettings.NumberOwned.ToString();
        NextPriceLabel.text = shopSettings.NextPrice.ToString();
    }

    public void CheckAffordability()
    {
        if (EconomyManager.Instance.CanAfford(BoughtWithCurrency, shopSettings.NextPrice))
        {
            PurchaseButton.interactable = true;
        }
        else
        {
            PurchaseButton.interactable = false;
        }
    }

    public void SpawnCollector()
    {
        Collector newCollector = new Collector(0f);
        _collectors.Add(newCollector);

        if (SpawnObject)
        {
            _objectSpawner.SpawnObject(newCollector);
        }

        EconomyManager.Instance.IncrementCurrency(BoughtWithCurrency, -shopSettings.NextPrice);
        shopSettings.NumberOwned++;
        IncrementPrice();
        UpdateLabels();
    }
    public float CurrentTicksPerSecond()
    {
        _ticksPerSecondCurrent = data.TicksPerSecondBase * data.TicksPerSecondMultiplier;
        return _ticksPerSecondCurrent;
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
                    CollectCurrency(CollectedCurrency);
                    CheckAffordability();
                }

            }
        }

    }
}
