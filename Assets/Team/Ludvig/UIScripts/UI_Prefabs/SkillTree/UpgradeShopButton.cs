using UnityEngine;
using UnityEngine.UI;
using Lood;
using System.Collections.Generic;
using Unity.VisualScripting;

public class UpgradeShopButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public UpgradeDataSO upgradeData;
    public UpgradeValueTracker Counter;
    public Button _button;
    public bool _maxedOut = false;
    public bool Active = false;
    public List<GameObject> ProgressBars;

    private bool _progressBarsEnabled;



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

        //Assign Button sprites
        _button.GetComponent<Image>().sprite = upgradeData.ColorStyle.SpriteSet.TreeButtonDefaultSprite;

        SpriteState state = _button.spriteState;
        state.disabledSprite = upgradeData.ColorStyle.SpriteSet.TreeButtonInactiveSprite;
        state.highlightedSprite = upgradeData.ColorStyle.SpriteSet.TreeButtonHoverSprite;
        state.pressedSprite = upgradeData.ColorStyle.SpriteSet.TreeButtonPressedSprite;
        _button.spriteState = state;

        ButtonHierarchy buttonHierarchy = GetComponent<ButtonHierarchy>();
        buttonHierarchy.ActiveParentLineColor = upgradeData.ColorStyle.MainColor;
        buttonHierarchy.InactiveParentLineColor = upgradeData.ColorStyle.SecondaryColor;
        buttonHierarchy.Lock.GetComponent<Image>().color = upgradeData.ColorStyle.MainColor;
        buttonHierarchy.AddLines();


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
        EnableProgressBars();
    }


    private void Initialize()
    {

        CheckInteractable(Lood.UpgradeManager.Instance.Upgrades[upgradeData.Tag].Shop.BoughtWithCurrency);

        if (!Active)
        {
            DisableProgressBars();
        }
    }

    private void DisableProgressBars()
    {
        Color ProgressTrackerColor = EconomyManager.Instance.GetCurrencySpriteData(Lood.UpgradeManager.Instance.Upgrades[upgradeData.Tag].Shop.BoughtWithCurrency).OverlayColor;
        foreach (GameObject bar in ProgressBars)
        {
            Image image = bar.GetComponent<Image>();
            if (bar.TryGetComponent(out RadialProgressTracker tracker))
            {
                bar.GetComponent<Image>().color = new Color(ProgressTrackerColor.r, ProgressTrackerColor.g, ProgressTrackerColor.b, 0f);
            }
            else
            {
                bar.GetComponent<Image>().color = new Color(image.color.r, image.color.g, image.color.b, 0f);
            }

        }
        _progressBarsEnabled = false;
    }

    private void EnableProgressBars()
    {
        Color ProgressTrackerColor = EconomyManager.Instance.GetCurrencySpriteData(Lood.UpgradeManager.Instance.Upgrades[upgradeData.Tag].Shop.BoughtWithCurrency).OverlayColor;
        foreach (GameObject bar in ProgressBars)
        {

            Image image = bar.GetComponent<Image>();
            if (bar.TryGetComponent(out RadialProgressTracker tracker))
            {
                bar.GetComponent<Image>().color = new Color(ProgressTrackerColor.r, ProgressTrackerColor.g, ProgressTrackerColor.b, 1f);
            }
            else
            {
                bar.GetComponent<Image>().color = new Color(image.color.r, image.color.g, image.color.b, 1f);
            }

        }
        _progressBarsEnabled = true;
    }

    private void CheckInteractable(Currency currency)
    {
        if (!Active)
        {
            _button.interactable = false;
            return;
        }
        // if (_maxedOut) return;

        if (Lood.UpgradeManager.Instance.Upgrades[upgradeData.Tag].Shop.CurrentOwned == Lood.UpgradeManager.Instance.Upgrades[upgradeData.Tag].Shop.MaxOwned)
        {
            DisableProgressBars();
            _button.interactable = false;
            Lood.UpgradeManager.Instance.Upgrades[upgradeData.Tag].Shop.MaxedOut = true;
            return;
        }

        if (!_progressBarsEnabled) EnableProgressBars();

        Lood.UpgradeManager.Instance.Upgrades[upgradeData.Tag].Shop.MaxedOut = false;

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

    public void Buy()
    {
        AudioManager.Instance.Play("UIClick");
        onUpgradePurchased?.Invoke(upgradeData.Tag);
    }
}
