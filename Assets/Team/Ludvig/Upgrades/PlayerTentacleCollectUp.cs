
public class PlayerTentacleCollectUp : Upgrade
{
    public TyrantTentacleManager tentacleManager;
    public Shop PlayerTentacleShop;
    public float AddValue;
    public float CurrentValue = 0;

    public override void Start()
    {
        base.Start();
        CurrentValue = tentacleManager.ModifiedCollectAmount();
        PlayerTentacleShop.UpdateTooltip(FormatValueString(CurrentValue, ValueType));
    }
    public override void ApplyUpgrade()
    {   
        CurrentValue += AddValue;
        tentacleManager.collectorData.CollectAdd += (int)AddValue;
        PlayerTentacleShop.UpdateTooltip(FormatValueString((float)tentacleManager.ModifiedCollectAmount(), UpgradeValueType.IntegerRaw));
    }

    
}
