
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIItemPanelInfo : MonoBehaviour
{
   public TMP_Text ItemName;
   public TMP_Text CurrentCount;
   public TMP_Text Price;

   public Button PurchaseButton;

    void OnEnable()
    {
        transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.3f, 1);
    }
}
