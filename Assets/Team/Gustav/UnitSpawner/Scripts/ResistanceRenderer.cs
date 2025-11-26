using System.Collections.Generic;
using UnityEngine.Pool;
using UnityEngine;

public class ResistanceRenderer : MonoBehaviour
{
    public ResistanceSystem resistanceSystem;
    public GameObject resistancePrefab;

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
        GameObject obj = Instantiate(resistancePrefab, transform);
        obj.SetActive(false);
        return obj;
    }
    void OnTakeFromPool(GameObject obj) => obj.SetActive(true);
    void OnReturnedToPool(GameObject obj) => obj.SetActive(false);
    void OnDestroyVisual(GameObject obj) => Destroy(obj);

    // -------------------------------------------------------
    // Make sure active visuals match fighter count
    // -------------------------------------------------------
    void SyncActiveCount()
    {
        int simCount = resistanceSystem.fighters.Count;

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
    // Update visuals from fighter simulation data
    // -------------------------------------------------------
    void UpdateVisuals()
    {
        var fighters = resistanceSystem.fighters;

        for (int i = 0; i < fighters.Count; i++)
        {
            var data = fighters[i];
            var visual = activeVisuals[i];

            // Movement direction
            Vector2 dir = data.isCenter ? data.wanderDirection :
                        (data.destination - data.position).normalized;

            // Update animator + flip + position
            visual.GetComponent<VictimVisual>().UpdateVisual(data.position, dir);

            // Scale
            visual.GetComponent<VictimVisual>().ApplyScale(data.scale);
        }
    }
}
