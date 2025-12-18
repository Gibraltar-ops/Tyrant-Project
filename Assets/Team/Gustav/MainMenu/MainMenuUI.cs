using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;

public class MainMenuUI : MonoBehaviour
{
    [Header("UI Buttons")]
    [SerializeField] private Button btn_Continue;
    [SerializeField] private Button btn_NewGame;
    [SerializeField] private Button btn_Settings;
    [SerializeField] private Button btn_Extras;
    [SerializeField] private Button btn_Credits;
    [SerializeField] private Button btn_Quit;

    [Header("Submenu Panels")]
    [SerializeField] private GameObject canvas_MainMenu;
    [SerializeField] private GameObject canvas_Settings;
    [SerializeField] private GameObject canvas_Extras;
    [SerializeField] private GameObject canvas_Credits;

    [Header("Game Settings")]

    [SerializeField] private string saveFileKey = "SaveExists";   // Used to check if a save exists

    private void Start()
    {
        // Initialize button listeners
        btn_Continue.onClick.AddListener(OnContinueClicked);
        btn_NewGame.onClick.AddListener(OnNewGameClicked);
        btn_Settings.onClick.AddListener(OnSettingsClicked);
        btn_Extras.onClick.AddListener(OnExtrasClicked);
        btn_Credits.onClick.AddListener(OnCreditsClicked);
        btn_Quit.onClick.AddListener(OnQuitClicked);

        SetupMainMenu();



        // Check if a saved game exists and enable/disable Continue button
        //bool hasSave = PlayerPrefs.HasKey(saveFileKey);
        //btn_Continue.interactable = hasSave;

        // Ensure only the main menu is shown on start

    }

    #region Button Handlers

    private void OnContinueClicked()
    {

        // Debug.Log("Continue Game");
        // // Example: Load saved game
        // if (PlayerPrefs.HasKey(saveFileKey))
        // {
        //     string savedScene = PlayerPrefs.GetString("SavedScene", sceneName);
        //     SceneManager.LoadScene(savedScene);
        // }
        // else
        // {
        //     Debug.LogWarning("No save found!");
        // }
    }

    private void OnNewGameClicked()
    {
        string path = Application.persistentDataPath + "/prototypePlayer.fun";
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        Debug.Log("New Game Started");
        // Clear save data if necessary
        PlayerPrefs.DeleteKey(saveFileKey);

        if (TransitionManager.Instance != null)
        {
            TransitionManager.Instance.TransitionTo("NewGameIntro");
        }
        // Load the first level or starting scene

    }

    private void OnSettingsClicked()
    {
        Debug.Log("Open Settings");
        canvas_MainMenu.SetActive(false);
        canvas_Settings.SetActive(true);
    }

    private void OnExtrasClicked()
    {
        Debug.Log("Open Extras");
        canvas_MainMenu.SetActive(false);
        canvas_Extras.SetActive(true);
    }
    private void OnCreditsClicked()
    {
        Debug.Log("Open Extras");
        canvas_MainMenu.SetActive(false);
        canvas_Credits.SetActive(true);
    }

    private void OnQuitClicked()
    {
        Debug.Log("Quit Game");
        Application.Quit();

#if UNITY_EDITOR
        // This only works inside the Unity Editor
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    #endregion

    #region Panel Management

    public void SetupMainMenu()
    {
        canvas_MainMenu.SetActive(true);
        canvas_Settings.SetActive(false);
        canvas_Extras.SetActive(false);
        canvas_Credits.SetActive(false);
    }

    public void CloseAllSubmenus()
    {
        canvas_Settings.SetActive(false);
        canvas_Extras.SetActive(false);
        canvas_Credits.SetActive(false);
    }

    #endregion
}
