using TMPro;
using UnityEngine;

public class UISubcategoryInfo : MonoBehaviour
{
    public UpgradeCategory category;
    public string HeadlineText;
    public TMP_Text HeadlineTextPanel;
    public GameObject ShopPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void SetFields(string headlineText)
    {
        HeadlineText = headlineText;
        HeadlineTextPanel.text = headlineText;
    }

    // Update is called once per frame

}
