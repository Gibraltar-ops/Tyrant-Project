using System;
using System.Collections.Generic;
using UnityEngine;

public class GadflySystem : MonoBehaviour
{
    [Header("General Settings")]
    public int maxGadflyUnits = 500;
    public float harvestCooldown = 10f;

    // --------------------------------------------
    // Spawn region spawn & Destination Regions of gadflys
    // --------------------------------------------
    [Header("REGION Zones")]
    public List<SpawnRegion> spawnArea = new();
    public List<DestinationRegion> gatherArea = new();
    public List<DestinationRegion> harvestArea = new();

    // Holds all active resistance fighters
    public List<GadflyData> gadflies = new List<GadflyData>();

    public event System.Action<int> OnEnemyAttack;

    

    void Start()
    {
        // Spawn some initial crowd (optional)
        for (int i = 0; i < 50; i++)
            SpawnGatfly();
    }

    // Update is called once per frame
    void Update()
    {
        //SimulateGatflys();
    }

    // --------------------------------------------
    // SPAWNING
    // --------------------------------------------
    public void SpawnGatfly()
    {
        var spawnRegion = spawnArea[UnityEngine.Random.Range(0, spawnArea.Count)];
        var gatherRegion = gatherArea[UnityEngine.Random.Range(0, gatherArea.Count)];

        gadflies.Add(
            new GadflyData
            {
                position = spawnRegion.RandomPointInside(),
                homeRegion = gatherRegion,
                destination = gatherRegion.RandomPointInside(),
                wanderSpeed = UnityEngine.Random.Range(1f, 1.1f),
                harvestSpeed = UnityEngine.Random.Range(2f, 2.1f),
                scale = UnityEngine.Random.Range(1f, 1.1f),
                wanderDirection = UnityEngine.Random.insideUnitCircle.normalized,
                wanderTimer = UnityEngine.Random.Range(1f, 2f),
                isHome = false,
            }
        );
    }

    // --------------------------------------------
    // MOVEMENT
    // --------------------------------------------
    //private void SimulateGatflys()
    //{
    //    foreach (var gadflies in gadflies)
    //    {
    //        if (!gadflies.isHome) // Moving to center
    //        {
    //            Vector2 dir = (gadflies.destination - gadflies.position).normalized;
    //            gadflies.position += dir * gadflies.harvestSpeed * Time.deltaTime;

    //            if (Vector2.Distance(gadflies.position, gadflies.destination) < 0.1f)
    //            {
    //                gadflies.isHome = true;
    //                gadflies.wanderTimer = UnityEngine.Random.Range(0.7f, 1.7f);
    //                gadflies.wanderDirection = UnityEngine.Random.insideUnitCircle.normalized;
    //            }
    //        }
    //        else // Wandering
    //        {
    //            gadflies.wanderTimer -= Time.deltaTime;

    //            if (gadflies.wanderTimer <= 0f)
    //            {
    //                gadflies.wanderDirection = UnityEngine.Random.insideUnitCircle.normalized;
    //                gadflies.wanderTimer = UnityEngine.Random.Range(0.7f, 1.7f);
    //            }

    //            // Apply wandering movement
    //            gadflies.position +=
    //                gadflies.wanderDirection * (gadflies.speed * 0.4f) * Time.deltaTime;

    //            // If victim has wandered outside the capsule walk back inside
    //            if (!gadflies.homeRegion.Contains(gadflies.position))
    //            {
    //                Vector2 target = gadflies.homeRegion.RandomPointInside();
    //                Vector2 dir = (target - gadflies.position).normalized;

    //                gadflies.position += dir * gadflies.speed * Time.deltaTime;
    //            }
    //        }
    //        // --------------------------------
    //        // Harvest LOGIC
    //        // --------------------------------
    //        gadflies.attackTimer -= Time.deltaTime;

    //        if (gadflies.isHome && gadflies.attackTimer <= 0f)
    //        {
    //            // FIRE!
    //            AttackDirector.Instance.SpawnProjectile(gadflies.position, gadflies.attackDamage);

    //            gadflies.attackTimer = attackCooldown;
    //        }
    //    }
    //}

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
