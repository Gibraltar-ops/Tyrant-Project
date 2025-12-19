using System.Collections;
using System.Collections.Generic;
using Lood;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIUpgradeableTooltip : MonoBehaviour
{
    [HideInInspector]
    public PurchaseableSO Data;

    [HideInInspector]
    public CurrencySourceShopButton shopButton;

    [HideInInspector]
    public ButtonHierarchy buttonHierarchy;

    public List<GameObject> HideOnMaxedOut = new();
    public List<GameObject> ShowOnMaxedOut = new();

    public TMP_Text NextPriceLabel;
    public TMP_Text LevelLabel;
    public Image BoughtWithCurrencyIcon;
    public TMP_Text NameLabel;
    public TMP_Text DescriptionLabel;

    public TMP_Text EffectHeaderLabel;
    public TMP_Text LoreLabel;
    public RectTransform ProgressSlider;
    public Button UpgradeButton;

    [Header("Currency Trackers")]
    public TMP_Text BiomassCollectLabel;
    public GameObject BiomassLabelParent;
    public TMP_Text BrainsCollectLabel;
    public GameObject BrainsLabelParent;
    public TMP_Text KeratinCollectLabel;

    public GameObject KeratinLabelParent;

    public TMP_Text HPCollectLabel;
    public GameObject HPLabelParent;

    private Image[] _colorizableImages;



    void Awake()
    {
        UpgradeButton.onClick.AddListener(Buy);
        CollectColorizableImages();
        gameObject.SetActive(false);
        Lood.UpgradeManager.Instance.UpgradeableTooltipPanel = gameObject;

    }
    void OnEnable()
    {

        UpdateFields();
        SetTheme();

        Upgrader.onUpgradeApplied += UpdateFields;
        EconomyManager.onCurrencyChanged += UpdateOnCurrencyChange;
        Lood.UpgradeManager.Instance.UpgradeableTooltipPanel = gameObject;


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
            if (item.gameObject.CompareTag("UI_IgnoreTheming"))
            {
                continue;
            }

            filteredImageList.Add(item);
        }
        _colorizableImages = filteredImageList.ToArray();
    }

    private void SetTheme()
    {
        CurrencySourceSO upgrade = Data as CurrencySourceSO;
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

        //Set shop tracker colors and sprite.
        CurrencySourceStatBlock stats = Lood.UpgradeManager.Instance.CurrencySources[Data.Tag];
        CurrencySpriteData spriteData = EconomyManager.Instance.GetCurrencySpriteData(stats.Shop.BoughtWithCurrency);
        BoughtWithCurrencyIcon.sprite = spriteData.Sprite;
        BoughtWithCurrencyIcon.color = spriteData.OverlayColor;
        ProgressSlider.GetComponent<Image>().color = spriteData.OverlayColor;

    }

    public void UpdateOnCurrencyChange(Currency currency)
    {
        if (currency != Lood.UpgradeManager.Instance.CurrencySources[Data.Tag].Shop.BoughtWithCurrency) return;

        UpgradeButton.interactable = shopButton._button.interactable;

        UpdateCollectLabels();
        StopAllCoroutines();
        UpdateFields();
        float previousNormalizedScale = ProgressSlider.localScale.x;

        long currentCurrency = EconomyManager.Instance.GetCurrent(Lood.UpgradeManager.Instance.CurrencySources[Data.Tag].Shop.BoughtWithCurrency);

        float normalizedProgressBarScale;

        if (currentCurrency >= 1)
        {
            normalizedProgressBarScale = Mathf.Clamp((float)currentCurrency / Lood.UpgradeManager.Instance.CurrencySources[Data.Tag].Shop.NextPrice, 0f, 1f);
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
        if (Lood.UpgradeManager.Instance.CurrencySources[Data.Tag].Shop.MaxedOut)
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

        CurrencySourceStatBlock stats = Lood.UpgradeManager.Instance.CurrencySources[Data.Tag];
        NameLabel.text = Data.Shop.Name;
        DescriptionLabel.text = Data.Shop.Description;
        EffectHeaderLabel.text = Data.Shop.EffectString;
        LoreLabel.text = Data.Shop.Lore;
        LevelLabel.text = FormatNumbers.FormatValueString(stats.Shop.CurrentOwned, ValueFormatType.IntegerRaw);
        NextPriceLabel.text = FormatNumbers.Format(stats.Shop.NextPrice);



        UpdateCollectLabels();


    }
    private void UpdateCollectLabels()
    {
        CurrencySourceStatBlock stats = Lood.UpgradeManager.Instance.CurrencySources[Data.Tag];

        if (stats.CollectStats[Currency.Biomass].Stats[CollectStat.CollectTotalCurrent] != 0)
        {
            BiomassCollectLabel.text = FormatNumbers.FormatValueString(stats.CollectStats[Currency.Biomass].Stats[CollectStat.CollectTotalCurrent], ValueFormatType.IntegerAdditive);
            BiomassLabelParent.SetActive(true);
        }
        else
        {
            BiomassLabelParent.SetActive(false);
        }

        if (stats.CollectStats[Currency.Brains].Stats[CollectStat.CollectTotalCurrent] != 0)
        {
            BrainsCollectLabel.text = FormatNumbers.FormatValueString(stats.CollectStats[Currency.Brains].Stats[CollectStat.CollectTotalCurrent], ValueFormatType.IntegerAdditive);
            BrainsLabelParent.SetActive(true);
        }
        else
        {
            BrainsLabelParent.SetActive(false);
        }

        if (stats.CollectStats[Currency.Keratin].Stats[CollectStat.CollectTotalCurrent] != 0)
        {
            KeratinCollectLabel.text = FormatNumbers.FormatValueString(stats.CollectStats[Currency.Keratin].Stats[CollectStat.CollectTotalCurrent], ValueFormatType.IntegerAdditive);
            KeratinLabelParent.SetActive(true);
        }
        else
        {
            KeratinLabelParent.SetActive(false);
        }

        if (stats.CollectStats[Currency.HealthPoints].Stats[CollectStat.CollectTotalCurrent] != 0)
        {
            HPCollectLabel.text = FormatNumbers.FormatValueString(stats.CollectStats[Currency.HealthPoints].Stats[CollectStat.CollectTotalCurrent], ValueFormatType.IntegerAdditive);
            HPLabelParent.SetActive(true);
        }
        else
        {
            HPLabelParent.SetActive(false);
        }

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
