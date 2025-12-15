using UnityEngine;

[System.Serializable]
public class Sound
{
    public string id;

    [Header("Clips & Variations")]
    public AudioClip[] clips;

    [Range(0f, 1f)]
    public float volume = 1f;

    [Tooltip("Random pitch range per play")]
    public Vector2 pitchRange = new Vector2(0.95f, 1.05f);

    [Header("Playback Controls")]
    public bool looping = false;
    public bool limitInstances = false;
    public int maxInstances = 3;

    [Tooltip("Minimum time between plays")]
    public float minDelay = 0.05f;

    [HideInInspector] public float lastPlayed;

    [Header("Routing")]
    public AudioCategory category;
}
