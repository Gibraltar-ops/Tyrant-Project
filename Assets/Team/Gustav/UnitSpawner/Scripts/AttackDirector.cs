using System;
using System.Collections.Generic;
using UnityEngine;

public class AttackDirector : MonoBehaviour
{
    // TODO: Attack thrown in an arc instead of straight line.
    // TODO: Attack thrown in random destination within the gizmo tool.
    // TODO: Different projectyle Types based on enemy.
    // TODO: Random Explosion sizes.
    // TODO: Different projectile speeds.
    // TODO: Scale Object based on distance to target. (Maybe based on height)
    // TODO: Sound effects.
    // TODO: Pooling of projectiles and explosions.
    // TODO: ONLY ATTACK if enemy is in center region.
    // TODO: Longer attack cooldowns.

    public static AttackDirector Instance;

    [Header("Prefabs")]
    public GameObject projectilePrefab;
    public GameObject explosionPrefab;

    [Header("Settings")]
    public Vector2 targetPosition = Vector2.zero;
    public float projectileSpeed = 20f;

    // Event that UI or player health listens to
    public event Action<int> OnExplosionDamage;

    private readonly List<ProjectileData> activeProjectiles = new();

    public List<DestinationRegion> grenadeImpactRegions = new();

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        SimulateProjectiles();
    }

    // ----------------------------------------------------------------------
    // PUBLIC: Spawn projectile toward a RANDOM point inside a RANDOM region
    // ----------------------------------------------------------------------
    public void SpawnProjectile(Vector2 startPos, int damage)
    {
        if (grenadeImpactRegions.Count == 0)
        {
            // Debug.LogWarning("No grenadeImpactRegions assigned!");
            return;
        }

        DestinationRegion region = grenadeImpactRegions[
            UnityEngine.Random.Range(0, grenadeImpactRegions.Count)];

        // Choose target within region
        Vector2 targetPoint = region.RandomPointInside();

        // Instantiate projectile object
        GameObject projObj = Instantiate(projectilePrefab, startPos, Quaternion.identity);

        // Add new projectile to simulation
        activeProjectiles.Add(new ProjectileData
        {
            obj = projObj,
            position = startPos,
            target = targetPoint,
            damage = damage
        });
    }

    // ----------------------------------------------------------------------
    // MOVE PROJECTILES + TRIGGER EXPLOSIONS
    // ----------------------------------------------------------------------
    private void SimulateProjectiles()
    {
        for (int i = activeProjectiles.Count - 1; i >= 0; i--)
        {
            var projectile = activeProjectiles[i];

            // Move toward target
            Vector2 dir = (projectile.target - projectile.position).normalized;

            projectile.position += dir * projectileSpeed * Time.deltaTime;
            projectile.obj.transform.position = projectile.position;

            // Reached target?
            if (Vector2.Distance(projectile.position, targetPosition) < 0.1f)
            {
                // Spawn explosion effect
                Instantiate(explosionPrefab, projectile.position, Quaternion.identity);

                // Send damage event
                OnExplosionDamage?.Invoke(projectile.damage);

                // Destroy projectile
                Destroy(projectile.obj);
                activeProjectiles.RemoveAt(i);
            }
        }
    }
    private void OnDrawGizmos()
    {
        if (grenadeImpactRegions == null) return;

        foreach (var region in grenadeImpactRegions)
            region.DrawGizmo(Color.yellow);
    }
}

public class ProjectileData
{
    public GameObject obj;
    public Vector2 position;
    public Vector2 target;
    public int damage;
}
