using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuitButtonTEMP : MonoBehaviour
{
    [SerializeField] private Button btn_ReturnMainMenu;
    [SerializeField] private Button btn_Quit;

    void Start()
    {
        btn_ReturnMainMenu.onClick.AddListener(OnReturnToMainMenu);
        btn_Quit.onClick.AddListener(OnQuitClicked);
    }
    private void OnReturnToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
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
}
