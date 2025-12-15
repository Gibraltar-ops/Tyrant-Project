using UnityEngine;
using UnityEngine.Audio;

public class AudioManagerExample : MonoBehaviour
{
    public static AudioManagerExample instance;
    void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
    }
    public void CreateSound(AudioClip audioClip, float volume = 0.6f, AudioMixerGroup audioMixerGroup = null, bool randomPitch = false)
    {
        GameObject soundToCreate = new GameObject("soundEffect");
        soundToCreate.SetActive(false);
        AudioSource audioSource = soundToCreate.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        if (audioMixerGroup)
        {
            audioSource.outputAudioMixerGroup = audioMixerGroup;
        }
        if (randomPitch)
        {
            audioSource.pitch = Random.Range(0.8f, 1.2f);
        }
        soundToCreate.SetActive(true);
        Destroy(soundToCreate, audioSource.clip.length);
    }
    public void CreateSoundWithRandomPith(AudioClip audioClip, float volume = 0.6f, float pitchRange = 0.1f, AudioMixerGroup audioMixerGroup = null)
    {
        GameObject soundToCreate = new GameObject("soundEffect");
        soundToCreate.SetActive(false);
        AudioSource audioSource = soundToCreate.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.pitch = audioSource.pitch + Random.Range(-pitchRange, pitchRange);
        if (audioMixerGroup)
        {
            audioSource.outputAudioMixerGroup = audioMixerGroup;
        }
        soundToCreate.SetActive(true);
        Destroy(soundToCreate, audioSource.clip.length);
    }
}

public class SoundManager : MonoBehaviour
{
    
    
    
    
}
