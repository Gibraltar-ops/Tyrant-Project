using UnityEngine;

[CreateAssetMenu(fileName = "UISpriteSetSO", menuName = "Scriptable Objects/UISpriteSetSO")]
public class UISpriteSetSO : ScriptableObject
{
    [Header("Upgrade Sprites")]
    public Sprite TreeButtonDefaultSprite;
    public Sprite TreeButtonInactiveSprite;
    public Sprite TreeButtonHoverSprite;

    public Sprite TreeButtonPressedSprite;

    [Header("Tooltip Sprites")]
    public Sprite TooltipButtonDefaultSprite;
    public Sprite TooltipButtonInactiveSprite;
    public Sprite TooltipButtonHoverSprite;
    public Sprite TooltipButtonPressedSprite;
}
