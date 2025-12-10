
using UnityEngine;

public class BuyCollectorUpgrader : Upgrader
{
    private bool _canBuy = true;
    public override void ApplyUpgrade(bool maxedOut)
    {
        if (!_canBuy)
        {
            return;
        }

        if (maxedOut)
        {
            _canBuy = false;
        }

        GetComponent<Lood.Harvester>().SpawnCollector();
        base.ApplyUpgrade(maxedOut);
    }
    

}

