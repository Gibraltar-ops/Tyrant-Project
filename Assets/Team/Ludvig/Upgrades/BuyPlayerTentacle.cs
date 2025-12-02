
public class BuyPlayerTentacle : Upgrade
{
    private TyrantTentacleManager tentacleManager;
    void Start()
    {
        tentacleManager = FindAnyObjectByType<TyrantTentacleManager>();
    }

    public override void ApplyUpgrade()
    {
        tentacleManager.ActivateNextTentacle();
    }

    
}
