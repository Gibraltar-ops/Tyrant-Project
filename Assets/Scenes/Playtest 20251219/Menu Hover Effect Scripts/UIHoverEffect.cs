using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;
public class UIHoverEffect : MonoBehaviour
{
    public Button newGameButton;
    public Button creditsButton;
    public Button quitButton;

    [Header("Color Settings")]
    public Color hoverColor = Color.darkRed;
    public float colorFadeDuration = 1.5f;

    private Dictionary<Transform, Color> originalColors = new Dictionary<Transform, Color>();

    private void Start()
    {
        SetupHover(newGameButton);
        SetupHover(creditsButton);
        SetupHover(quitButton);
    }

    void SetupHover(Button button)
    {
        if (button == null) return;

        TextMeshProUGUI btnText = button.GetComponentInChildren<TextMeshProUGUI>();

        if (btnText != null)
        {
            originalColors[button.transform] = btnText.color;
        }
        else
        {
            Debug.LogWarning($"Ingen TextMeshProUGUI hittades på {button.name}. Om du använder vanlig Text, byt 'TextMeshProUGUI' till 'Text' i koden.");
        }

        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null) trigger = button.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((data) => { OnHoverEnter(button.transform); });
        trigger.triggers.Add(entryEnter);

        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((data) => { OnHoverExit(button.transform); });
        trigger.triggers.Add(entryExit);
    }

    void OnHoverEnter(Transform target)
    {

        target.DOKill();
        target.DOScale(1.2f, 0.2f).SetEase(DG.Tweening.Ease.OutBounce);


        TextMeshProUGUI txt = target.GetComponentInChildren<TextMeshProUGUI>();
        if (txt != null)
        {
            txt.DOKill();
            txt.DOColor(hoverColor, colorFadeDuration);
        }
    }

    void OnHoverExit(Transform target)
    {

        target.DOKill();
        target.DOScale(1f, 0.2f);

        TextMeshProUGUI txt = target.GetComponentInChildren<TextMeshProUGUI>();
        if (txt != null && originalColors.ContainsKey(target))
        {
            txt.DOKill();
            txt.DOColor(originalColors[target], 0.2f);
        }
    }
}