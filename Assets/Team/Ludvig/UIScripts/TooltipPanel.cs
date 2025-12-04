using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipPanel : MonoBehaviour
{
    public TooltipData TooltipData;

    public TMP_Text HeadlineLabel;
    public TMP_Text DescriptionLabel;

    public TMP_Text CurrentEffectLabel;
    public TMP_Text LoreLabel;

    public void SetTooltip(TooltipData tooltip)
    {
        TooltipData = tooltip;
        UpdateFields();
    }

    public void UpdateFields()
    {
        HeadlineLabel.text = TooltipData.Headline;
        DescriptionLabel.text = TooltipData.Description;
        //CurrentEffectLabel.text = Tooltip.CurrentEffect;
        LoreLabel.text = TooltipData.Lore;
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());

    }
   
}
