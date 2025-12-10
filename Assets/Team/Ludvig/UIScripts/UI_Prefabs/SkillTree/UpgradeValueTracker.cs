using TMPro;
using UnityEngine;

public class UpgradeValueTracker : MonoBehaviour
{
    public UpgradeTag upgradeTag;
    public UpgradeTrackValue trackValue;

    public ValueFormatType FormatAs;
    private TMP_Text label;
    private bool _initialized = false;

    void OnEnable()
    {   
        if(_initialized) UpdateLabel();
        Upgrader.onUpgradeApplied += UpdateLabel;
    }
    void OnDisable()
    {
        Upgrader.onUpgradeApplied -= UpdateLabel;
    }
    void Start()
    {
       label = GetComponent<TMP_Text>();
       Invoke("Initialize", 0.01f);
    }
    private void Initialize()
    {
         
        UpdateLabel();
        _initialized = true;
    }

    // Update is called once per frame
    public void UpdateLabel()
    {
        if (!Lood.UpgradeManager.Instance.Upgrades.ContainsKey(upgradeTag))
        {
            Debug.LogWarning("No such Upgrade key.");
            return;
        }
        
        switch (trackValue)
        {
            case UpgradeTrackValue.Count:
                 if (Lood.UpgradeManager.Instance.Upgrades[upgradeTag].Shop.MaxedOut)
                    {   
                label.text = "MAX: " + FormatNumbers.FormatValueString(Lood.UpgradeManager.Instance.Upgrades[upgradeTag].Shop.CurrentOwned, FormatAs);
                    break;
                    }
                label.text = FormatNumbers.FormatValueString(Lood.UpgradeManager.Instance.Upgrades[upgradeTag].Shop.CurrentOwned, FormatAs);
                break;
            case UpgradeTrackValue.CurrentModifier:
                label.text = FormatNumbers.FormatValueString(Lood.UpgradeManager.Instance.Upgrades[upgradeTag].CurrentModifier, FormatAs);
                break;
            case UpgradeTrackValue.NextPrice:
                 
                 if (Lood.UpgradeManager.Instance.Upgrades[upgradeTag].Shop.MaxedOut)
                    {   
                    label.text = "";
                    break;
                    }
                label.text = FormatNumbers.FormatValueString(Lood.UpgradeManager.Instance.Upgrades[upgradeTag].Shop.NextPrice, FormatAs);
                break;
            default: break;
        }
        
    }
}

public enum UpgradeTrackValue
{
    Count,
    CurrentModifier,
    NextPrice,
}

