

public class BuyCollector2 : Upgrade
{   
    public override void ApplyUpgrade()
    {
      GetComponent<Harvester>().SpawnCollector();
    }
    
}
