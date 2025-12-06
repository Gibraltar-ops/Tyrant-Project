
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
public class MenuButtonArea : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //This script lives on all UI shop items. It sends messages upwards to trigger showing and hiding of the tooltip panel
    //and sending the correct tooltip info.
    


    public void OnPointerEnter(PointerEventData eventData)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
        //Debug.Log("PointerEntered");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

}
