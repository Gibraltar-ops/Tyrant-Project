using UnityEngine;

public class EventSpawn : MonoBehaviour
{
    public bool HasSpawned = false;

    public void SpawnUI()
    {
        if (!HasSpawned)
        {
            GetComponent<Shop>().CreateStoreItem();
            HasSpawned = true;
        }
        
    }

}
