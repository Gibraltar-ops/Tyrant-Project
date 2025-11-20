
public class BuyPlayerTentacle : Upgrade
{
    public TyrantController tyrantController;
    
    public override void ApplyUpgrade()
    {
        tyrantController.ActivateNewTentacle();
    }

    
}
