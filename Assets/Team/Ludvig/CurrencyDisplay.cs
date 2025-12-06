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
                    label.text = FormatNumbers.Format(EconomyManager.Instance.Biomass);
                    break;
                case Currency.Brains:              
                    label.text = FormatNumbers.Format(EconomyManager.Instance.Brains);
                    break;
                case Currency.Keratin:
                    label.text = FormatNumbers.Format(EconomyManager.Instance.Keratin);
                    break;
                case Currency.Victims:
                    label.text = FormatNumbers.Format(EconomyManager.Instance.Victims);
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
                    label.text = FormatNumbers.Format(EconomyManager.Instance.Biomass);
                    break;
                case Currency.Brains:              
                    label.text = FormatNumbers.Format(EconomyManager.Instance.Brains);
                    break;
                case Currency.Keratin:
                    label.text = FormatNumbers.Format(EconomyManager.Instance.Keratin);
                    break;
                case Currency.Victims:
                    label.text = FormatNumbers.Format(EconomyManager.Instance.Victims);
                    break;
                default: break;
            }
        }

    }

}
