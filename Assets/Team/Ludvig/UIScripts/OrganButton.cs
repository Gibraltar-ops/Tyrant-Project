using UnityEngine;
using UnityEngine.UI;

public class OrganButton : MonoBehaviour
{
    public Organ Organ;

    public delegate void OnButtonClick(Organ organ);
    public static event OnButtonClick onButtonClick;
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(ToggleMenu);
        
    }

    public void ToggleMenu()
    {
        onButtonClick?.Invoke(Organ);
    }
    
}
