using UnityEngine;

[CreateAssetMenu(fileName = "CurrencySourceSO", menuName = "Scriptable Objects/CurrencySourceSO")]
public class CurrencySourceSO : PurchaseableSO
{
    public UIColorStyleSO ColorStyle;
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

    [Header("Song Stats")]
    public float SongTrickleStrengthBase = 1f;
    public float SongTrickleStrengthAdd = 0f;
    public float SongTrickleStrengthAdditivePercentage = 0f;
    public float SongTrickleFrequencyBase = 1f;
    public float SongTrickleFrequencyAdditivePercentage = 0f;
    public float SongPulseStrengthBase = 10f;
    public float SongPulseStrengthAdditivePercentage = 0f;
    public float SongPulseFrequencyBase = 10f;
    public float SongPulseFrequencyAdditivePercentage = 0f;
    public float SongPulsePerTentacleBonus = 0f;

    [Header("Health Stats")]
    public float HealthMaxBase = 1000f;
    public float HealthMaxMultiplier = 1f;
    public float HealthMaxAdditivePercentage = 0f;
    public float HealthMaxCurrentTotal;

    [Header("Collection Stats")]
    public CollectType BiomassCollect;
    public CollectType BrainsCollect;
    public CollectType KeratinCollect;
    public CollectType HealthPointsCollect;

    [Header("Kebabarm Stats")]
    public int KebabMaxStrikeCount = 4;

}
