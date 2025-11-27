using UnityEngine;
using UnityEngine.EventSystems;
public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("PointerEntered");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("PointerExited");
    }

}
