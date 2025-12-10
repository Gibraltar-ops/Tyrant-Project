using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeableObjectDataSO", menuName = "Scriptable Objects/UpgradeableObjectDataSO")]
public class UpgradeableObjectDataSO : ScriptableObject
{
    public UpgradeableObjectTag Tag;
    public ShopStats Shop;

    [Header("Source Stats")]

    //Speed stats
    public float BaseSpeed = 0.2f;
    public float SpeedAdd = 0;
    public float SpeedMultiply = 1;
    public float SpeedAdditivePercentage = 0f;
    public float SpeedMultiplicativePercentage = 0f;

    //Gadfly stats. Make sure to add everything to the 
    public float GadflyAggression = 1f;

    [Header("Collection Stats")]
    public CollectType BiomassCollect;
    public CollectType BrainsCollect;
    public CollectType KeratinCollect;

}
