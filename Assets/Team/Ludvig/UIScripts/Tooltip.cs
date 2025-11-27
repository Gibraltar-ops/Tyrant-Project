using UnityEngine;

[CreateAssetMenu(fileName = "Tooltip", menuName = "Scriptable Objects/Tooltip")]
public class Tooltip : ScriptableObject
{
    public string Headline;
    [TextArea(2, 3)]public string Description;

    [TextArea(3, 10)] public string CurrentEffect;
    [TextArea(3, 10)] public string Lore;
}
