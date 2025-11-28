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
    // TODO: Longer random attack cooldowns.

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

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        SimulateProjectiles();
    }

    public void SpawnProjectile(Vector2 startPos, int damage)
    {
        GameObject projObj = Instantiate(projectilePrefab, startPos, Quaternion.identity);

        activeProjectiles.Add(new ProjectileData
        {
            obj = projObj,
            position = startPos,
            damage = damage
        });
    }

    private void SimulateProjectiles()
    {
        for (int i = activeProjectiles.Count - 1; i >= 0; i--)
        {
            var p = activeProjectiles[i];

            // Move toward target
            Vector2 dir = (targetPosition - p.position).normalized;
            p.position += dir * projectileSpeed * Time.deltaTime;

            p.obj.transform.position = p.position;

            // Reached target?
            if (Vector2.Distance(p.position, targetPosition) < 0.1f)
            {
                // Spawn explosion effect
                Instantiate(explosionPrefab, p.position, Quaternion.identity);

                // Send damage event
                OnExplosionDamage?.Invoke(p.damage);

                // Destroy projectile
                Destroy(p.obj);
                activeProjectiles.RemoveAt(i);
            }
        }
    }
}

public class ProjectileData
{
    public GameObject obj;
    public Vector2 position;
    public int damage;
}
