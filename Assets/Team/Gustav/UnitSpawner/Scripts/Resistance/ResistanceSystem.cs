using System;
using System.Collections.Generic;
using UnityEngine;

public class ResistanceSystem : MonoBehaviour
{
    [Header("General Settings")]
    public int maxResistanceUnits = 500;
    public float attackCooldown = 7f;

    [Header("LEFT Resistance Group")]
    public List<SpawnRegion> leftSpawnRegions = new();
    public List<DestinationRegion> leftGatherRegions = new();

    [Header("RIGHT Resistance Group")]
    public List<SpawnRegion> rightSpawnRegions = new();
    public List<DestinationRegion> rightGatherRegions = new();

    // Holds all active resistance fighters
    public List<ResistanceData> fighters = new();
    public event Action<int> OnFighterCountChanged;

    void Start()
    {
        /* ///Optional: Spawn some initial crowd
        for (int i = 0; i < 50; i++)
            SpawnResistance(); */
    }

    void Update()
    {
        SimulateFighters();
    }

    public void SpawnResistance()
    {
        if (fighters.Count >= maxResistanceUnits)
            return;

        bool spawnLeft = UnityEngine.Random.value < 0.5f;

        var spawnList = spawnLeft ? leftSpawnRegions : rightSpawnRegions;
        var gatherList = spawnLeft ? leftGatherRegions : rightGatherRegions;

        if (spawnList.Count == 0 || gatherList.Count == 0)
            return;

        var spawnRegion = spawnList[UnityEngine.Random.Range(0, spawnList.Count)];
        var gatherRegion = gatherList[UnityEngine.Random.Range(0, gatherList.Count)];

        fighters.Add(
            new ResistanceData
            {
                position = spawnRegion.RandomPointInside(),
                homeRegion = gatherRegion,
                destination = gatherRegion.RandomPointInside(),

                speed = UnityEngine.Random.Range(0.4f, 1f),
                scale = UnityEngine.Random.Range(1f, 1.01f),

                attackDamage = 10,
                attackTimer = UnityEngine.Random.Range(0f, attackCooldown), // desync

                isCenter = false,
                isMarkedForDeath = false,
                isLeftSide = spawnLeft,
            }
        );

        OnFighterCountChanged?.Invoke(fighters.Count);
    }

    // -----------------------------------------------------
    // MOVEMENT + ATTACK
    // -----------------------------------------------------

    private void SimulateFighters()
    {
        foreach (var fighter in fighters)
        {
            if (fighter.isMarkedForDeath)
                continue;

            // --------------------------
            // MOVE TO GATHER ZONE
            // --------------------------
            if (!fighter.isCenter)
            {
                Vector2 dir = (fighter.destination - fighter.position).normalized;
                fighter.position += dir * fighter.speed * Time.deltaTime;

                if (Vector2.Distance(fighter.position, fighter.destination) < 0.1f)
                {
                    fighter.isCenter = true;
                }
            }

            // --------------------------
            // ATTACK LOGIC
            // --------------------------
            fighter.attackTimer -= Time.deltaTime;

            if (fighter.isCenter && fighter.attackTimer <= 0f)
            {
                ProjectileDirector.Instance.SpawnGrenade(fighter.position, fighter.attackDamage);
                AudioManager.Instance.Play("ProjectileLaunch");
                fighter.attackTimer = attackCooldown;
            }
        }
    }

    // --------------------------------------------
    // Editor Gizmos
    // --------------------------------------------
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        foreach (var l in leftSpawnRegions)
            l.DrawGizmo(Color.red);
        foreach (var g in leftGatherRegions)
            g.DrawGizmo(Color.magenta);

        foreach (var r in rightSpawnRegions)
            r.DrawGizmo(Color.cyan);
        foreach (var g in rightGatherRegions)
            g.DrawGizmo(Color.blue);
    }
}
