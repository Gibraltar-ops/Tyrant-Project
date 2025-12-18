using System.Collections;
using System.Collections.Generic;
using Lood;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISinglePurchaseTooltip : MonoBehaviour
{
    [HideInInspector]
    public PurchaseableSO Data;

    [HideInInspector]
    public CurrencySourceShopButton shopButton;

    [HideInInspector]
    public ButtonHierarchy buttonHierarchy;

    public List<GameObject> HideOnMaxedOut;
    public List<GameObject> ShowOnMaxedOut;

    public TMP_Text NextPriceLabel;
    public UnityEngine.UI.Image BoughtWithCurrencyIcon;
    public TMP_Text NameLabel;
    public TMP_Text DescriptionLabel;

    public TMP_Text LoreLabel;
    public RectTransform ProgressSlider;
    public Button UpgradeButton;

    private Image[] _colorizableImages;

    void Awake()
    {
        UpgradeButton.onClick.AddListener(Buy);
        CollectColorizableImages();
        gameObject.SetActive(false);
        Lood.UpgradeManager.Instance.SinglePurchaseTooltip = gameObject;
    }
    void OnEnable()
    {

        UpdateFields();
        SetTheme();
        SetProgressSlider();
        RebuildLayout();
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
        SetProgressSlider();

    }
    public void SetProgressSlider()
    {
        StopAllCoroutines();
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
            RebuildLayout();

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
        LoreLabel.text = Data.Shop.Lore;

        NextPriceLabel.text = FormatNumbers.Format(Lood.UpgradeManager.Instance.CurrencySources[Data.Tag].Shop.NextPrice);



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
