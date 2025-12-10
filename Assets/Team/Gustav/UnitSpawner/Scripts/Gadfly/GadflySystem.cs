using System.Collections.Generic;
using UnityEngine;

public class GadflySystem : MonoBehaviour
{
    [Header("General Settings")]
    public int maxGadflyUnits = 500;

    [Header("Harvest Timers")]
    public float harvestCooldown = 10f;   // Time between waves
    public float harvestDuration = 5f;    // Time spent away harvesting

    private float harvestTimer;
    private float harvestDurationTimer;

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
        harvestTimer = harvestCooldown;

        // Spawn some initial crowd (optional)
        for (int i = 0; i < 25; i++)
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
                harvestSpeed = Random.Range(4.5f, 4.6f),
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
            harvestTimer -= Time.deltaTime;
        }
        // ------------------------------------------------
        // 2. WHEN TIMER HITS ZERO -> FORCE ALL TO HARVEST
        // ------------------------------------------------
        if (!harvestActive && harvestTimer <= 0f)
        {
            harvestActive = true;
            harvestDurationTimer = harvestDuration;

            for (int i = 0; i < total; i++)
            {
                var g = gadflies[i];

                g.isHarvesting = true;
                g.isHome = false;
                g.hasReachedHarvest = false;
                g.destination = g.harvestRegion.RandomPointInside();
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
            harvestTimer = harvestCooldown;

            for (int i = 0; i < total; i++)
            {
                var gadfly = gadflies[i];

                gadfly.isHarvesting = false;
                gadfly.isHome = false;
                gadfly.hasReachedHarvest = false;
                gadfly.destination = gadfly.homeRegion.RandomPointInside();
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
            float targetSpeed = gadfly.isHarvesting ? gadfly.harvestSpeed : gadfly.wanderSpeed;

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
            if (Vector2.Distance(gadfly.position, gadfly.destination) < 0.15f)
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



            //// --------------------------------
            //// MOVING TO DESTINATION
            //// --------------------------------
            //Vector2 dir = (gadfly.destination - gadfly.position).normalized;
            //float speed = gadfly.isHarvesting ? gadfly.harvestSpeed : gadfly.wanderSpeed;

            //gadfly.position += dir * speed * Time.deltaTime;

            //// --------------------------------
            //// DESTINATION REACHED
            //// --------------------------------
            //if (Vector2.Distance(gadfly.position, gadfly.destination) < 0.1f)
            //{
            //    if (gadfly.isHarvesting)
            //    {
            //        gadfly.hasReachedHarvest = true;
            //    }
            //    else
            //    {
            //        // ARRIVED HOME
            //        gadfly.isHome = true;
            //        gadfly.wanderTimer = Random.Range(1f, 2f);
            //        gadfly.wanderDirection = Random.insideUnitCircle.normalized;
            //    }
            //}

            //// --------------------------------
            //// WANDERING WHILE HOME
            //// --------------------------------
            //if (gadfly.isHome && !gadfly.isHarvesting)
            //{
            //    gadfly.wanderTimer -= Time.deltaTime;

            //    if (gadfly.wanderTimer <= 0f)
            //    {
            //        gadfly.wanderDirection = Random.insideUnitCircle.normalized;
            //        gadfly.wanderTimer = Random.Range(1f, 2f);
            //    }

            //    gadfly.position += gadfly.wanderDirection * (gadfly.wanderSpeed * 0.4f) * Time.deltaTime;

            //    if (!gadfly.homeRegion.Contains(gadfly.position))
            //    {
            //        gadfly.destination = gadfly.homeRegion.RandomPointInside();
            //        gadfly.isHome = false;
            //    }
            //}
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
