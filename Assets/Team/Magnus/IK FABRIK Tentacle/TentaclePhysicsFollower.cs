using UnityEngine;

public class TentaclePhysicsFollower : MonoBehaviour
{
    [Header("Sprite bones (källa)")]
    public Transform[] visualBones; 

    [Header("Physics bones (mål)")]
    public Transform[] physicsBones; 

    private void LateUpdate()
    {
        int count = Mathf.Min(visualBones.Length, physicsBones.Length);
        for (int i = 0; i < count; i++)
        {
            if (visualBones[i] == null || physicsBones[i] == null) continue;

            physicsBones[i].position = visualBones[i].position;
            physicsBones[i].rotation = visualBones[i].rotation;
        }
    }
}
