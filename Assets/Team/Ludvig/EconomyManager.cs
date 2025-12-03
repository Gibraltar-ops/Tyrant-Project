using Unity.VisualScripting;
using UnityEngine;

public class EconomyManager : MonoBehaviour
{


    public long Biomass = 12;
    public long Brains;
    public long Keratin;
    public long Victims;

    public long BiomassWinThreshold = 20000;

    public long BiomassLifetime = 12;

    public delegate void OnCurrencyChanged(Currency currency);
    public static event OnCurrencyChanged onCurrencyChanged;


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

    public void IncrementCurrency(Currency currency, long value)
    {
        switch (currency)
        {
            case Currency.Biomass:
                Biomass += value;

                if(Mathf.Sign(value) == 1)
                {
                    BiomassLifetime += value;
                }
                Biomass = Biomass <= 0 ? 0 : Biomass;
                onCurrencyChanged?.Invoke(Currency.Biomass);
                break;
            case Currency.Brains:
                Brains += value;
                Biomass = Biomass <= 0 ? 0 : Biomass;
                onCurrencyChanged?.Invoke(Currency.Brains);
                break;
            case Currency.Keratin:
                Keratin += value;
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

}

public enum Currency
{
    Biomass,
    Brains,
    Keratin,
    Victims,
    BiomassLifetime
}

public enum ShopTag
{
    BiomassCollect,
    BrainsCollect,

}
