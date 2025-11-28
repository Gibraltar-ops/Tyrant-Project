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
    
    public event System.Action<int> OnEnemyAttack;

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
    public void SpawnResistance()
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
            attackCooldown = UnityEngine.Random.Range(1f, 2f),
            attackDamage = 10,  // Base damage
            attackTimer = UnityEngine.Random.Range(0.3f, 1f), // offset so they don't sync
            speed = UnityEngine.Random.Range(1.6f, 2.8f),
            scale = UnityEngine.Random.Range(0.75f, 1.25f),
            wanderDirection = UnityEngine.Random.insideUnitCircle.normalized,
            wanderTimer = UnityEngine.Random.Range(1f, 2f),
            isCenter = false,
            isLeftSide = spawnLeft
        });

        OnFighterCountChanged?.Invoke(fighters.Count); // Update UI count.
    }

    // --------------------------------------------
    // MOVEMENT
    // --------------------------------------------
    private void SimulateFighters()
    {
        foreach (var fighter in fighters)
        {
            if (!fighter.isCenter)
            {
                Vector2 dir = (fighter.destination - fighter.position).normalized;
                fighter.position += dir * fighter.speed * Time.deltaTime;

                if (Vector2.Distance(fighter.position, fighter.destination) < 0.1f)
                {
                    fighter.isCenter = true;
                    fighter.wanderTimer = UnityEngine.Random.Range(0.7f, 1.7f);
                    fighter.wanderDirection = UnityEngine.Random.insideUnitCircle.normalized;
                }
            }
            else // Wandering
            {
                fighter.wanderTimer -= Time.deltaTime;

                if (fighter.wanderTimer <= 0f)
                {
                    fighter.wanderDirection = UnityEngine.Random.insideUnitCircle.normalized; 
                    fighter.wanderTimer = UnityEngine.Random.Range(0.7f, 1.7f);
                }

                // Apply wandering movement
                fighter.position += fighter.wanderDirection * (fighter.speed * 0.4f) * Time.deltaTime;

                // If victim has wandered outside the capsule walk back inside
                if (!fighter.homeRegion.Contains(fighter.position))
                {
                    Vector2 target = fighter.homeRegion.RandomPointInside();
                    Vector2 dir = (target - fighter.position).normalized;

                    fighter.position += dir * fighter.speed * Time.deltaTime;
                }
            }
            // --------------------------------
            // ATTACK LOGIC
            // --------------------------------
            fighter.attackTimer -= Time.deltaTime;

            if (fighter.attackTimer <= 0f)
            {
                fighter.attackTimer = fighter.attackCooldown;

                // Send attack event
                AttackDirector.Instance.SpawnProjectile(fighter.position, fighter.attackDamage);
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
