using UnityEngine;
using UnityEngine.UI;

public class UIContentRebuilder : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        SendMessageUpwards("UpdateUI", SendMessageOptions.DontRequireReceiver);
    }
    void OnEnable()
    {
        SendMessageUpwards("UpdateUI", SendMessageOptions.DontRequireReceiver);
       
    }
    
    void OnDisable()
    {
        SendMessageUpwards("UpdateUI",SendMessageOptions.DontRequireReceiver);
    }

}
