using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioDatabase", menuName = "Audio/Audio Database")]
public class AudioDatabase : ScriptableObject
{
    public List<Sound> sounds = new List<Sound>();
}
