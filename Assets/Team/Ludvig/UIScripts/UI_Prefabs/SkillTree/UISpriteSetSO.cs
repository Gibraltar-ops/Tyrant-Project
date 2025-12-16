using UnityEngine;

[CreateAssetMenu(fileName = "UIColorStyleSO", menuName = "Scriptable Objects/UIColorStyleSO")]
public class UIColorStyleSO : ScriptableObject
{
    public string Tag;
    public Sprite CurrencyIcon;
    public Color MainColor;
    public Color SecondaryColor;

    public UISpriteSetSO SpriteSet;


}
