using System.Collections.Generic;
using DG.Tweening;
using Lood;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class ButtonHierarchy : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Button button;
    public Color ActiveParentLineColor = Color.white;
    public Color InactiveParentLineColor = Color.black;
    public GameObject Lock;
    private UILineRenderer lineRenderer;

    public List<GameObject> ButtonParents;
    public TMP_Text countLabel;

    public int ActiveThreshold = 1;
    public int Remainder = 1;

    private int _parentCount = 0;

    public bool Active = false;
    public UnityEvent onActivate;
    private List<UILineRendererLine> _lines = new();

    void Awake()
    {
        Remainder = ActiveThreshold;
    }

    public void AddLines()
    {
        lineRenderer = transform.parent.GetComponentInChildren<UILineRenderer>();
        if (ButtonParents.Count != 0)
        {
            foreach (var parent in ButtonParents)
            {
                UILineRendererLine line = new UILineRendererLine(GetComponent<RectTransform>(), parent.GetComponent<RectTransform>(), InactiveParentLineColor, false);
                lineRenderer.AddLine(line);
                _lines.Add(line);
                Debug.DrawLine(parent.transform.position, transform.position, Color.red);
            }
        }
    }

    void OnDrawGizmos()
    {
        foreach (var parent in ButtonParents)
        {
            Debug.DrawLine(parent.transform.position, transform.position, Color.red);
        }
    }

    void OnEnable()
    {
        Upgrader.onUpgradeApplied += CheckActive;
    }
    void OnDisable()
    {
        Upgrader.onUpgradeApplied -= CheckActive;
    }
    void Start()
    {

        Invoke("CheckActive", 0.1f);
    }


    void Update()
    {
        foreach (var parent in ButtonParents)
        {
            Debug.DrawLine(parent.transform.position, transform.position, Color.red);
        }

    }

    public void CheckActive()
    {
        if (Active)
        {
            return;
        }
        if (ButtonParents == null)
        {
            Active = true;
            onActivate.Invoke();
            return;
        }

        foreach (GameObject parent in ButtonParents)
        {
            if (parent.GetComponent<CurrencySourceShopButton>() != null)
            {
                var currencySource = parent.GetComponentInChildren<CurrencySourceShopButton>();
                _parentCount += Lood.UpgradeManager.Instance.GetCurrencySourceCount(currencySource.CurrencySourceData.Tag);
            }
            if (parent.GetComponent<UpgradeShopButton>() != null)
            {
                var upgrade = parent.GetComponent<UpgradeShopButton>();
                _parentCount += Lood.UpgradeManager.Instance.GetUpgradeCount(upgrade.upgradeData.Tag);
            }
        }
        if (_parentCount == 0)
        {
            Lock.SetActive(true);
            //countLabel.text = "-";
        }
        if (_parentCount < ActiveThreshold)
        {
            Lock.SetActive(true);
            Active = false;
            Remainder = ActiveThreshold - _parentCount;
            //countLabel.text = Remainder.ToString();
            _parentCount = 0;
            return;
        }
        else
        {
            Lock.SetActive(false);
            Active = true;
            onActivate.Invoke();
            transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.2f);

            foreach (var line in _lines)
            {
                line.animated = true;
                line.color = ActiveParentLineColor;
            }
            return;
        }


    }

}
