using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradeStore : MonoBehaviour
{
    public ShopSettings shopSettings;
    public Currency BoughtWithCurrency;
    public TMP_Text CurrentCountLabel;
    public TMP_Text NextPriceLabel;
    public Button PurchaseButton;

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
        PurchaseButton.onClick.AddListener(PurchaseUpgrade);
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
        CurrentCountLabel.text = shopSettings.NumberOwned.ToString();
        NextPriceLabel.text = shopSettings.NextPrice.ToString();
    }

    public void PurchaseUpgrade()
    {
        EconomyManager.Instance.IncrementCurrency(BoughtWithCurrency, -shopSettings.NextPrice);
        shopSettings.NumberOwned++;
        IncrementPrice();
        UpdateLabels();
    }

}
