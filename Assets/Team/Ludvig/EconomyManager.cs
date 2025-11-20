using UnityEngine;

public class EconomyManager : MonoBehaviour
{


    public long Biomass = 12;
    public long Brains;
    public long Keratin;

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
                onCurrencyChanged?.Invoke(Currency.Biomass);
                break;
            case Currency.Brains:
                Brains += value;
                onCurrencyChanged?.Invoke(Currency.Brains);
                break;
            case Currency.Keratin:
                Keratin += value;
                onCurrencyChanged?.Invoke(Currency.Keratin);
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
            default:
                return false;
        }
    }

}

public enum Currency
{
    Biomass,
    Brains,
    Keratin
}

public enum ShopTag
{
    AutomaticTentacle,
    AttackTentacle,
    GadflyBrute,
    GadflySuicideBomber,
    GadflySwarm,

}
