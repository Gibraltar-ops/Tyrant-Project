using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EconomyManager : MonoBehaviour
{


    public long Biomass = 100;
    private bool _biomassActive = false;
    public long Brains;
    private bool _brainsActive = false;
    public long Keratin;
    private bool _keratinActive = false;
    public long Victims;

    public long BiomassWinThreshold = 20000;

    public long BiomassLifetime = 12;

    public CurrencyPerSecond BiomassPerSecond = new CurrencyPerSecond();
    public CurrencyPerSecond BrainsPerSecond = new CurrencyPerSecond();
    public CurrencyPerSecond KeratinPerSecond = new CurrencyPerSecond();

    public delegate void OnCurrencyChanged(Currency currency);
    public static event OnCurrencyChanged onCurrencyChanged;

    public delegate void OnTextPopup(PopupTextData data);
    public static event OnTextPopup onTextPopup;


    private static EconomyManager _instance;
    public static EconomyManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<EconomyManager>();
                if (_instance == null)
                {
                    GameObject singleton = new GameObject(typeof(EconomyManager).ToString());
                    _instance = singleton.AddComponent<EconomyManager>();
                    DontDestroyOnLoad(singleton);
                }
            }
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Update()
    {
        if(_biomassActive)
        BiomassPerSecond.UpdateValue();

        if(_brainsActive)
        BrainsPerSecond.UpdateValue();

        if(_keratinActive)
        KeratinPerSecond.UpdateValue();

    }

    public void IncrementCurrency(Currency currency, long value)
    {
      SetValue(currency, value);
    }

     public void IncrementCurrency(Currency currency, long value, PopupTextData popupTextData)
    {
       SetValue(currency, value);
       PopUp(popupTextData);
    }

    private void SetValue(Currency currency, long value)
    {
         switch (currency)
        {
            case Currency.Biomass:
                Biomass += value;
                if(!_biomassActive) _biomassActive = true;
                BiomassPerSecond.AddCurrency((float)value);

                if(Mathf.Sign(value) == 1)
                {
                    BiomassLifetime += value;
                }
                Biomass = Biomass <= 0 ? 0 : Biomass;
                onCurrencyChanged?.Invoke(Currency.Biomass);
                break;
            case Currency.Brains:
                Brains += value;
                if(!_brainsActive) _brainsActive = true;
                BrainsPerSecond.AddCurrency((float)value);

                Brains = Brains <= 0 ? 0 : Brains;
                onCurrencyChanged?.Invoke(Currency.Brains);
                break;
            case Currency.Keratin:
                Keratin += value;
                if(!_keratinActive) _keratinActive = true;
                KeratinPerSecond.AddCurrency((float)value);

                Keratin = Keratin <= 0 ? 0 : Keratin;
                onCurrencyChanged?.Invoke(Currency.Keratin);
                break;
            case Currency.Victims:
                Victims += value;
                Victims = Victims <= 0 ? 0 : Victims;
                onCurrencyChanged?.Invoke(Currency.Victims);
                break;
            case Currency.BiomassLifetime:
                BiomassLifetime += value;
                BiomassLifetime = BiomassLifetime <= 0 ? 0 : BiomassLifetime;
                onCurrencyChanged?.Invoke(Currency.BiomassLifetime);
                break;
            default:
                break;
        }
    }

    public void PopUp(PopupTextData data)
    {
        onTextPopup?.Invoke(data);
    }

    public bool CanAfford(Currency currency, int cost)
    {
        switch (currency)
        {
            case Currency.Biomass:
                return cost <= Biomass ? true : false;
            case Currency.Brains:
                return cost <= Brains ? true : false;
            case Currency.Keratin:
                return cost <= Keratin ? true : false;
            case Currency.Victims:
                return cost <= Victims ? true : false;
            default:
                return false;
        }
    }

    public long GetCurrent(Currency currency)
    {
        switch (currency)
        {
            case Currency.Biomass:
                return Biomass;
            case Currency.Brains:
                return Brains;
            case Currency.Keratin:
                return Keratin;
            case Currency.Victims:
                return Victims;
            case Currency.BiomassLifetime:
                return BiomassLifetime;
            default:
                return 0;
        }
    }

    public void UpdateBiomassLabel(double value)
{
    string labelValue = FormatNumbers.Format(value);
    //currentBiomassLabel.text = labelValue;
}

}

public enum Currency
{
    Biomass,
    Brains,
    Keratin,
    Victims,
    BiomassLifetime,
    HealthPoints,
    None
}

public enum ShopTag
{
    BiomassCollect,
    BrainsCollect,

}

//(AI Generated) Class for calculating currency per second.
public class CurrencyPerSecond
{
    public float windowSize = 5f;         // Rolling window in seconds
    public float smoothingFactor = 1.5f;  // EMA responsiveness

    private Queue<(float time, float amount)> events = new Queue<(float, float)>();
    public float PerSecond = 0f;
   

    public void UpdateValue()
    {
        float now = Time.time;

        // Remove old events outside the window
        while (events.Count > 0 && now - events.Peek().time > windowSize)
        {
            events.Dequeue();
        }

        // Calculate rolling window CPS
        float total = 0f;
        foreach (var e in events)
        {
            total += e.amount;
        }
        float windowCPS = total / windowSize;

        // Blend with EMA for smoothness
        PerSecond += smoothingFactor * (windowCPS - PerSecond);
        
    }

    public void AddCurrency(float amount)
    {   if(Mathf.Sign(amount) == 1)
        events.Enqueue((Time.time, amount));
    }
}
