using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Database")]
    public SoundDatabase database;

    [Header("Mixer Routing")]
    public AudioMixerGroup sfxGroup;
    public AudioMixerGroup musicGroup;
    public AudioMixerGroup ambienceGroup;
    public AudioMixerGroup uiGroup;

    [Header("Pooling")]
    public int poolSize = 32;
    private List<AudioSource> pool;

    private Dictionary<string, Sound> soundLookup;

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
    }

    // ------------------- API -------------------- //

    public void Play(string id)
    {
        if (!soundLookup.TryGetValue(id, out var s))
        {
            Debug.LogWarning("Missing sound: " + id);
            return;
        }

        if (Time.time - s.lastPlayed < s.minDelay)
            return;

        if (s.limitInstances && CountActiveInstances(s) >= s.maxInstances)
            return;

        var src = GetFreeSource();
        if (src == null) return;

        var clip = s.clips[Random.Range(0, s.clips.Length)];

        src.clip = clip;
        src.loop = s.looping;
        src.volume = s.volume;
        src.pitch = Random.Range(s.pitchRange.x, s.pitchRange.y);
        src.outputAudioMixerGroup = GetMixerGroup(s.category);

        src.Play();
        s.lastPlayed = Time.time;
    }

    public void Stop(string id)
    {
        if (!soundLookup.TryGetValue(id, out var s)) return;

        foreach (var src in pool)
            if (src.isPlaying && src.clip != null && s.clips.Contains(src.clip))
                src.Stop();
    }

    // ---------------- Helper Methods ---------------- //

    private AudioSource GetFreeSource()
    {
        foreach (var src in pool)
            if (!src.isPlaying)
                return src;
        return null;
    }

    private int CountActiveInstances(Sound s)
    {
        int count = 0;
        foreach (var src in pool)
        {
            if (!src.isPlaying || src.clip == null) continue;
            if (System.Array.Exists(s.clips, c => c == src.clip))
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
}
