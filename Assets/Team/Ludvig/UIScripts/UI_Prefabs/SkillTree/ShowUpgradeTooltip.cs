using UnityEngine;
using UnityEngine.EventSystems;

public class ShowUpgradeTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject tooltipPanel;

    public void OnPointerEnter(PointerEventData eventData)
    {
        CancelInvoke();
        RectTransform tooltipTransform = tooltipPanel.GetComponent<RectTransform>();
        tooltipTransform.position = new Vector3(GetComponent<RectTransform>().position.x - 100, tooltipTransform.position.y);
        tooltipPanel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        RectTransform rTransform = GetComponent<RectTransform>();
        if (eventData.position.x <= rTransform.position.x && eventData.position.y <= rTransform.position.y + 50 && eventData.position.y >= rTransform.position.y - 50)
        {
            Invoke("SetInactive", 0.5f);
            return;
        }
        tooltipPanel.SetActive(false);

    }
    private void SetInactive()
    {
        tooltipPanel.GetComponent<TooltipMouseOver>().TryClose();
    }

}
