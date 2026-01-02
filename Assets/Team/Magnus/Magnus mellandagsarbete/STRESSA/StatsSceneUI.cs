using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class StatsSceneUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text statsDisplayBuffer;
    [SerializeField] private Button menuButton;

    [Header("Animation Settings")]
    [SerializeField] private float numberCountDuration = 1.5f;
    [SerializeField] private float textRevealDuration = 2.0f;
    [SerializeField] private DG.Tweening.Ease numberEase = DG.Tweening.Ease.OutQuad;

    private void Start()
    {
        if (menuButton != null)
        {
            menuButton.gameObject.SetActive(false);
            menuButton.onClick.AddListener(BackToMenu);
        }

        if (statsDisplayBuffer != null)
        {
            statsDisplayBuffer.text = "";
        }

        if (EatManager.Instance != null)
        {
            StartCoroutine(AnimateStats());
        }
    }

    private IEnumerator AnimateStats()
    {
        long finalHumansConsumed = EatManager.Instance.totalHumansConsumed;
        float currentCount = 0;


        Tween numberTween = DOTween.To(() => currentCount, x => currentCount = x, (float)finalHumansConsumed, numberCountDuration)
            .SetEase(numberEase)
            .OnUpdate(() =>
            {
                statsDisplayBuffer.text = $"You consumed {(long)currentCount} humans.";
            });

        yield return numberTween.WaitForCompletion();

        yield return new WaitForSeconds(0.8f);

        yield return statsDisplayBuffer.DOFade(0, 0.3f).WaitForCompletion();

        string finalMessage = EatManager.Instance.GetStatsSummary();
        statsDisplayBuffer.text = finalMessage;
        statsDisplayBuffer.maxVisibleCharacters = 0;

        statsDisplayBuffer.alpha = 1;
        int totalCharacters = finalMessage.Length;

        yield return DOTween.To(() => statsDisplayBuffer.maxVisibleCharacters,
            x => statsDisplayBuffer.maxVisibleCharacters = x,
            totalCharacters,
            textRevealDuration)
            .SetEase(DG.Tweening.Ease.OutCubic)
            .WaitForCompletion();


        if (menuButton != null)
        {
            menuButton.gameObject.SetActive(true);
            CanvasGroup btnCanvasGroup = menuButton.GetComponent<CanvasGroup>();
            if (btnCanvasGroup == null) btnCanvasGroup = menuButton.gameObject.AddComponent<CanvasGroup>();

            btnCanvasGroup.alpha = 0;
            btnCanvasGroup.DOFade(1, 0.5f);
        }
    }

    private void BackToMenu()
    {
        if (EatManager.Instance != null) EatManager.Instance.ResetStats();
        if (TransitionManager.Instance != null) TransitionManager.Instance.TransitionTo("MainMenuPlaytest");
    }
}