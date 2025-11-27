using System;
using System.Collections.Generic;
using UnityEngine;

public class VictimSystem : MonoBehaviour
{
    [Header("Spawning")]
    public bool enableSpawning = true;
    public float spawnInterval = 1.0f;
    public int maxVictims = 10000;

    public List<SpawnRegion> spawnRegions = new List<SpawnRegion>();
    public List<DestinationRegion> destinationRegions = new List<DestinationRegion>();

    private float spawnTimer;
    public List<VictimData> victims = new List<VictimData>();

    public event Action<int> OnVictimCountChanged;

    [HideInInspector]
    public float spawnRateMultiplier = 1f; // Adjust spawn rate with UI Btn/Slider.

    void Start()
    {
        spawnTimer = spawnInterval;

        //// Spawn some initial crowd (optional)
        //for (int i = 0; i < 50; i++)
        //    SpawnVictim();
    }

    void Update()
    {
        spawnTimer -= Time.deltaTime * spawnRateMultiplier;

        if (enableSpawning && victims.Count < maxVictims && spawnTimer <= 0f)
        {
            SpawnVictim();
            spawnTimer = spawnInterval;
        }

        SimulateVictims();
    }

    // --------------------------------------------
    // SPAWNING
    // --------------------------------------------
    private void SpawnVictim()
    {
        if (spawnRegions.Count == 0 || destinationRegions.Count == 0)
            return;

        var spawnRegion = spawnRegions[UnityEngine.Random.Range(0, spawnRegions.Count)];
        var centerRegion = destinationRegions[UnityEngine.Random.Range(0, destinationRegions.Count)];

        victims.Add(new VictimData
        {
            position = spawnRegion.RandomPointInside(),
            homeRegion = centerRegion,
            destination = centerRegion.RandomPointInside(),
            speed = UnityEngine.Random.Range(1.5f, 3f),
            scale = UnityEngine.Random.Range(0.75f, 1.25f),
            wanderDirection = UnityEngine.Random.insideUnitCircle.normalized,
            wanderTimer = UnityEngine.Random.Range(0.5f, 1.5f),
            isCenter = false
        });

        OnVictimCountChanged?.Invoke(victims.Count); // Update UI count.
    }

    // --------------------------------------------
    // MOVEMENT
    // --------------------------------------------
    private void SimulateVictims()
    {
        foreach (var victim in victims)
        {
            if (!victim.isCenter)
            {
                Vector2 dir = (victim.destination - victim.position).normalized;
                victim.position += dir * victim.speed * Time.deltaTime;

                if (Vector2.Distance(victim.position, victim.destination) < 0.1f)
                {
                    victim.isCenter = true;
                    victim.wanderTimer = UnityEngine.Random.Range(0.5f, 1.5f);
                    victim.wanderDirection = UnityEngine.Random.insideUnitCircle.normalized;
                }
            }
            else // Wandering
            {
                victim.wanderTimer -= Time.deltaTime;

                if (victim.wanderTimer <= 0f)
                {
                    victim.wanderDirection = UnityEngine.Random.insideUnitCircle.normalized;
                    victim.wanderTimer = UnityEngine.Random.Range(0.5f, 1.5f);
                }

                // Apply wandering movement
                victim.position += victim.wanderDirection * (victim.speed * 0.5f) * Time.deltaTime;

                // If victim has wandered outside the circle walk back inside
                if (!victim.homeRegion.Contains(victim.position))
                {
                    Vector2 returnPoint = victim.homeRegion.RandomPointInside();
                    Vector2 dir = (returnPoint - victim.position).normalized;

                    victim.position += dir * victim.speed * Time.deltaTime;
                }
            }
        }
    }

    // --------------------------------------------
    // Editor Gizmos
    // --------------------------------------------
    private void OnDrawGizmosSelected()
    {
        if (spawnRegions == null) return;

        foreach (var r in spawnRegions)
            r.DrawGizmo(Color.yellow);
        
        if (destinationRegions != null)
            foreach (var c in destinationRegions)
                c.DrawGizmo(Color.green);
    }
}
