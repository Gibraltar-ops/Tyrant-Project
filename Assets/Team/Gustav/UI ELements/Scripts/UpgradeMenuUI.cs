using UnityEngine;
using UnityEngine.UI;

public class UpgradeMenuUI : MonoBehaviour
{
    [Header("UI Buttons")]
    [SerializeField] private Button btn_Button1;

    [Header("Submenu Panels")]
    [SerializeField] private GameObject panel_UpgradeMenu;

    private bool isUpgradeMenuActive = false;

    void Start()
    {
        // Initialize button listeners
        btn_Button1.onClick.AddListener(OnButton1Clicked);
    }

    private void OnButton1Clicked()
    {
        panel_UpgradeMenu.SetActive(!isUpgradeMenuActive);
        isUpgradeMenuActive = !isUpgradeMenuActive;
    }
}
