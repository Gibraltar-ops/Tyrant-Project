using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundDatabase", menuName = "Audio/Sound Database")]
public class SoundDatabase : ScriptableObject
{
    public List<Sound> sounds = new List<Sound>();
}
