using System.Collections.Generic;
using Lood;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonHierarchy : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Button button;

    public List<GameObject> ButtonParents;
    public TMP_Text countLabel;

    public int ActiveThreshold = 1;
    public int Remainder = 1;

    private int _parentCount = 0;

    public bool Active = false;
    public UnityEvent onActivate;


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
        Remainder = ActiveThreshold;
        CheckActive();
    }


    public void CheckActive()
    {
        if (Active)
        {
            return;
        }
        if(ButtonParents == null)
        {
            Active = true;
            onActivate.Invoke();
            return;
        }

        foreach (GameObject parent in ButtonParents)
        {
            if(parent.GetComponentInChildren<CurrencySourceShopButton>() != null)
            {   
                var currencySource = parent.GetComponentInChildren<CurrencySourceShopButton>();
                _parentCount += Lood.UpgradeManager.Instance.GetCurrencySourceCount(currencySource.currencySourceTag);
            }
            if(parent.GetComponentInChildren<UpgradeShopButton>() != null)
            {   
                var upgrade = parent.GetComponentInChildren<UpgradeShopButton>();
                _parentCount += Lood.UpgradeManager.Instance.GetUpgradeCount(upgrade.upgradeTag);
            }
        }
        if(_parentCount == 0)
        {
            countLabel.text = "-";
        }
        if(_parentCount < ActiveThreshold)
        {
            countLabel.text = Remainder.ToString();
            Active = false;
            Remainder = ActiveThreshold - _parentCount;
            _parentCount = 0;
            return;
        }
        else
        {
            Active = true;
            onActivate.Invoke();
            return;
        }

        
    }

    // public void CreateLine(Vector2 start, Vector2 end)
    // {
    // GameObject line = new GameObject("UILine", typeof(Image));
    // line.transform.SetParent(container, false);

    // Image image = line.GetComponent<Image>();
    // image.color = lineColor;

    // Vector2 direction = end - start;
    // float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    // float length = direction.magnitude;

    // RectTransform rectTransform = line.GetComponent<RectTransform>();
    // rectTransform.sizeDelta = new Vector2(length, 5); // Adjust thickness here
    // rectTransform.pivot = new Vector2(0, 0.5f);
    // rectTransform.anchoredPosition = start;
    // rectTransform.rotation = Quaternion.Euler(0, 0, angle);
    // }
    
}
