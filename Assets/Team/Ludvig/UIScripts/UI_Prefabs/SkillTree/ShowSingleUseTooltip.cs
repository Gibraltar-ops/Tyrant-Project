using System.Collections;
using Lood;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShowSingleUseTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector]
    public GameObject tooltipPanel;
    private PurchaseableSO upgradeData;
    private UpgradeCategory upgradeCategory = UpgradeCategory.Upgrade;
    void Awake()
    {
        upgradeData = GetComponent<CurrencySourceShopButton>().CurrencySourceData;
        upgradeCategory = UpgradeCategory.Collector;
    }

    void OnEnable()
    {
        tooltipPanel = Lood.UpgradeManager.Instance.SinglePurchaseTooltip;
    }

    void Start()
    {
        tooltipPanel = Lood.UpgradeManager.Instance.SinglePurchaseTooltip;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        CancelInvoke();

        //Debug.Log(height);

        tooltipPanel.GetComponent<UISinglePurchaseTooltip>().Data = upgradeData;
        tooltipPanel.GetComponent<UISinglePurchaseTooltip>().shopButton = GetComponent<CurrencySourceShopButton>();
        tooltipPanel.GetComponent<UISinglePurchaseTooltip>().buttonHierarchy = GetComponent<ButtonHierarchy>();


        tooltipPanel.SetActive(true);

        RectTransform tooltipRect = tooltipPanel.GetComponent<RectTransform>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(tooltipRect);
        var componentsInChildren = tooltipRect.GetComponentsInChildren<RectTransform>();
        foreach (var layoutGroup in componentsInChildren)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup);
        }

        StartCoroutine(SetTooltipPosition());


    }

    IEnumerator SetTooltipPosition()
    {
        yield return new WaitForEndOfFrame();

        RectTransform tooltipRect = tooltipPanel.GetComponent<RectTransform>();
        RectTransform canvasRect = tooltipRect.root.GetComponent<RectTransform>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(tooltipRect);

        RectTransform buttonRect = GetComponent<RectTransform>();

        // Use the utility method
        Vector2 buttonPos = GetLocalPositionRelativeToCanvas(buttonRect, canvasRect);

        // Apply offsets
        Vector2 halfSize = new Vector2(tooltipRect.rect.width / 2f * tooltipRect.localScale.x, 0f);
        Vector2 offset = new Vector2(buttonRect.rect.width / 2f * tooltipRect.localScale.x, 0f);

        Vector2 desiredPosition = buttonPos - halfSize - offset;

        tooltipRect.anchoredPosition = ClampToCanvas(tooltipRect, canvasRect, desiredPosition);
    }


    public Vector2 GetLocalPositionRelativeToCanvas(RectTransform target, RectTransform canvas)
    {
        // Convert world position of target into screen point
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, target.position);

        // Convert screen point into local position of the canvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, screenPoint, null, out Vector2 localPoint);

        return localPoint;
    }



    public Vector2 ClampToCanvas(RectTransform panel, RectTransform canvas, Vector2 targetPos)
    {
        float halfPanelW = panel.rect.width / 2f * panel.localScale.x;
        float halfPanelH = panel.rect.height / 2f * panel.localScale.x;
        float halfCanvasW = canvas.rect.width / 2f;
        float halfCanvasH = canvas.rect.height / 2f;

        float x = Mathf.Clamp(targetPos.x, -halfCanvasW + halfPanelW, halfCanvasW - halfPanelW);
        float y = Mathf.Clamp(targetPos.y, -halfCanvasH + halfPanelH, halfCanvasH - halfPanelH);

        return new Vector2(x, y);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipPanel.SetActive(false);
        return;
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
