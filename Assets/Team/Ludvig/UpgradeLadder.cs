using System;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeLadder", menuName = "Scriptable Objects/UpgradeLadder")]
public class UpgradeLadder : ScriptableObject
{
    public UpgradeLevel[] Levels;

    [Serializable]
    public struct UpgradeLevel
    {
        public float Value;
        public int Price;
        public Currency Currency;
    }

    public enum Currency
    {
        Biomass,
        Brains
    }

}



