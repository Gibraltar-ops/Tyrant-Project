using System;
using System.Collections.Generic;
using UnityEngine;

public class ResistanceSystem : MonoBehaviour
{
    [Header("General Settings")]
    public bool enableSpawning = true;
    public float spawnInterval = 2f;
    public int maxResistanceUnits = 500;

    [Tooltip("Multiplier controlled by UI, similar to VictimSystem.")]
    public float spawnRateMultiplier = 1f;

    private float spawnTimer;

    // --------------------------------------------
    // Two groups of resistance units
    // --------------------------------------------
    [Header("LEFT Resistance Group")]
    public List<SpawnRegion> leftSpawnRegions = new();
    public List<DestinationRegion> leftGatherRegions = new();

    [Header("RIGHT Resistance Group")]
    public List<SpawnRegion> rightSpawnRegions = new();
    public List<DestinationRegion> rightGatherRegions = new();

    // Holds all active resistance fighters
    public List<ResistanceData> fighters = new();

    public event Action<int> OnFighterCountChanged;

    // --------------------------------------------
    // Unity
    // --------------------------------------------
    void Start()
    {
        spawnTimer = spawnInterval;
    }

    void Update()
    {
        spawnTimer -= Time.deltaTime * spawnRateMultiplier;

        if (enableSpawning && fighters.Count < maxResistanceUnits && spawnTimer <= 0f)
        {
            SpawnResistance();
            spawnTimer = spawnInterval;
        }

        SimulateFighters();
    }

    // --------------------------------------------
    // SPAWNING
    // --------------------------------------------
    private void SpawnResistance()
    {
        bool spawnLeft = UnityEngine.Random.value < 0.5f;

        // Select appropriate lists
        List<SpawnRegion> spawnList = spawnLeft ? leftSpawnRegions : rightSpawnRegions;
        List<DestinationRegion> gatherList = spawnLeft ? leftGatherRegions : rightGatherRegions;

        if (spawnList.Count == 0 || gatherList.Count == 0)
            return;

        var spawnRegion = spawnList[UnityEngine.Random.Range(0, spawnList.Count)];
        var gatherRegion = gatherList[UnityEngine.Random.Range(0, gatherList.Count)];

        fighters.Add(new ResistanceData
        {
            position = spawnRegion.RandomPointInside(),
            homeRegion = gatherRegion,
            destination = gatherRegion.RandomPointInside(),
            speed = UnityEngine.Random.Range(1.6f, 2.8f),
            wanderDirection = UnityEngine.Random.insideUnitCircle.normalized,
            wanderTimer = UnityEngine.Random.Range(1f, 2f),
            isCenter = false,
            isLeftSide = spawnLeft
        });

        OnFighterCountChanged?.Invoke(fighters.Count);
    }

    // --------------------------------------------
    // MOVEMENT
    // --------------------------------------------
    private void SimulateFighters()
    {
        foreach (var f in fighters)
        {
            if (!f.isCenter)
            {
                Vector2 dir = (f.destination - f.position).normalized;
                f.position += dir * f.speed * Time.deltaTime;

                if (Vector2.Distance(f.position, f.destination) < 0.1f)
                {
                    f.isCenter = true;
                    f.wanderTimer = UnityEngine.Random.Range(0.7f, 1.7f);
                    f.wanderDirection = UnityEngine.Random.insideUnitCircle.normalized;
                }
            }
            else
            {
                // Wandering
                f.wanderTimer -= Time.deltaTime;

                if (f.wanderTimer <= 0f)
                {
                    f.wanderTimer = UnityEngine.Random.Range(0.7f, 1.7f);
                    f.wanderDirection = UnityEngine.Random.insideUnitCircle.normalized;
                }

                f.position += f.wanderDirection * (f.speed * 0.4f) * Time.deltaTime;

                // Keep them inside capsule / circle
                if (!f.homeRegion.Contains(f.position))
                {
                    Vector2 target = f.homeRegion.RandomPointInside();
                    Vector2 dir = (target - f.position).normalized;
                    f.position += dir * f.speed * Time.deltaTime;
                }
            }
        }
    }

    // --------------------------------------------
    // Editor Gizmos
    // --------------------------------------------
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        foreach (var s in leftSpawnRegions)
            s.DrawGizmo(Color.red);
        foreach (var g in leftGatherRegions)
            g.DrawGizmo(Color.magenta);

        foreach (var s in rightSpawnRegions)
            s.DrawGizmo(Color.cyan);
        foreach (var g in rightGatherRegions)
            g.DrawGizmo(Color.blue);
    }
}
