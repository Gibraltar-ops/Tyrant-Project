using System.Collections.Generic;
using UnityEngine.Pool;
using UnityEngine;
public class VictimRenderer : MonoBehaviour
{
    public VictimSystem victimSystem;
    public GameObject victimPrefab;

    private IObjectPool<GameObject> pool;

    public int preloadAmount = 1000;

    // All currently active visual objects
    private readonly List<GameObject> activeVisuals = new();

    void Awake()
    {
        pool = new ObjectPool<GameObject>(
            CreateVisual,
            OnTakeFromPool,
            OnReturnedToPool,
            OnDestroyVisual,
            collectionCheck: false,
            defaultCapacity: 100,
            maxSize: 5000
        );
    }

    void LateUpdate()
    {
        SyncActiveCount();
        UpdateVisuals();
    }

    // -------------------------------------------------------
    // Create a pooled visual object
    // -------------------------------------------------------
    GameObject CreateVisual()
    {
        GameObject obj = Instantiate(victimPrefab, transform);
        obj.SetActive(false);
        return obj;
    }

    void OnTakeFromPool(GameObject obj)
    {
        obj.SetActive(true);
    }

    void OnReturnedToPool(GameObject obj)
    {
        obj.SetActive(false);
    }

    void OnDestroyVisual(GameObject obj)
    {
        Destroy(obj);
    }

    // -------------------------------------------------------
    // Make sure active visuals match victim count
    // -------------------------------------------------------
    void SyncActiveCount()
    {
        int simCount = victimSystem.victims.Count;

        // Need more visuals?
        while (activeVisuals.Count < simCount)
        {
            activeVisuals.Add(pool.Get());
        }

        // Too many visuals?
        while (activeVisuals.Count > simCount)
        {
            var last = activeVisuals[^1];
            activeVisuals.RemoveAt(activeVisuals.Count - 1);
            pool.Release(last);
        }
    }

    // -------------------------------------------------------
    // Update visual transforms from simulation data
    // -------------------------------------------------------
    void UpdateVisuals()
    {
        var victims = victimSystem.victims;

        for (int i = 0; i < victims.Count; i++)
        {
            var data = victims[i];
            var visual = activeVisuals[i];

            visual.transform.position = data.position;
            visual.transform.localScale = Vector3.one * data.scale;
        }
    }
}
