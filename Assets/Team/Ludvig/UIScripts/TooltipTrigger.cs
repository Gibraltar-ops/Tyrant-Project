
using UnityEngine;
using UnityEngine.EventSystems;
public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //This script lives on all UI shop items. It sends messages upwards to trigger showing and hiding of the tooltip panel
    //and sending the correct tooltip info.
    public Tooltip Tooltip;

    public void OnPointerEnter(PointerEventData eventData)
    {
        SendMessageUpwards("ShowTooltipPanel", Tooltip, SendMessageOptions.DontRequireReceiver);
        //Debug.Log("PointerEntered");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SendMessageUpwards("HideTooltipPanel", SendMessageOptions.DontRequireReceiver);
    }

}
