

public class BuyCollector2 : Upgrade
{   
    public override void ApplyUpgrade()
    {
      GetComponent<Lood.Harvester>().SpawnCollector();
    }
    
}
