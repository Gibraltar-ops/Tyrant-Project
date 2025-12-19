
using UnityEngine;

public class BuyPlayerTentacleUpgrader : Upgrader
{
    private bool _canBuy = true;
    private TyrantTentacleManager tentacleManager;
    void Awake()
    {
        tentacleManager = GameObject.FindAnyObjectByType<TyrantTentacleManager>();
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
        EconomyManager.Instance.TentacleCount++;
        EconomyManager.Instance.HunterTentacleCount++;
        tentacleManager.ActivateNextTentacle();
        base.ApplyUpgrade(maxedOut);
    }


}

