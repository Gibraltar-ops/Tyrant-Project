
using UnityEngine.Events;

public class UpgradePurchaseNewShop : Upgrade
{
    public UnityEvent OnUpgrade;
    
    public override void ApplyUpgrade()
    {
        OnUpgrade.Invoke();
        RemoveUI();
        
    }
    public void RemoveUI()
    {
        GetComponent<Shop>().GetUIPanel().SetActive(false);
    }


    
}
