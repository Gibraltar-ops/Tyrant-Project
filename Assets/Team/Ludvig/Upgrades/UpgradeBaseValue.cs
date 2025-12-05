
using System;
using UnityEngine.Events;

public class UpgradeBaseValue : Upgrade
{

    public Currency Currency;
    public HarvesterBaseValue Value;
    public float AddValue;
    public float CurrentValue = 0;
    public UnityEvent<float, HarvesterBaseValue, Currency> OnUpgrade;

    public override void Start()
    {
        base.Start();
        gameObject.GetComponent<Shop>().UpdateTooltip(FormatValueString(CurrentValue, ValueType));
    }

    public override void ApplyUpgrade()
    {
        OnUpgrade.Invoke(AddValue, Value, Currency);
        CurrentValue += AddValue;
        gameObject.GetComponent<Shop>().UpdateTooltip(FormatValueString(CurrentValue, ValueType));

    }


    
}
