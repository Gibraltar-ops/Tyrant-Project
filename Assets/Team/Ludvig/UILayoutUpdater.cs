using UnityEngine;
using UnityEngine.UI;

public class UILayoutUpdater : MonoBehaviour
{
    void Start()
    {
        UpdateUI();
    } // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void UpdateUI()
    {
        var componentsInChildren = transform.GetComponentsInChildren<LayoutGroup>(true);
        foreach (var layoutGroup in componentsInChildren)
            {
        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
            }

        var parent = transform.GetComponent<LayoutGroup>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(parent.GetComponent<RectTransform>());
         
    }


}
