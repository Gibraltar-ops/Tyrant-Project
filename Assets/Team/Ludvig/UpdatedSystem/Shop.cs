using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public Organ ParentOrgan;
    public UpgradeCategory Category;

    
    public string Tag;

    public string ItemName;
    public string ButtonText;
    public Tooltip tooltip;

    public GameObject UIPrefab;

    public ShopSettings shopSettings;
    public Currency BoughtWithCurrency;

    public GameObject ParentMenu;
    private TMP_Text _itemName;
    private TMP_Text _currentCountLabel;
    private TMP_Text _nextPriceLabel;
    private Button _purchaseButton;

    void OnEnable()
    {
        EconomyManager.onCurrencyChanged += CheckAffordability;
    }
    void OnDisable()
    {
        EconomyManager.onCurrencyChanged -= CheckAffordability;
    }
    void Start()
    {   //CreateStoreItem();
        shopSettings.Tag = Tag;
        
        if (TryGetComponent(out Harvester harvester))
        {
            harvester.Data.Tag = Tag;
        }
        
    }

    public void CreateStoreItem()
    {
        GameObject StoreItem = Instantiate(UIPrefab, ParentMenu.transform);

        _itemName = StoreItem.transform.Find("ItemName").GetComponent<TMP_Text>();
        _currentCountLabel = StoreItem.transform.Find("CurrentCount").GetComponent<TMP_Text>();
        _nextPriceLabel = StoreItem.transform.Find("NextPrice").GetComponent<TMP_Text>();
        _purchaseButton = StoreItem.transform.Find("PurchaseButton").GetComponent<Button>();
        _purchaseButton.onClick.AddListener(PurchaseUpgrade);
        UpdateLabels();
    }

    public void IncrementPrice()
    {
        shopSettings.NextPrice = (int)(shopSettings.PriceBase * Mathf.Pow(shopSettings.PriceGrowthRate, shopSettings.NumberOwned));
    }

    public void CheckAffordability(Currency currency)
    {
        if (currency == BoughtWithCurrency)
        {
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
    }

    public void PurchaseUpgrade()
    {
        GetComponent<Upgrade>().ApplyUpgrade();
        EconomyManager.Instance.IncrementCurrency(BoughtWithCurrency, -shopSettings.NextPrice);
        shopSettings.NumberOwned++;
        IncrementPrice();
        UpdateLabels();
    }

    

}
