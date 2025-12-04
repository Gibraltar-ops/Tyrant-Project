
using UnityEngine;

[CreateAssetMenu(fileName = "Tooltip", menuName = "Scriptable Objects/Tooltip")]
public class Tooltip : ScriptableObject
{
    [Header("Tooltip Texts")]
    public string Headline;
    [TextArea(2, 3)]public string Description;
    [TextArea(3, 10)] public string CurrentEffect;
    [TextArea(3, 10)] public string Lore;
    public string ModifierValue;
}

public class TooltipData
{
    public string Headline;
    public string Description;
    public string CurrentEffect;
    public string Lore;
    public string ModifierValue;

    public TooltipData(Tooltip tooltip, string modifierValue)
    {
        Headline = tooltip.Headline;
        Description = tooltip.Description;
        CurrentEffect = tooltip.CurrentEffect;
        Lore = tooltip.Lore;
        ModifierValue = modifierValue;
        
    }
}
