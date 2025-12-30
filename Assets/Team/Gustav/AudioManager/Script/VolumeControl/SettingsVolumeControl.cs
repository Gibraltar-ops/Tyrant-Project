using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using Unity.VisualScripting;

public class SettingsVolumeControl : MonoBehaviour
{

    [Header("Audio Components")]
    [SerializeField] private AudioMixer mainMixer;

    [UnitHeaderInspectable("UI Sliders")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider uiSlider;
    [SerializeField] private Slider ambianceSlider;

    void OnEnable()
    {
        InitializeSlider("MusicVolume", musicSlider);
        InitializeSlider("SFXVolume", sfxSlider);
        InitializeSlider("UIVolume", uiSlider);
        InitializeSlider("AmbianceVolume", ambianceSlider);
    }

    private void InitializeSlider(string parameterName, Slider slider)
    {

        if (mainMixer.GetFloat(parameterName, out float dbValue))
        {
            slider.value = Mathf.Pow(10, dbValue / 20);
        }
    }

    public void OnMusicVolumeChanged(float value)
    {
        AudioManager.Instance.SetVolume("MusicVolume", value);
    }

    public void OnSFXVolumeChanged(float value)
    {
        AudioManager.Instance.SetVolume("SFXVolume", value);
    }

    public void OnUIVolumeChanged(float value)
    {
        AudioManager.Instance.SetVolume("UIVolume", value);
    }

    public void OnAmbianceVolumeChanged(float value)
    {
        AudioManager.Instance.SetVolume("AmbianceVolume", value);
    }
}
