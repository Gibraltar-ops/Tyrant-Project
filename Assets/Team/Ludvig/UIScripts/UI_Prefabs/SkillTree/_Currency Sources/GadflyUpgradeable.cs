using UnityEngine;

public class GadflyUpgradeable : MonoBehaviour
{
    public CurrencySourceSO stats;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Lood.UpgradeManager.Instance.AddUpgradeableObject(stats);
    }
    void Start()
    {
        Debug.Log(Lood.UpgradeManager.Instance.CurrencySources[stats.Tag].CollectStats[Currency.Biomass].Stats[CollectStat.BaseCollect]);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
