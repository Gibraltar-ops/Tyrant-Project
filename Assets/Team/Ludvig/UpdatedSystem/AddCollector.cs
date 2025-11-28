

public class BuyCollector : Upgrade
{   
    public override void ApplyUpgrade()
    {
      GetComponent<Harvester>().SpawnCollector();
    }
    
}
