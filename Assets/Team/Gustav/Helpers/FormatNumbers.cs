using UnityEngine;
using System.Collections.Generic;
using System;

public static class FormatNumbers
{
    private static readonly Dictionary<int, string> units = new Dictionary<int, string>
    {
        {0, ""},
        {1, "K"},
        {2, "M"},
        {3, "B"},
        {4, "T"},
    };
    private static readonly int charA = Convert.ToInt32('a');

    public static string Format(double value)
    {
        if (value < 1d)
        {
            return "0";
        }

        var n = (int)Math.Log(value, 1000);
        var m = value / Math.Pow(1000, n);
        var unit = "";
        if (n < units.Count)
        {
            unit = units[n];
        }
        else
        {
            var unitInt=n- units.Count;
            var secondUnit = unitInt % 26;
            var firstUnit = unitInt / 26;
            unit = Convert.ToChar(firstUnit + charA).ToString() + Convert.ToChar(secondUnit + charA).ToString();
        }

        return (Math.Floor(m * 100) / 100).ToString("0.##") + unit;
    }

    public static string FormatValueString(float value, ValueFormatType valueType)
    {
        switch (valueType)
        {
             case ValueFormatType.FloatRaw:
                return value.ToString("0.0");
            case ValueFormatType.IntegerRaw:
                return value.ToString("0");
            case ValueFormatType.FloatAdditive:
                return "+" + value.ToString("0.0");
            case ValueFormatType.IntegerAdditive:
                return "+" + value.ToString("0");
            case ValueFormatType.IntegerPerSecond:
                return "+" + value.ToString("0") + " /s";
            case ValueFormatType.FloatPerSecond:
                return "+" + value.ToString("0.0") + " /s";
            case ValueFormatType.PercentageAdditive:
                return "+" + value.ToString("0") + "%";
            case ValueFormatType.PercentageMultiplicative:
                return "x" + value.ToString("0") + "%";
            case ValueFormatType.SinglePurchase:
                return "";
            default: return "";
        }
    }
}

public enum ValueFormatType
{
    FloatRaw,
    IntegerRaw,
    FloatAdditive,
    IntegerAdditive,
    IntegerPerSecond,
    FloatPerSecond,
    PercentageAdditive,
    PercentageMultiplicative,
    SinglePurchase
}

//Deprecated
public enum UpgradeValueType
{
    FloatRaw,
    IntegerRaw,
    IntegerAdditive,
    IntegerPerSecond,
    FloatAdditive,
    FloatPerSecond,
    PercentageAdditive,
    PercentageMultiplicative,
    SinglePurchase
}

// Use this method to update the text correctly regards towards the formating.
//public void UpdateBiomassLabel(double value)
//{
//    string labelValue = FormatNumbers.Format(value);
//    currentBiomassLabel.text = labelValue;
//}