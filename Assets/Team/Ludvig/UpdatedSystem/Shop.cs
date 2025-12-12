using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class Shop : MonoBehaviour
{
    public Organ ParentOrgan;
    public UpgradeCategory Category;

    public string ItemName;
    public string ButtonText;
    public Tooltip Tooltip;

    public GameObject UIPrefab;

    public ShopSettings shopSettings;
    public Currency BoughtWithCurrency;

    //public GameObject ParentMenu;
    public TMP_Text ItemNameLabel {get; private set;}
    public TMP_Text CurrencyCountLabel {get; private set;}
    public TMP_Text NextPriceLabel {get; private set;}
    public Button PurchaseButton {get; private set;}
    public RectTransform ProgressBar {get; private set;}
    public GameObject InstancedUIPanel {get; private set;}
    public bool Active = false;
    private bool _upgradeButtonClicked = false;

    void OnEnable()
    {
        EconomyManager.onCurrencyChanged += CheckAffordability;
    }
    void OnDisable()
    {
        EconomyManager.onCurrencyChanged -= CheckAffordability;
    }
    void Start()
    {   
        
    }

    public void CreateStoreItem()
    {
        MenuCreator menu = transform.parent.gameObject.GetComponent<MenuCreator>();

        Transform shopRoot = menu.MenuInstance.GetComponent<UIMenuInfo>().ShopRoot.transform;
        Transform instanceParent = null;;

        foreach (Transform subcategory in shopRoot)
        {
            if(subcategory.gameObject.GetComponent<UISubcategoryInfo>().category == Category)
            {
                instanceParent = subcategory.gameObject.GetComponent<UISubcategoryInfo>().ShopPanel.transform;
                //Debug.Log(instanceParent.name);
            }
        }

        if (instanceParent != null)
        {
            InstancedUIPanel = Instantiate(UIPrefab, instanceParent);
            InstancedUIPanel.GetComponent<TooltipTrigger>().Tooltip = Tooltip;
            InstancedUIPanel.GetComponent<TooltipTrigger>().InitializeTooltipData("0");
            
            ItemNameLabel = InstancedUIPanel.GetComponent<UIItemPanelInfo>().ItemName;
            CurrencyCountLabel = InstancedUIPanel.GetComponent<UIItemPanelInfo>().CurrentCount;
            NextPriceLabel = InstancedUIPanel.GetComponent<UIItemPanelInfo>().Price;
            PurchaseButton = InstancedUIPanel.GetComponent<UIItemPanelInfo>().PurchaseButton;
            ProgressBar = InstancedUIPanel.GetComponent<UIItemPanelInfo>().ProgressBar.GetComponent<RectTransform>();


            PurchaseButton.onClick.AddListener(PurchaseUpgrade);

            UpdateLabels();

        }
        Active = true;
        CheckAffordability(BoughtWithCurrency);

        // _itemName = StoreItem.transform.Find("ItemName").GetComponent<TMP_Text>();
        // _currentCountLabel = StoreItem.transform.Find("CurrentCount").GetComponent<TMP_Text>();
        // _nextPriceLabel = StoreItem.transform.Find("NextPrice").GetComponent<TMP_Text>();
        // _purchaseButton = StoreItem.transform.Find("PurchaseButton").GetComponent<Button>();

        
        // _purchaseButton.onClick.AddListener(PurchaseUpgrade);
        // UpdateLabels();
    }

    public GameObject GetUIPanel()
    {
        return InstancedUIPanel;
    }

    public void IncrementPrice()
    {
        shopSettings.NextPrice = (int)(shopSettings.PriceBase * Mathf.Pow(shopSettings.PriceGrowthRate, shopSettings.NumberOwned));
    }

    public void CheckAffordability(Currency currency)
    {
        if (!Active)
        {
            return;
        }
        if (currency == BoughtWithCurrency)
        {   
            long currentCurrency = EconomyManager.Instance.GetCurrent(BoughtWithCurrency);

            float normalizedProgressBarScale;

            if(currentCurrency >= 1)
            {
                normalizedProgressBarScale = Mathf.Clamp((float)currentCurrency/shopSettings.NextPrice, 0f, 1f);
            }
            else
            {
                normalizedProgressBarScale = 0f;
            }
            ProgressBar.localScale = new Vector3(normalizedProgressBarScale, 1f, 1f);

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

    public void UpdateLabels()
    {
        ItemNameLabel.text = ItemName;
        CurrencyCountLabel.text = shopSettings.NumberOwned.ToString();
        NextPriceLabel.text = shopSettings.NextPrice.ToString();

        if (!_upgradeButtonClicked)
        {   
            _upgradeButtonClicked = true;
             CurrencyCountLabel.gameObject.transform.DOPunchScale(new Vector3(0.6f, 0.6f, 0.6f), .5f, 1, 0f).onComplete = ResetButton;
        }
 
    }

    public void UpdateTooltip(string value)
    {
        //InstancedUIPanel.GetComponent<TooltipTrigger>().UpdateTooltip(value);
    }

    public void MaxOutLabels()
    {
        ItemNameLabel.text = ItemName;
        CurrencyCountLabel.text = "Max: " + shopSettings.MaxOwned;
        NextPriceLabel.text = "";//shopSettings.NextPrice.ToString();
        PurchaseButton.gameObject.SetActive(false);

    }
    private void ResetButton()
    {
        _upgradeButtonClicked = false;
    }

    public void PurchaseUpgrade()
    { 

        Upgrade[] upgrades = GetComponents<Upgrade>();
        foreach (Upgrade upgrade in upgrades)
        {
            upgrade.ApplyUpgrade();
        } 

        EconomyManager.Instance.IncrementCurrency(BoughtWithCurrency, -shopSettings.NextPrice);
        shopSettings.NumberOwned++;

        if(shopSettings.NumberOwned < shopSettings.MaxOwned)
        {
        IncrementPrice();
        UpdateLabels();
        PopupTextData data = new PopupTextData((Vector2)Camera.main.ScreenToWorldPoint((Vector2)PurchaseButton.gameObject.transform.position) + Random.insideUnitCircle, "+1", Color.green);
        EconomyManager.Instance.PopUp(data);
        }
        else
        {
            MaxOutLabels();
        }
        
    }

    

}
