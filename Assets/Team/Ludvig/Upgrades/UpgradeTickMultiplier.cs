
public class UpgradeTickMultiplier : Upgrade
{
    public float AddValue;
    public CollectorShop shop;
    
    public override void ApplyUpgrade()
    {
       shop.TicksPerSecondMultiplier += AddValue;
    }

    
}
