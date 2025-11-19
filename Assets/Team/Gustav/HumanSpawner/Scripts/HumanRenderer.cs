using System.Collections.Generic;
using UnityEngine;

public class HumanRenderer : MonoBehaviour
{
    // Reference to the simulation system
    public HumanSystem humanSystem;

    // Prefab for visual representation
    public GameObject humanPrefab;

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
        foreach (var human in humanSystem.humans)
        {
            GameObject visual = Instantiate(humanPrefab, transform);
            visuals.Add(visual);
        }
    }

    // ---------------------------------------------------------------
    // Add/remove visuals if humans list size changes at runtime
    // ---------------------------------------------------------------
    void SyncPoolSize()
    {
        int simCount = humanSystem.humans.Count;
        int visCount = visuals.Count;

        // Add more visuals if needed
        while (visCount < simCount)
        {
            GameObject visual = Instantiate(humanPrefab, transform);
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
        var humans = humanSystem.humans;

        for (int i = 0; i < humans.Count; i++)
        {
            var data = humans[i];
            var visual = visuals[i];

            visual.SetActive(true); // ensure enabled (in case reused from pool)
            visual.transform.position = data.position;
            visual.transform.localScale = Vector3.one * data.scale;
        }
    }
}
