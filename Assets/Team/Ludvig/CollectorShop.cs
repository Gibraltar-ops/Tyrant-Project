using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectorShop : MonoBehaviour
{
    public string Tag;
    public Currency BoughtWithCurrency;
    public float TicksPerSecondBase = 1f;
    public float TicksPerSecondMultiplier = 1f;
    //Labels
    public TMP_Text CurrentCountLabel;
    public TMP_Text NextPriceLabel;
    public Button PurchaseButton;

    //Shop settings and data for the collector
    public ShopSettings shopSettings;

    public List<CollectorData> Data;

    public bool Active = false;
    public bool SpawnObject = true;

    private CollectorObjectSpawner _objectSpawner;
    private float _ticksPerSecondCurrent;
    private List<Collector> _collectors;

    void OnEnable()
    {
        EconomyManager.onCurrencyChanged += CheckAffordability;
    }
    void OnDisable()
    {
        EconomyManager.onCurrencyChanged -= CheckAffordability;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PurchaseButton.onClick.AddListener(SpawnCollector);

        _objectSpawner = GetComponent<CollectorObjectSpawner>();

        _collectors = new List<Collector>();
        _ticksPerSecondCurrent = TicksPerSecondBase * TicksPerSecondMultiplier;

        for (int i = 0; i < shopSettings.NumberOwned; i++)
        {
            StartupSpawn();
        }
        Active = true;
        CheckAffordability(BoughtWithCurrency);
        UpdateLabels();
    }

    public void IncrementPrice()
    {
        shopSettings.NextPrice = (int)(shopSettings.PriceBase * Mathf.Pow(shopSettings.PriceGrowthRate, shopSettings.NumberOwned));
    }

    public void CollectCurrency(Currency collectedCurrency, int amount)
    {
        EconomyManager.Instance.IncrementCurrency(collectedCurrency, amount);
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

    public void CheckAffordability(Currency currency)
    {
        if (currency == BoughtWithCurrency)
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
        _ticksPerSecondCurrent = TicksPerSecondBase * TicksPerSecondMultiplier;
        return _ticksPerSecondCurrent;
    }
    
    public int ModifiedCollectAmount(CollectorData data)
    {
        float finalCurrency = data.BaseCollect;

        finalCurrency += data.CollectAdd;

        finalCurrency *= data.CollectMultiplier;

        return (int)finalCurrency;
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
                    foreach (var data in Data)
                    {
                        CollectCurrency(data.CollectedCurrency, ModifiedCollectAmount(data));
                    }

                }

            }
        }

    }
}
