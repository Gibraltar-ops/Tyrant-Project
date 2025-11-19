using System.Collections.Generic;
using UnityEngine;

public class VictimRenderer : MonoBehaviour
{
    // Reference to the simulation system
    public VictimSystem victimSystem;

    // Prefab for visual representation
    public GameObject victimPrefab;

    private readonly List<GameObject> visuals = new List<GameObject>();

    void Start()
    {
        CreateInitialPool();
    }

    void LateUpdate()
    {
        SyncPoolSize();
        UpdateVisuals();
    }

    // ---------------------------------------------------------------
    // Create pool matching initial human count
    // ---------------------------------------------------------------
    void CreateInitialPool()
    {
        foreach (var human in victimSystem.victims)
        {
            GameObject visual = Instantiate(victimPrefab, transform);
            visuals.Add(visual);
        }
    }

    // ---------------------------------------------------------------
    // Add/remove visuals if humans list size changes at runtime
    // ---------------------------------------------------------------
    void SyncPoolSize()
    {
        int simCount = victimSystem.victims.Count;
        int visCount = visuals.Count;

        // Add more visuals if needed
        while (visCount < simCount)
        {
            GameObject visual = Instantiate(victimPrefab, transform);
            visuals.Add(visual);
            visCount++;
        }

        // Disable excess visuals
        while (visCount > simCount)
        {
            visuals[visCount - 1].SetActive(false);
            visCount--;
        }
    }

    // ---------------------------------------------------------------
    // Apply simulation data to pooled GameObjects
    // ---------------------------------------------------------------
    void UpdateVisuals()
    {
        var victims = victimSystem.victims;

        for (int i = 0; i < victims.Count; i++)
        {
            var data = victims[i];
            var visual = visuals[i];

            visual.SetActive(true); // ensure enabled (in case reused from pool)
            visual.transform.position = data.position;
            visual.transform.localScale = Vector3.one * data.scale;
        }
    }
}
