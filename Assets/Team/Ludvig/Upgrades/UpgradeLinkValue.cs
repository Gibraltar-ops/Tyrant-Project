
using System;
using UnityEngine.Events;

public class UpgradeLinkValue : Upgrade
{

    public Currency Currency;
    public HarvesterBaseValue Value;
    public float AddValue;
    public UnityEvent<float, HarvesterBaseValue, Currency> OnUpgrade;
    
    public override void ApplyUpgrade()
    {
        OnUpgrade.Invoke(AddValue, Value, Currency);
    }


    
}
