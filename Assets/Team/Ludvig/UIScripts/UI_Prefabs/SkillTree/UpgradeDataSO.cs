using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeDataSO", menuName = "Scriptable Objects/UpgradeDataSO")]
public class UpgradeDataSO : ScriptableObject
{
    public UpgradeTag Tag;
    public ShopStats Shop;

    [Header("Upgrade Effects")]
    public List<UpgradeEffect> UpgradeEffects;
    public float CurrentModifier = 0f;
    public float AddValue = 0f;

}
