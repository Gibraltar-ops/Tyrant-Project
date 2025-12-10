
using Unity.VisualScripting;
using UnityEngine;

public class Upgrade : MonoBehaviour
{
    public UpgradeValueType ValueType;

    public virtual void Start()
    {
        
    }

    public virtual void ApplyUpgrade()
    {
        
    }

    public static string FormatValueString(float value, UpgradeValueType valueType)
    {
        switch (valueType)
        {
             case UpgradeValueType.FloatRaw:
                return value.ToString("0.0");
            case UpgradeValueType.IntegerRaw:
                return value.ToString("0");
            case UpgradeValueType.FloatAdditive:
                return "+" + value.ToString("0.0");
            case UpgradeValueType.IntegerAdditive:
                return "+" + value.ToString("0");
            case UpgradeValueType.IntegerPerSecond:
                return "+" + value.ToString("0") + " /s";
            case UpgradeValueType.FloatPerSecond:
                return "+" + value.ToString("0.0") + " /s";
            case UpgradeValueType.PercentageAdditive:
                return "+" + value.ToString("0") + "%";
            case UpgradeValueType.PercentageMultiplicative:
                return "x" + value.ToString("0") + "%";
            case UpgradeValueType.SinglePurchase:
                return "";
            default: return "";
        }
    }
 
}
