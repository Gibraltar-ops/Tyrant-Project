using UnityEngine;
using System.Linq;
using UnityEngine.Audio;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Database")]
    public AudioDatabase database;

    [Header("Mixer Routing")]
    public AudioMixer mainMixer;
    public AudioMixerGroup musicGroup;
    public AudioMixerGroup ambienceGroup;
    public AudioMixerGroup uiGroup;
    public AudioMixerGroup sfxGroup;

    [Header("Pooling")]
    public int poolSize = 32;
    private List<AudioSource> pool;

    private Dictionary<string, Sound> soundLookup;

    private Dictionary<Sound, float> lastPlayTime = new Dictionary<Sound, float>();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Build lookup
        soundLookup = new Dictionary<string, Sound>();
        foreach (var s in database.sounds)
            soundLookup[s.id] = s;

        // Build pool
        pool = new List<AudioSource>();
        for (int i = 0; i < poolSize; i++)
        {
            var src = gameObject.AddComponent<AudioSource>();
            src.playOnAwake = false;
            pool.Add(src);
        }

        if (database == null)
        {
            Debug.LogError("[Audio] No AudioDatabase assigned!");
            enabled = false;
            return;
        }
    }

    void Start()
    {
        Play("Music");
    }

    // ------------------- API -------------------- //

    public void Play(string id)
    {
        if (!soundLookup.TryGetValue(id, out var s))
        {
            Debug.LogWarning($"[Audio] Sound ID not found: {id}");
            return;
        }

        if (s.clips == null || s.clips.Length == 0)
        {
            Debug.LogWarning($"[Audio] Sound '{id}' has no clips assigned");
            return;
        }

        if (lastPlayTime.TryGetValue(s, out float lastTime))
        {
            if (Time.time - lastTime < s.minDelay)
                return;
        }

        if (s.limitInstances && CountActiveInstances(s) >= s.maxInstances)
            return;

        var src = GetFreeSource();
        if (src == null)
        {
            Debug.Log($"[Audio] Pool exhausted, skipping '{id}'");
            return;
        }

        src.clip = s.clips[Random.Range(0, s.clips.Length)];
        src.volume = s.volume;
        src.pitch = Random.Range(s.pitchRange.x, s.pitchRange.y);
        src.loop = s.looping;
        src.outputAudioMixerGroup = GetMixerGroup(s.category);

        src.Play();
        lastPlayTime[s] = Time.time;
    }

    public void Stop(string id)
    {
        if (!soundLookup.TryGetValue(id, out var sound)) return;

        foreach (var soundSource in pool)
            if (soundSource.isPlaying && soundSource.clip != null && sound.clips.Contains(soundSource.clip))
                soundSource.Stop();
    }

    // ---------------- Helper Methods ---------------- //

    private AudioSource GetFreeSource()
    {
        foreach (var soundSource in pool)
            if (!soundSource.isPlaying)
                return soundSource;
        return null;
    }

    private int CountActiveInstances(Sound s)
    {
        int count = 0;
        foreach (var soundSource in pool)
        {
            if (!soundSource.isPlaying || soundSource.clip == null) continue;
            if (System.Array.Exists(s.clips, c => c == soundSource.clip))
                count++;
        }
        return count;
    }

    private AudioMixerGroup GetMixerGroup(AudioCategory category)
    {
        switch (category)
        {
            case AudioCategory.Music: return musicGroup;
            case AudioCategory.Ambience: return ambienceGroup;
            case AudioCategory.UI: return uiGroup;
            default: return sfxGroup;
        }
    }

    public void SetVolume(string parameterName, float sliderValue)
    {
        float db = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20;
        if (mainMixer != null)
        {
            mainMixer.SetFloat(parameterName, db);
        }
    }
}
