using System.Collections.Generic;
using UnityEngine.Pool;
using UnityEngine;

public class GadflyRenderer : MonoBehaviour
{
    public GadflySystem gadflySystem;
    public GameObject gadflyPrefab;

    private IObjectPool<GameObject> pool;
    private readonly List<GameObject> activeVisuals = new(); // All currently active visual objects

    void Awake()
    {
        pool = new ObjectPool<GameObject>(
            CreateVisual,
            obj => obj.SetActive(true),
            obj => obj.SetActive(false),
            obj => Destroy(obj),
            false, 100, 500
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

    // -------------------------------------------------------
    // Make sure active visuals match gadfly count
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
        var gadflies = gadflySystem.gadflies;

        for (int i = 0; i < gadflies.Count; i++)
        {
            var data = gadflies[i];
            var visual = activeVisuals[i];

            Vector2 dir = (data.destination - data.position).normalized;

            // Update animator + flip + position
            visual.GetComponent<UnitVisual>().UpdateVisual(data.position, dir);

            // Scale
            visual.GetComponent<UnitVisual>().ApplyScale(data.scale);
        }
    }
}
