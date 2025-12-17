using UnityEngine;
using UnityEngine.UI;

public class HealthTestButton : MonoBehaviour
{
    public CurrencySourceSO data;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(DoDamage);
    }

    void DoDamage()
    {
        Lood.UpgradeManager.Instance.CurrencySources[data.Tag].MainStats[MainStat.HealthMaxAdditivePercentage] += 20;
        //EconomyManager.Instance.IncrementCurrency(Currency.HealthPoints, -100);
    }
}
