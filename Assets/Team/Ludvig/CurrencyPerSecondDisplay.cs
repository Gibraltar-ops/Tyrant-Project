using TMPro;
using UnityEngine;

public class CurrencyPerSecondDisplay : MonoBehaviour
{
    public Currency Currency;
    private TMP_Text label;
    public float UpdateInterval = 0.2f;
    private float _currentTime = 0f;
    void OnEnable()
    {  
        label = GetComponent<TMP_Text>();
    }
    

    void Start()
    {
        switch (Currency)
        {
           case Currency.Biomass:
                    label.text = FormatNumbers.Format((int)EconomyManager.Instance.BiomassPerSecond.PerSecond) + " /s";
                    break;
                case Currency.Brains:              
                    label.text = FormatNumbers.Format((int)EconomyManager.Instance.BrainsPerSecond.PerSecond) + " /s";
                    break;
                case Currency.Keratin:
                    label.text = FormatNumbers.Format((int)EconomyManager.Instance.BrainsPerSecond.PerSecond) + " /s";
                    break;
                default: break;
        }
    }

    void Update()
    {   
        _currentTime += Time.deltaTime;

        if(_currentTime >= UpdateInterval)
        {
             _currentTime = 0f;
       
            switch (Currency)
            {
                case Currency.Biomass:
                    label.text = FormatNumbers.Format((int)EconomyManager.Instance.BiomassPerSecond.PerSecond) + " /s";
                    break;
                case Currency.Brains:              
                    label.text = FormatNumbers.Format((int)EconomyManager.Instance.BrainsPerSecond.PerSecond) + " /s";
                    break;
                case Currency.Keratin:
                    label.text = FormatNumbers.Format((int)EconomyManager.Instance.KeratinPerSecond.PerSecond) + " /s";
                    break;
                default: break;
            }
        }
        
    }

}
