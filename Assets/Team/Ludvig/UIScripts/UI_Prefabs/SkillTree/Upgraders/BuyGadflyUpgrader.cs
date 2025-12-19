
using UnityEngine;

public class BuyGadflyUpgrader : Upgrader
{
    private bool _canBuy = true;
    private GadflySystem gadflyManager;
    void Awake()
    {
        gadflyManager = GameObject.FindAnyObjectByType<GadflySystem>();
    }
    public override void ApplyUpgrade(bool maxedOut)
    {
        // if (!_canBuy)
        // {
        //     return;
        // }

        // if (maxedOut)
        // {
        //     _canBuy = false;
        // }
        EconomyManager.Instance.GadflyCount++;
        gadflyManager.SpawnGatfly();
        base.ApplyUpgrade(maxedOut);
    }


}

