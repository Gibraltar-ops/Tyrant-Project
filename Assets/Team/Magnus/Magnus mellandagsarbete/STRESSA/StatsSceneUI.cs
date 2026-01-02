using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StatsSceneUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text statsDisplayBuffer;
    [SerializeField] private Button menuButton;

    private void Start()
    {
        if (EatManager.Instance != null)
        {
            statsDisplayBuffer.text = EatManager.Instance.GetStatsSummary();
        }

        if (menuButton != null)
        {
            menuButton.onClick.AddListener(BackToMenu);
        }
    }

    private void BackToMenu()
    {

        if (EatManager.Instance != null)
        {
            EatManager.Instance.ResetStats();
        }

        if (TransitionManager.Instance != null)
        {
            TransitionManager.Instance.TransitionTo("MainMenuPlaytest");
        }
    }
}