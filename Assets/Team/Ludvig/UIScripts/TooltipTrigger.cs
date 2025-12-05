
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //This script lives on all UI shop items. It sends messages upwards to trigger showing and hiding of the tooltip panel
    //and sending the correct tooltip info.
    public Tooltip Tooltip;
    private bool _playerHasInteracted = false;
    private TooltipData _tooltipData;

    public void InitializeTooltipData(string value)
    {
        _tooltipData = new TooltipData(Tooltip, value);
    }

    public void UpdateTooltip(string modifierValue)
    {
        _tooltipData.ModifierValue = modifierValue;
        SendMessageUpwards("ShowTooltipPanel", _tooltipData, SendMessageOptions.DontRequireReceiver);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<UIItemPanelInfo>().PurchaseButton.transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.2f);
        SendMessageUpwards("ShowTooltipPanel", _tooltipData, SendMessageOptions.DontRequireReceiver);

        if (!_playerHasInteracted)
        {
            GetComponent<UIItemPanelInfo>().NewItemIcon.enabled = false;
            _playerHasInteracted = true;
        }
        //Debug.Log("PointerEntered");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<UIItemPanelInfo>().PurchaseButton.transform.DOScale(new Vector3(1f, 1f, 1f), 0.2f);
        SendMessageUpwards("HideTooltipPanel", SendMessageOptions.DontRequireReceiver);
    }

}

