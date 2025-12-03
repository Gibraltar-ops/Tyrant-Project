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
    private TMP_Text _itemName;
    private TMP_Text _currentCountLabel;
    private TMP_Text _nextPriceLabel;
    private Button _purchaseButton;
    private RectTransform _progressBar;
    private GameObject instancedUIPanel;
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
            instancedUIPanel = Instantiate(UIPrefab, instanceParent);
            instancedUIPanel.GetComponent<TooltipTrigger>().Tooltip = Tooltip;
            _itemName = instancedUIPanel.GetComponent<UIItemPanelInfo>().ItemName;
            _currentCountLabel = instancedUIPanel.GetComponent<UIItemPanelInfo>().CurrentCount;
            _nextPriceLabel = instancedUIPanel.GetComponent<UIItemPanelInfo>().Price;
            _purchaseButton = instancedUIPanel.GetComponent<UIItemPanelInfo>().PurchaseButton;
            _progressBar = instancedUIPanel.GetComponent<UIItemPanelInfo>().ProgressBar.GetComponent<RectTransform>();


            _purchaseButton.onClick.AddListener(PurchaseUpgrade);

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
        return instancedUIPanel;
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
            _progressBar.localScale = new Vector3(normalizedProgressBarScale, 1f, 1f);

            if (EconomyManager.Instance.CanAfford(BoughtWithCurrency, shopSettings.NextPrice))
            {
                _purchaseButton.interactable = true;
            }
            else
            {
                _purchaseButton.interactable = false;
            }
        }

    }

    public void UpdateLabels()
    {
        _itemName.text = ItemName;
        _currentCountLabel.text = shopSettings.NumberOwned.ToString();
        _nextPriceLabel.text = shopSettings.NextPrice.ToString();

        if (!_upgradeButtonClicked)
        {   
            _upgradeButtonClicked = true;
             _currentCountLabel.gameObject.transform.DOPunchScale(new Vector3(0.6f, 0.6f, 0.6f), .5f, 1, 0f).onComplete = ResetButton;
        }
 
    }
    public void MaxOutLabels()
    {
        _itemName.text = ItemName;
        _currentCountLabel.text = "Max: " + shopSettings.MaxOwned;
        _nextPriceLabel.text = "";//shopSettings.NextPrice.ToString();
        _purchaseButton.gameObject.SetActive(false);

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
        }
        else
        {
            MaxOutLabels();
        }
        
    }

    

}
