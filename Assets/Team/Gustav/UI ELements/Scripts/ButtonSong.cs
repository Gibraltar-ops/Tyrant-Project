using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSong : MonoBehaviour
{
    private VictimSystem victimSystem;

    [Header("References")]
    public Slider slider1;
    public Slider slider2;

    [Header("Settings")]
    public float fillSpeed = 0.5f;
    public float drainSpeed = 0.1f;

    public bool isPressed = false;

    void Start()
    {
        victimSystem = FindAnyObjectByType<VictimSystem>();
    }

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

        if (victimSystem != null)
        {
            // slider goes from 0–1, we convert it to 1–5
            victimSystem.spawnRateMultiplier = Mathf.Lerp(1f, 5f, slider1.value);
        }
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
