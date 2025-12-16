using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeDataSO", menuName = "Scriptable Objects/UpgradeDataSO")]
public class UpgradeDataSO : PurchaseableSO
{
    [Header("Upgrade Effects")]
    public List<UpgradeEffect> UpgradeEffects;
    public float CurrentModifier = 0f;
    public float AddValue = 0f;
    public ValueFormatType FormatAs;
    public UIColorStyleSO ColorStyle;

}
