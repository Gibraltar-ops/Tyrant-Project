
public class UpgradeCollectAdd : Upgrade
{
    public Currency Currency;
    public CollectorShop shop;
    public int AddValue;
    
    public override void ApplyUpgrade()
    {
        foreach (CollectorData item in shop.Data)
        {
            if(item.CollectedCurrency == Currency)
            {
                item.CollectAdd += AddValue;
            }
        }
    }

    
}
