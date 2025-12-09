using System.Collections.Generic;
using UnityEngine.Pool;
using UnityEngine;

public class GadflyRenderer : MonoBehaviour
{
    public GadflySystem gadflySystem;
    public GameObject gadflyPrefab;

    private IObjectPool<GameObject> pool;

    public int preloadAmount = 100;

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
            maxSize: 500
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
        GameObject obj = Instantiate(gadflyPrefab, transform);
        obj.SetActive(false);
        return obj;
    }
    void OnTakeFromPool(GameObject obj) => obj.SetActive(true);
    void OnReturnedToPool(GameObject obj) => obj.SetActive(false);
    void OnDestroyVisual(GameObject obj) => Destroy(obj);
    // -------------------------------------------------------
    // Make sure active visuals match gadfly count // REMOVE THIS METHOD?!?!?!
    // -------------------------------------------------------
    void SyncActiveCount()
    {
        int simCount = gadflySystem.gadflies.Count;

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
    // Update visuals from gadfly simulation data
    // -------------------------------------------------------
    void UpdateVisuals()
    {
        var gadflys = gadflySystem.gadflies;

        //for (int i = 0; i < gadflys.Count; i++)
        //{
        //    var data = gadflys[i];
        //    var visual = activeVisuals[i];

        //    // Movement direction
        //    Vector2 dir = data.isCenter ? data.wanderDirection :
        //                (data.destination - data.position).normalized;

        //    // Update animator + flip + position
        //    visual.GetComponent<UnitVisual>().UpdateVisual(data.position, dir);

        //    // Scale
        //    visual.GetComponent<UnitVisual>().ApplyScale(data.scale);
        //}
    }
}
