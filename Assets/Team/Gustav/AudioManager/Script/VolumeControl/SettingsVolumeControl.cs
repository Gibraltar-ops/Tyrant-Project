using UnityEngine;
using UnityEngine.UI;

public class SettingsVolumeControl : MonoBehaviour
{
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
