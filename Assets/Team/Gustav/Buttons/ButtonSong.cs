using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSong : MonoBehaviour
{
    [Header("References")]
    public Slider slider1;
    public Slider slider2;

    [Header("Settings")]
    public float fillSpeed = 0.5f;
    public float drainSpeed = 0.1f;

    public bool isPressed = false;

    void Update()
    {
        if (isPressed)
        {
            // Fill toward 1 (100%).
            slider1.value = Mathf.MoveTowards(slider1.value, 1f, fillSpeed * Time.deltaTime);
            slider2.value = Mathf.MoveTowards(slider2.value, 1f, fillSpeed * Time.deltaTime);
        }
        else
        {
            // Drain toward 0.
            slider1.value = Mathf.MoveTowards(slider1.value, 0f, drainSpeed * Time.deltaTime);
            slider2.value = Mathf.MoveTowards(slider2.value, 0f, drainSpeed * Time.deltaTime);
        }

        //Place code here for sending slider current value or
        //add new method further down for getting the value at will/need.
    }

    // When button is pressed.
    public void OnPointerDown(BaseEventData data)
    {
        isPressed = true;
    }

    // When button is realeased.
    public void OnPointerUp(BaseEventData data)
    {
        isPressed = false;
    }
}
