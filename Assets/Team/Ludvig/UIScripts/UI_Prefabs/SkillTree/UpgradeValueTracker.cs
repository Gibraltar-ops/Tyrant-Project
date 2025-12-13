using TMPro;
using UnityEngine;

public class UpgradeValueTracker : MonoBehaviour
{
    public bool GetDataFromParent = false;
    public UpgradeDataSO upgradeData;
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
    void Awake()
    {
        
       label = GetComponent<TMP_Text>();
       
    }
    void Start()
    {
        Initialize();
    }
    private void Initialize()
    {
         
        UpdateLabel();
        _initialized = true;
    }

    // Update is called once per frame
    public void UpdateLabel()
    {
        if (!Lood.UpgradeManager.Instance.Upgrades.ContainsKey(upgradeData.Tag))
        {
            Debug.LogWarning("Upgrade Key" + upgradeData.Tag + " does not exist in upgrade manager.");
            return;
        }
        
        switch (trackValue)
        {
            case UpgradeTrackValue.Count:
                 if (Lood.UpgradeManager.Instance.Upgrades[upgradeData.Tag].Shop.MaxedOut)
                    {   
                label.text = FormatNumbers.FormatValueString(Lood.UpgradeManager.Instance.Upgrades[upgradeData.Tag].Shop.CurrentOwned, FormatAs);
                    break;
                    }
                label.text = FormatNumbers.FormatValueString(Lood.UpgradeManager.Instance.Upgrades[upgradeData.Tag].Shop.CurrentOwned, FormatAs);
                break;
            case UpgradeTrackValue.CurrentModifier:
                label.text = FormatNumbers.FormatValueString(Lood.UpgradeManager.Instance.Upgrades[upgradeData.Tag].CurrentModifier, FormatAs);
                break;
            case UpgradeTrackValue.NextPrice:
                 
                 if (Lood.UpgradeManager.Instance.Upgrades[upgradeData.Tag].Shop.MaxedOut)
                    {   
                    label.text = "";
                    break;
                    }
                label.text = FormatNumbers.FormatValueString(Lood.UpgradeManager.Instance.Upgrades[upgradeData.Tag].Shop.NextPrice, FormatAs);
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

