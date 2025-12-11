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
    public float fillSpeed = 0.5f; // Speed at which the slider fills when pressed
    public float drainSpeed = 0.1f; // Speed at which the slider drains when not pressed
    public float minSpawnRateMultiplier = 1f; // Minimal spawn rate multiplier (Lerps between min/max based on slider 0%->100%)
    public float maxSpawnRateMultiplier = 5f; // Maximal spawn rate multiplier (Lerps between min/max based on slider 0%->100%)

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
            // slider goes from 0–1, it gets converted it to 1–5
            victimSystem.smallWaveAmountMultiplier = Mathf.Lerp(minSpawnRateMultiplier, maxSpawnRateMultiplier, slider1.value);
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
