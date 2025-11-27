using UnityEngine;


public class TooltipShowHide : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject TooltipPanel;
    void Start()
    {
        TooltipPanel.SetActive(false);
    }
    //Triggered by messages from children when the mouse enters them.
    public void ShowTooltipPanel(Tooltip tooltip)
    {
        TooltipPanel.SetActive(true);

        var panel = TooltipPanel.GetComponent<TooltipPanel>();
        panel.SetTooltip(tooltip);
        panel.UpdateFields();
    }
    // Triggered by messages from children when the mouse leaves them.
    void HideTooltipPanel()
    {
        TooltipPanel.SetActive(false);
    }
}
