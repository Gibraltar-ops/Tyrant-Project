
using UnityEngine;

public class Upgrader : MonoBehaviour
{
    public delegate void OnUpgradeApplied();
    public static event OnUpgradeApplied onUpgradeApplied;

    public virtual void ApplyUpgrade(bool maxedOut)
    {
        onUpgradeApplied?.Invoke();
    }
    
 
}

