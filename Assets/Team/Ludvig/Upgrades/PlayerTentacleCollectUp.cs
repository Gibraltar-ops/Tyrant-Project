
public class PlayerTentacleCollectUp : Upgrade
{
    public TyrantTentacleManager tentacleManager;
    public int AddValue;
    
    public override void ApplyUpgrade()
    {
        tentacleManager.collectorData.CollectAdd += AddValue;
    }

    
}
