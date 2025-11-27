using TMPro;
using UnityEngine;

public class UIHeadlineGroup : MonoBehaviour
{
    public string HeadlineText;
    public TMP_Text HeadlineTextPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void SetFields(string headlineText)
    {
        HeadlineText = headlineText;
        HeadlineTextPanel.text = headlineText;
    }

    // Update is called once per frame

}
