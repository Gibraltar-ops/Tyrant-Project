using UnityEngine;
using UnityEngine.UI;
public class UI_MenuCloseButton : MonoBehaviour
{
    public Organ Organ;

    public delegate void OnButtonClick();
    public static event OnButtonClick onButtonClick;
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(CloseMenu);
        
    }

    public void CloseMenu()
    {
        onButtonClick?.Invoke();
    }
    
}