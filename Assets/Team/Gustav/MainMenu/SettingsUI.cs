using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [Header("UI Buttons")]
    [SerializeField] private Button btn_Button1;
    [SerializeField] private Button btn_Button2;
    [SerializeField] private Button btn_Button3;
    [SerializeField] private Button btn_Button4;
    [SerializeField] private Button btn_Back;

    [Header("Submenu Panels")]
    [SerializeField] private GameObject canvas_MainMenu;
    [SerializeField] private GameObject canvas_Settings;

    private void Start()
    {
        // Initialize button listeners
        btn_Button1.onClick.AddListener(OnButton1Clicked);
        btn_Button2.onClick.AddListener(OnButton2Clicked);
        btn_Button3.onClick.AddListener(OnButton3Clicked);
        btn_Button4.onClick.AddListener(OnButton4Clicked);
        btn_Back.onClick.AddListener(OnBackClicked);
    }

    #region Button Handlers
    private void OnButton1Clicked()
    {
        Debug.Log("Button1 Clicked");
    }
    private void OnButton2Clicked()
    {
        Debug.Log("Button2 Clicked");
    }
    private void OnButton3Clicked()
    {
        Debug.Log("Button4 Clicked");
    }
    private void OnButton4Clicked()
    {
        Debug.Log("Button4 Clicked");
    }
    private void OnBackClicked()
    {
        Debug.Log("Return to Main Menu");
        canvas_Settings.SetActive(false);
        canvas_MainMenu.SetActive(true);
    }
    #endregion
}
