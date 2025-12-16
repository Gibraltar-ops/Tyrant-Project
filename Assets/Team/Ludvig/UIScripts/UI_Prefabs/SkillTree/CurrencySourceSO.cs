using UnityEngine;

[CreateAssetMenu(fileName = "CurrencySourceSO", menuName = "Scriptable Objects/CurrencySourceSO")]
public class CurrencySourceSO : PurchaseableSO
{
    [Header("Speed Stats")]

    //Speed stats
    public float BaseSpeed = 0.2f;
    public float SpeedAdd = 0;
    public float SpeedMultiply = 1;
    public float SpeedAdditivePercentage = 0f;
    public float SpeedMultiplicativePercentage = 0f;

    [Header("Gadfly Stats")]
    public float GadflyCooldown = 10f;
    public float GadflyHarvestDuration = 6f;

    [Header("Collection Stats")]
    public CollectType BiomassCollect;
    public CollectType BrainsCollect;
    public CollectType KeratinCollect;
    public CollectType HealthPointsCollect;

}
