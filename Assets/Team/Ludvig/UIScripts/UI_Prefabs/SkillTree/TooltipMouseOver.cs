using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipMouseOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject tooltipPanel;
    public bool IsHovering = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        // IsHovering = true;
        // gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // SetInactive();
    }
    private void SetInactive()
    {
        // IsHovering = false;
        // gameObject.SetActive(false);
    }

    public void TryClose()
    {
        // SetInactive();
        // if (!IsHovering)
        // {
        //     // SetInactive();
        // }
    }
}
