
public class UpgradeTickMultiplier : Upgrade
{
    public float AddValue;
    
    public override void ApplyUpgrade()
    {
       shop.TicksPerSecondMultiplier += AddValue;
    }

    
}
