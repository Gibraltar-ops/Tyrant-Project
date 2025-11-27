using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipPanel : MonoBehaviour
{
    public Tooltip Tooltip;

    public TMP_Text HeadlineLabel;
    public TMP_Text DescriptionLabel;

    public TMP_Text CurrentEffectLabel;
    public TMP_Text LoreLabel;

    public void SetTooltip(Tooltip tooltip)
    {
        Tooltip = tooltip;
        UpdateFields();
    }

    public void UpdateFields()
    {
        HeadlineLabel.text = Tooltip.Headline;
        DescriptionLabel.text = Tooltip.Description;
        //CurrentEffectLabel.text = Tooltip.CurrentEffect;
        LoreLabel.text = Tooltip.Lore;
        LayoutRebuilder.MarkLayoutForRebuild(GetComponent<RectTransform>());

    }
   
}
