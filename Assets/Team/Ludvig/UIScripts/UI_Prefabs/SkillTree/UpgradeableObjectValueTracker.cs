using TMPro;
using UnityEngine;

public class CurrencySourceValueTracker : MonoBehaviour
{

    public CurrencySourceSO currencySource;
    public bool HideZeroCount = false;

    public TrackableStat TrackedStat;

    [Header("Collection Stat Track")]
    public Currency CollectedCurrency;
    public CollectStat CollectStat;

    [Header("Main Stat Track")]
    public MainStat MainStat;

    public ValueFormatType FormatAs;
    private TMP_Text label;
    private bool _initialized = false;

    void OnEnable()
    {
        if (_initialized) UpdateLabel();
        Upgrader.onUpgradeApplied += UpdateLabel;
    }
    void OnDisable()
    {
        Upgrader.onUpgradeApplied -= UpdateLabel;

    }

    void Start()
    {
        label = GetComponent<TMP_Text>();
        Invoke("Initialize", 0.01f);
    }

    private void Initialize()
    {
        UpdateLabel();
        _initialized = true;
    }

    public void UpdateLabel()
    {
        if (!Lood.UpgradeManager.Instance.CurrencySources.ContainsKey(currencySource.Tag))
        {
            Debug.LogWarning("No such Upgradeable Object key in Upgrade Manager: " + currencySource.Tag);
            return;
        }

        if (TrackedStat == TrackableStat.MainStat)
        {
            label.text = FormatNumbers.FormatValueString(Lood.UpgradeManager.Instance.GetCurrencySourceStat(currencySource.Tag, MainStat), FormatAs);
            return;
        }
        if (TrackedStat == TrackableStat.Collection)
        {
            label.text = FormatNumbers.FormatValueString(Lood.UpgradeManager.Instance.CurrencySources[currencySource.Tag].CollectStats[CollectedCurrency].Stats[CollectStat], FormatAs);
            return;
        }

        if (TrackedStat == TrackableStat.Count)
        {
            if (HideZeroCount && Lood.UpgradeManager.Instance.CurrencySources[currencySource.Tag].Shop.CurrentOwned == 0)
            {
                label.text = "";
                return;
            }
            if (Lood.UpgradeManager.Instance.CurrencySources[currencySource.Tag].Shop.MaxedOut)
            {
                label.text = FormatNumbers.FormatValueString(Lood.UpgradeManager.Instance.CurrencySources[currencySource.Tag].Shop.CurrentOwned, FormatAs);
                return;
            }

            label.text = FormatNumbers.FormatValueString(Lood.UpgradeManager.Instance.CurrencySources[currencySource.Tag].Shop.CurrentOwned, FormatAs);
            return;
        }
        if (TrackedStat == TrackableStat.NextPrice)
        {
            if (Lood.UpgradeManager.Instance.CurrencySources[currencySource.Tag].Shop.MaxedOut)
            {
                label.text = "";
                return;
            }
            label.text = FormatNumbers.FormatValueString(Lood.UpgradeManager.Instance.CurrencySources[currencySource.Tag].Shop.NextPrice, FormatAs);
            return;
        }

    }
    // Update is called once per frame
}

public enum TrackableStat
{
    Collection,
    MainStat,
    Count,
    NextPrice
}



