using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIUpgradeTooltip : MonoBehaviour
{
    [HideInInspector]
    public PurchaseableSO Data;

    [HideInInspector]
    public UpgradeShopButton shopButton;

    [HideInInspector]
    public ButtonHierarchy buttonHierarchy;

    public List<GameObject> HideOnMaxedOut;
    public List<GameObject> ShowOnMaxedOut;

    public TMP_Text NextPriceLabel;
    public TMP_Text LevelLabel;
    public UnityEngine.UI.Image BoughtWithCurrencyIcon;
    public TMP_Text NameLabel;
    public TMP_Text DescriptionLabel;

    public TMP_Text EffectHeaderLabel;
    public TMP_Text CurrentEffectLabel;
    public TMP_Text NextEffectLabel;
    public TMP_Text LoreLabel;
    public RectTransform ProgressSlider;
    public Button UpgradeButton;

    private Image[] _colorizableImages;

    void Awake()
    {
        UpgradeButton.onClick.AddListener(Buy);
        CollectColorizableImages();
        gameObject.SetActive(false);
        Lood.UpgradeManager.Instance.UpgradeTooltipPanel = gameObject;
    }
    void OnEnable()
    {

        UpdateFields();
        SetTheme();
        //RebuildLayout();
        Upgrader.onUpgradeApplied += UpdateFields;
        EconomyManager.onCurrencyChanged += UpdateOnCurrencyChange;


    }
    void OnDisable()
    {
        Upgrader.onUpgradeApplied -= UpdateFields;
        EconomyManager.onCurrencyChanged -= UpdateOnCurrencyChange;
    }

    private void CollectColorizableImages()
    {
        _colorizableImages = GetComponentsInChildren<Image>(true);
        List<Image> filteredImageList = new List<Image>();
        foreach (var item in _colorizableImages)
        {
            if (item.TryGetComponent(out Button button))
            {
                continue;
            }
            filteredImageList.Add(item);
        }
        _colorizableImages = filteredImageList.ToArray();
    }

    private void SetTheme()
    {
        UpgradeDataSO upgrade = Data as UpgradeDataSO;
        foreach (Image image in _colorizableImages)
        {

            image.color = upgrade.ColorStyle.MainColor;
        }

        Button _button = GetComponentInChildren<Button>(true);

        //Assign Button sprites
        _button.GetComponent<Image>().sprite = upgrade.ColorStyle.SpriteSet.TooltipButtonDefaultSprite;

        SpriteState state = _button.spriteState;
        state.disabledSprite = upgrade.ColorStyle.SpriteSet.TooltipButtonInactiveSprite;
        state.highlightedSprite = upgrade.ColorStyle.SpriteSet.TooltipButtonHoverSprite;
        state.pressedSprite = upgrade.ColorStyle.SpriteSet.TooltipButtonPressedSprite;
        _button.spriteState = state;

        UpgradeStatBlock stats = Lood.UpgradeManager.Instance.Upgrades[Data.Tag];
        CurrencySpriteData spriteData = EconomyManager.Instance.GetCurrencySpriteData(stats.Shop.BoughtWithCurrency);
        BoughtWithCurrencyIcon.sprite = spriteData.Sprite;
        BoughtWithCurrencyIcon.color = spriteData.OverlayColor;
        ProgressSlider.GetComponent<Image>().color = spriteData.OverlayColor;

    }

    public void UpdateOnCurrencyChange(Currency currency)
    {
        if (currency != Lood.UpgradeManager.Instance.Upgrades[Data.Tag].Shop.BoughtWithCurrency) return;

        UpgradeButton.interactable = shopButton._button.interactable;

        StopAllCoroutines();
        float previousNormalizedScale = ProgressSlider.localScale.x;

        long currentCurrency = EconomyManager.Instance.GetCurrent(Lood.UpgradeManager.Instance.Upgrades[Data.Tag].Shop.BoughtWithCurrency);

        float normalizedProgressBarScale;

        if (currentCurrency >= 1)
        {
            normalizedProgressBarScale = Mathf.Clamp((float)currentCurrency / Lood.UpgradeManager.Instance.Upgrades[Data.Tag].Shop.NextPrice, 0f, 1f);
        }
        else
        {
            normalizedProgressBarScale = 0f;
        }
        StartCoroutine(LerpPos(previousNormalizedScale, normalizedProgressBarScale, 0.2f, ProgressSlider));
        //ProgressSlider.localScale = new Vector3(normalizedProgressBarScale, 1f, 1f);
    }

    public void UpdateFields()
    {
        if (Lood.UpgradeManager.Instance.Upgrades[Data.Tag].Shop.MaxedOut)
        {
            foreach (GameObject item in HideOnMaxedOut)
            {
                item.SetActive(false);
            }
            foreach (GameObject item in ShowOnMaxedOut)
            {
                item.SetActive(true);
            }

        }
        else
        {
            foreach (GameObject item in HideOnMaxedOut)
            {
                item.SetActive(true);
            }
            foreach (GameObject item in ShowOnMaxedOut)
            {
                item.SetActive(false);
            }
        }
        UpgradeButton.interactable = shopButton._button.interactable;
        NameLabel.text = Data.Shop.Name;
        DescriptionLabel.text = Data.Shop.Description;
        EffectHeaderLabel.text = Data.Shop.EffectString;
        LoreLabel.text = Data.Shop.Lore;
        ValueFormatType formatAs = Lood.UpgradeManager.Instance.Upgrades[Data.Tag].FormatAs;
        LevelLabel.text = FormatNumbers.FormatValueString(Lood.UpgradeManager.Instance.Upgrades[Data.Tag].Shop.CurrentOwned, ValueFormatType.IntegerRaw);

        CurrentEffectLabel.text = FormatNumbers.FormatValueString(Lood.UpgradeManager.Instance.Upgrades[Data.Tag].CurrentModifier, formatAs);
        NextEffectLabel.text = FormatNumbers.FormatValueString(Lood.UpgradeManager.Instance.Upgrades[Data.Tag].CurrentModifier + Lood.UpgradeManager.Instance.Upgrades[Data.Tag].AddValue, formatAs);
        NextPriceLabel.text = FormatNumbers.Format(Lood.UpgradeManager.Instance.Upgrades[Data.Tag].Shop.NextPrice);

    }

    private void RebuildLayout()
    {

        var componentsInChildren = GetComponentsInChildren<RectTransform>();
        foreach (var layoutGroup in componentsInChildren)
        {
            var nestedChildGroup = layoutGroup.GetComponentsInChildren<RectTransform>();
            foreach (var rect in nestedChildGroup)
            {
                var nestedNestedChildGroup = rect.GetComponentsInChildren<RectTransform>();
                foreach (var nestRect in nestedNestedChildGroup)
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(nestRect);
                }
                LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup);

        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    IEnumerator LerpPos(float start, float end, float timeToMove, RectTransform rectTransform)
    {
        float t = 0;
        while (t < 1)
        {
            rectTransform.localScale = new Vector3(Mathf.Lerp(start, end, t), 1f, 1f);
            t += Time.deltaTime / timeToMove;
            yield return null;//Stops here until right after the next update loop, then continues
        }

        //t won't end up exactly at 1, so let's snap it to be precisely where we want.
        rectTransform.localScale = new Vector3(end, 1f, 1f);


    }
    private void Buy()
    {
        shopButton.Buy();
    }
}
