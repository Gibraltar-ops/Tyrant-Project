using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

public class StatsUIHoverEffect1 : MonoBehaviour
{
    public Button backToMainMenu;

    [Header("Color Settings")]
    public Color hoverColor = Color.red;
    public float colorFadeDuration = 0.8f;

    private Dictionary<Transform, Color> originalColors = new Dictionary<Transform, Color>();

    private void Start()
    {
        SetupHover(backToMainMenu);
    }

    void SetupHover(Button button)
    {
        if (button == null) return;

        TextMeshProUGUI btnText = button.GetComponentInChildren<TextMeshProUGUI>();

        if (btnText != null)
        {
            originalColors[btnText.transform] = btnText.color;
        }
        else
        {
            Debug.LogWarning($"Ingen TextMeshProUGUI hittades på {button.name}.");
        }

        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>() ?? button.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entryEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        entryEnter.callback.AddListener((data) => { OnHoverEnter(button); }); // Pass the whole button
        trigger.triggers.Add(entryEnter);

        EventTrigger.Entry entryExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        entryExit.callback.AddListener((data) => { OnHoverExit(button); }); // Pass the whole button
        trigger.triggers.Add(entryExit);
    }

    void OnHoverEnter(Button button)
    {
        TextMeshProUGUI txt = button.GetComponentInChildren<TextMeshProUGUI>();
        if (txt != null)
        {
            txt.transform.DOKill();
            txt.DOKill();
            txt.transform.DOScale(1.2f, 0.2f).SetEase(DG.Tweening.Ease.OutBounce);
            txt.DOColor(hoverColor, colorFadeDuration);
        }
    }

    void OnHoverExit(Button button)
    {
        TextMeshProUGUI txt = button.GetComponentInChildren<TextMeshProUGUI>();
        if (txt != null && originalColors.ContainsKey(txt.transform))
        {
            txt.transform.DOKill();
            txt.DOKill();
            txt.transform.DOScale(1f, 0.2f);
            txt.DOColor(originalColors[txt.transform], 0.2f);
        }
    }
}