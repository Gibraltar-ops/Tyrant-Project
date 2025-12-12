using System.Collections.Generic;
using UnityEngine;

public class GadflySystem : MonoBehaviour
{
    [Header("General Settings")]
    public int maxGadflyUnits = 500;
    public float biomassAmount = 10;
    public float biomassMultiplier = 1.0f;
    public float brainmassAmount = 1;
    public float brainmassMultiplier = 1.0f;
    public float harvestMovementSpeed = 4.5f;

    [Header("Harvest Timers")]
    public float harvestCooldown = 10f;         // Time between waves
    public float harvestDuration = 5f;          // Time spent away harvesting
    private float harvestCooldownTimer;         // The codes cooldown countdown timer
    private float harvestDurationTimer;         // The codes harvest duration timer

    private bool harvestActive = false;

    // Spawn region spawn & Destination Regions of gadflys
    [Header("REGION Zones")]
    public List<SpawnRegion> spawnArea = new();
    public List<DestinationRegion> gatherArea = new();
    public List<DestinationRegion> harvestArea = new();

    // Holds all active resistance fighters
    public List<GadflyData> gadflies = new();

    // --------------------------------------------
    // START Set up starting variables & behaviors
    // --------------------------------------------
    void Start()
    {
        harvestCooldownTimer = harvestCooldown;

        // Spawn some initial crowd (optional)
        for (int i = 0; i < 10; i++)
            SpawnGatfly();
    }

    // --------------------------------------------
    // UPDATE Simulation & HarvestCycle
    // --------------------------------------------
    void Update()
    {
        SimulateGatflies();
        HandleHarvestCycle();
    }

    // --------------------------------------------
    // SPAWNING new Gadfly Unit
    // --------------------------------------------
    public void SpawnGatfly()
    {
        if (gadflies.Count >= maxGadflyUnits) return;

        var spawnRegion = spawnArea[Random.Range(0, spawnArea.Count)];
        var gatherRegion = gatherArea[Random.Range(0, gatherArea.Count)];
        var harvestRegion = harvestArea[Random.Range(0, harvestArea.Count)];

        gadflies.Add(
            new GadflyData
            {
                position = spawnRegion.RandomPointInside(),
                homeRegion = gatherRegion,
                harvestRegion = harvestRegion,
                destination = gatherRegion.RandomPointInside(),

                wanderSpeed = Random.Range(2f, 2.1f),
                harvestSpeed = harvestMovementSpeed,
                scale = Random.Range(0.2f, 0.21f),

                wanderDirection = Random.insideUnitCircle.normalized,
                wanderTimer = Random.Range(1f, 2f),

                velocity = Vector2.zero,
                arcStrength = Random.Range(-0.6f, 0.6f),
                jitterStrength = Random.Range(0.6f, 1.2f),

                isHome = false,
                isHarvesting = false,
            }
        );
    }

    // --------------------------------------------
    // GLOBAL HARVEST STATE MACHINE
    // --------------------------------------------
    void HandleHarvestCycle()
    {
        int total = gadflies.Count;
        if (total == 0) return;

        int atHomeCount = 0;
        int atHarvestArrivedCount = 0;

        for (int i = 0; i < total; i++)
        {
            if (gadflies[i].isHome)
                atHomeCount++;

            if (gadflies[i].hasReachedHarvest)
                atHarvestArrivedCount++;
        }

        // ------------------------------------------------
        // 1. ONLY COUNTDOWN TO NEXT HARVEST IF
        //     at least HALF have returned home
        // ------------------------------------------------
        if (!harvestActive && atHomeCount >= total * 0.5f)
        {
            harvestCooldownTimer -= Time.deltaTime;
        }
        // ------------------------------------------------
        // 2. WHEN TIMER HITS ZERO -> FORCE ALL TO HARVEST
        // ------------------------------------------------
        if (!harvestActive && harvestCooldownTimer <= 0f)
        {
            harvestActive = true;
            harvestDurationTimer = harvestDuration;

            for (int i = 0; i < total; i++)
            {
                var gadfly = gadflies[i];

                gadfly.isHarvesting = true;
                gadfly.isHome = false;
                gadfly.hasReachedHarvest = false;
                gadfly.destination = gadfly.harvestRegion.RandomPointInside();

                gadfly.hasPayload = false;
                gadfly.hasThrownPayload = false;
            }
        }
        // ------------------------------------------------
        // 3. START HARVEST DURATION TIMER
        //     WHEN AT LEAST HALF REACH HARVEST
        // ------------------------------------------------
        if (harvestActive && atHarvestArrivedCount >= total * 0.5f)
        {
            harvestDurationTimer -= Time.deltaTime;
        }
        // ------------------------------------------------
        // 4. WHEN HARVEST DURATION ENDS -> SEND ALL HOME
        // ------------------------------------------------
        if (harvestActive && harvestDurationTimer <= 0f)
        {
            harvestActive = false;
            harvestCooldownTimer = harvestCooldown;

            for (int i = 0; i < total; i++)
            {
                var gadfly = gadflies[i];

                gadfly.isHarvesting = false;
                gadfly.isHome = false;
                gadfly.hasReachedHarvest = false;
                gadfly.destination = gadfly.homeRegion.RandomPointInside();

                gadfly.hasPayload = true;
                gadfly.hasThrownPayload = false;
            }
        }
    }

    // --------------------------------------------
    // MOVEMENT Simulation
    // --------------------------------------------
    private void SimulateGatflies()
    {
        foreach (var gadfly in gadflies)
        {
            // Selection of movement speed
            float targetSpeed = gadfly.isHarvesting ? harvestMovementSpeed : gadfly.wanderSpeed;

            // Target direction
            Vector2 toTarget = (gadfly.destination - gadfly.position);
            Vector2 targetDir = toTarget.normalized;

            // Arc Flight (Only when Traveling)
            if (!gadfly.isHome)
            {
                Vector2 perp = new Vector2(-targetDir.y, targetDir.x);
                targetDir += perp * gadfly.arcStrength;
                targetDir.Normalize();
            }

            // Smooth velocity blend
            gadfly.velocity = Vector2.Lerp(gadfly.velocity, targetDir * targetSpeed, Time.deltaTime * 3.5f);

            // Apply "Micro Jitter" when home
            if (gadfly.isHome && !gadfly.isHarvesting)
            {
                Vector2 jitter = Random.insideUnitCircle * gadfly.jitterStrength;
                gadfly.velocity += jitter * 0.4f;
            }

            // Move Gadfly
            gadfly.position += gadfly.velocity * Time.deltaTime;

            // Destination Reached Check
            if (Vector2.Distance(gadfly.position, gadfly.destination) < 0.45f)
            {
                if (gadfly.isHarvesting)
                {
                    gadfly.hasReachedHarvest = true;
                }
                else
                {
                    gadfly.isHome = true;
                    gadfly.wanderTimer = Random.Range(1f, 2f);
                    gadfly.wanderDirection = Random.insideUnitCircle.normalized;

                    // THROW ONLY ONCE PER RETURN
                    if (gadfly.hasPayload && !gadfly.hasThrownPayload)
                    {
                        gadfly.hasThrownPayload = true;
                        gadfly.hasPayload = false;

                        ProjectileDirector.Instance.SpawnHumanProjectile(
                            gadfly.position,
                            Mathf.RoundToInt(biomassAmount * biomassMultiplier),
                            Mathf.RoundToInt(brainmassAmount * brainmassMultiplier)
                        );
                    }
                }
            }

            // Idle Wander in soft circles
            if (gadfly.isHome && !gadfly.isHarvesting)
            {
                gadfly.wanderTimer -= Time.deltaTime;

                if (gadfly.wanderTimer <= 0f)
                {
                    gadfly.wanderDirection = Random.insideUnitCircle.normalized;
                    gadfly.wanderTimer = Random.Range(0.6f, 1.2f);
                }

                // Gently bias destination instead of snapping
                gadfly.destination = gadfly.position + gadfly.wanderDirection * Random.Range(0.4f, 0.8f);

                // Stay inside home region
                if (!gadfly.homeRegion.Contains(gadfly.destination))
                    gadfly.destination = gadfly.homeRegion.RandomPointInside();
            }
        }
    }

    // --------------------------------------------
    // Editor Gizmos
    // --------------------------------------------
    private void OnDrawGizmosSelected()
    {
        foreach (var s in spawnArea)
            s.DrawGizmo(Color.yellow);
        foreach (var g in gatherArea)
            g.DrawGizmo(Color.orange);
        foreach (var h in harvestArea)
            h.DrawGizmo(Color.brown);
    }
}
