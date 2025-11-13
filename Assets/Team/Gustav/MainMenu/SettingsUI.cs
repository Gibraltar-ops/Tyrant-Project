using UnityEngine;
using UnityEngine.SceneManagement;
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
    [SerializeField] private GameObject canvas_Extras;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
