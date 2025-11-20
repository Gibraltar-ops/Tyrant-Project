using System.Collections.Generic;
using UnityEngine;

public class VictimSystem : MonoBehaviour
{
    [Header("Spawning")]
    public bool enableSpawning = true;
    public float spawnInterval = 1.0f;
    public int maxVictims = 1000;

    public List<SpawnRegion> spawnRegions = new List<SpawnRegion>();
    public List<DestinationRegion> centerCircles = new List<DestinationRegion>();

    private float spawnTimer;
    public List<VictimData> victims = new List<VictimData>();

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

    private void SpawnVictim()
    {
        if (spawnRegions.Count == 0 || centerCircles.Count == 0)
            return;

        var spawnRegion = spawnRegions[Random.Range(0, spawnRegions.Count)];
        var centerRegion = centerCircles[Random.Range(0, centerCircles.Count)];

        victims.Add(new VictimData
        {
            position = spawnRegion.RandomPointInside(),

            homeRegion = centerRegion,
            destination = centerRegion.RandomPointInside(),

            speed = Random.Range(1.5f, 3f),
            scale = Random.Range(0.75f, 1.25f),

            wanderDirection = Random.insideUnitCircle.normalized,
            wanderTimer = Random.Range(0.5f, 1.5f),

            isCenter = false
        });
    }

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
                    victim.wanderTimer = Random.Range(0.5f, 1.5f);
                    victim.wanderDirection = Random.insideUnitCircle.normalized;
                }
            }
            else // wandering
            {
                victim.wanderTimer -= Time.deltaTime;

                if (victim.wanderTimer <= 0f)
                {
                    victim.wanderDirection = Random.insideUnitCircle.normalized;
                    victim.wanderTimer = Random.Range(0.5f, 1.5f);
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

    // Visualize spawn regions in editor
    private void OnDrawGizmosSelected()
    {
        if (spawnRegions == null) return;

        foreach (var r in spawnRegions)
            r.DrawGizmo(Color.yellow);
        
        if (centerCircles != null)
            foreach (var c in centerCircles)
                c.DrawGizmo(Color.green);
    }
}
