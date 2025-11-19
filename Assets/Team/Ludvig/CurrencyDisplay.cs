using TMPro;
using UnityEngine;

public class CurrencyDisplay : MonoBehaviour
{
    public Currency Currency;
    private TMP_Text label;
    void OnEnable()
    {
        EconomyManager.onCurrencyChanged += UpdateValue;
        label = GetComponent<TMP_Text>();
    }
    void OnDisable()
    {
        EconomyManager.onCurrencyChanged -= UpdateValue;

    }

    void Start()
    {
        switch (Currency)
        {
            case Currency.Biomass:
                label.text = EconomyManager.Instance.Biomass.ToString();
                break;
            case Currency.Brains:
                label.text = EconomyManager.Instance.Brains.ToString();
                break;
            default: break;
        }
    }

    public void UpdateValue(Currency currency)
    {
        if (currency == Currency)
        {
            switch (currency)
            {
                case Currency.Biomass:
                    label.text = EconomyManager.Instance.Biomass.ToString();
                    break;
                case Currency.Brains:
                    label.text = EconomyManager.Instance.Brains.ToString();
                    break;
                default: break;
            }
        }

    }

}
