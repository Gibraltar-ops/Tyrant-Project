using UnityEngine;
using UnityEngine.UI;

public class ExtrasUI : MonoBehaviour
{
    [Header("UI Buttons")]
    [SerializeField] private Button btn_Back;

    [Header("Submenu Panels")]
    [SerializeField] private GameObject canvas_MainMenu;
    [SerializeField] private GameObject canvas_Credits;

    private void Start()
    {
        // Initialize button listeners
        btn_Back.onClick.AddListener(OnBackClicked);
    }

    #region Button Handlers
    private void OnBackClicked()
    {
        Debug.Log("Return to Main Menu");
        canvas_Credits.SetActive(false);
        canvas_MainMenu.SetActive(true);
    }
    #endregion
}
