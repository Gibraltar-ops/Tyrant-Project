
using UnityEngine;

public class UpgradeAddTentacle : Upgrade
{
    private TyrantController tyrantController;

    public override void ApplyUpgrade()
    {
       
        if (tyrantController == null)
        {
            tyrantController = FindObjectOfType<TyrantController>();
        }

   
        if (tyrantController != null)
        {
            tyrantController.ActivateNewTentacle();
        }
        else
        {
            Debug.LogError("TyrantController not found");
        }
    }
}