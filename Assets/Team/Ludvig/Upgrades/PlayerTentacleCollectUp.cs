
public class PlayerTentacleCollectUp : Upgrade
{
    public TyrantController tyrantController;
    public int AddValue;
    
    public override void ApplyUpgrade()
    {
        tyrantController.biomassPerHuman += AddValue;
    }

    
}
