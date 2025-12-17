using System.Collections;
using Lood;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShowUpgradeTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector]
    public GameObject tooltipPanel;
    private PurchaseableSO upgradeData;
    private UpgradeCategory upgradeCategory = UpgradeCategory.Upgrade;
    void Awake()
    {
        if (TryGetComponent(out UpgradeShopButton UshopButton))
        {
            upgradeData = UshopButton.upgradeData;
            upgradeCategory = UpgradeCategory.Upgrade;
        }

        if (TryGetComponent(out CurrencySourceShopButton CSshopButton))
        {
            upgradeData = CSshopButton.CurrencySourceData;
            upgradeCategory = UpgradeCategory.Collector;
        }



    }
    void OnEnable()
    {
        if (upgradeCategory == UpgradeCategory.Upgrade) tooltipPanel = Lood.UpgradeManager.Instance.UpgradeTooltipPanel;

        if (upgradeCategory == UpgradeCategory.Collector) tooltipPanel = Lood.UpgradeManager.Instance.UpgradeableTooltipPanel;
    }

    void Start()
    {
        if (upgradeCategory == UpgradeCategory.Upgrade) tooltipPanel = Lood.UpgradeManager.Instance.UpgradeTooltipPanel;

        if (upgradeCategory == UpgradeCategory.Collector) tooltipPanel = Lood.UpgradeManager.Instance.UpgradeableTooltipPanel;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        CancelInvoke();

        //Debug.Log(height);

        if (upgradeCategory == UpgradeCategory.Upgrade)
        {
            tooltipPanel.GetComponent<UIUpgradeTooltip>().Data = upgradeData;
            tooltipPanel.GetComponent<UIUpgradeTooltip>().shopButton = GetComponent<UpgradeShopButton>();
            tooltipPanel.GetComponent<UIUpgradeTooltip>().buttonHierarchy = GetComponent<ButtonHierarchy>();
        }

        if (upgradeCategory == UpgradeCategory.Collector)
        {
            tooltipPanel.GetComponent<UIUpgradeableTooltip>().Data = upgradeData;
            tooltipPanel.GetComponent<UIUpgradeableTooltip>().shopButton = GetComponent<CurrencySourceShopButton>();
            tooltipPanel.GetComponent<UIUpgradeableTooltip>().buttonHierarchy = GetComponent<ButtonHierarchy>();
        }

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
        // Wait until the end of the frame
        yield return new WaitForEndOfFrame();

        RectTransform TooltipRect = tooltipPanel.GetComponent<RectTransform>();
        RectTransform canvasRect = TooltipRect.root.GetComponent<RectTransform>();

        // Convert desired position into canvas space
        Vector2 desiredPosition;
        // Or, if you want to follow another element:
        Vector2 halfSize = new Vector2(TooltipRect.rect.width / 2f * TooltipRect.localScale.x, 0f);
        Vector2 offset = new Vector2(((RectTransform)transform).rect.width / 2f * TooltipRect.localScale.x, 0f);
        desiredPosition = GetComponent<RectTransform>().anchoredPosition - halfSize - offset;

        TooltipRect.anchoredPosition = ClampToCanvas(TooltipRect, canvasRect, desiredPosition);
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
