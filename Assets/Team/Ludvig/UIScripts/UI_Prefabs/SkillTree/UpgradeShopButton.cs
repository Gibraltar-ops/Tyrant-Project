using UnityEngine;
using UnityEngine.UI;
using Lood;
using System.Collections.Generic;

public class UpgradeShopButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public UpgradeDataSO upgradeData;
    public UpgradeValueTracker Counter;
    public Button _button;
    public bool _maxedOut = false;
    public bool Active = false;
    public List<GameObject> ProgressBars;

    public delegate void OnUpgradePurchased(string tag);
    public static event OnUpgradePurchased onUpgradePurchased;

    void OnEnable()
    {
        EconomyManager.onCurrencyChanged += CheckInteractable;

    }
    void OnDisable()
    {
        EconomyManager.onCurrencyChanged -= CheckInteractable;
    }

    void Awake()
    {
        Counter.upgradeData = upgradeData;
        //GetComponentInChildren<UpgradeValueTracker>().upgradeData = upgradeData;
    }
    void Start()
    {
        _button = GetComponentInChildren<Button>();
        _button.onClick.AddListener(Buy);
        Invoke("Initialize", 0.01f);

    }
    public void Activate()
    {
        Active = true;
        CheckInteractable(Lood.UpgradeManager.Instance.Upgrades[upgradeData.Tag].Shop.BoughtWithCurrency);
        foreach (GameObject bar in ProgressBars)
        {
            Image image = bar.GetComponent<Image>();
            bar.GetComponent<Image>().color = new Color(image.color.r, image.color.g, image.color.b, 1f);
        }
    }


    private void Initialize()
    {

        CheckInteractable(Lood.UpgradeManager.Instance.Upgrades[upgradeData.Tag].Shop.BoughtWithCurrency);

        if (!Active)
        {
            foreach (GameObject bar in ProgressBars)
            {
                Image image = bar.GetComponent<Image>();
                bar.GetComponent<Image>().color = new Color(image.color.r, image.color.g, image.color.b, 0f);
            }
        }
    }

    private void CheckInteractable(Currency currency)
    {
        if (!Active)
        {
            _button.interactable = false;
            return;
        }
        if (_maxedOut) return;

        if (Lood.UpgradeManager.Instance.Upgrades[upgradeData.Tag].Shop.MaxedOut)
        {
            _button.interactable = false;
            _maxedOut = true;
            return;
        }

        if (currency != Lood.UpgradeManager.Instance.Upgrades[upgradeData.Tag].Shop.BoughtWithCurrency)
        {
            return;
        }
        if (Lood.UpgradeManager.Instance.Upgrades[upgradeData.Tag].Shop.MaxedOut)
        {
            return;
        }

        if (EconomyManager.Instance.CanAfford(currency, (int)Lood.UpgradeManager.Instance.Upgrades[upgradeData.Tag].Shop.NextPrice))
        {
            _button.interactable = true;
        }
        else
        {
            _button.interactable = false;
        }
    }

    private void Buy()
    {
        onUpgradePurchased?.Invoke(upgradeData.Tag);
    }
}
