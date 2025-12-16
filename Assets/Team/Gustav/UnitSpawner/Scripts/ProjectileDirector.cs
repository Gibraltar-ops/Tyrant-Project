using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDirector : MonoBehaviour
{
    public static ProjectileDirector Instance;

    [Header("Health bar reference")]
    public HealthBar healthBar;

    [Header("Projectile Prefabs")]
    public GameObject humanProjectilePrefab;
    public GameObject grenadeProjectilePrefab;
    public GameObject rocketProjectilePrefab;

    [Header("Impact FX Prefabs")]
    public GameObject explosionFXPrefab;
    public GameObject humanImpactFXPrefab;

    [Header("Projectile Settings")]
    public float defaultProjectileSpeed = 20f;

    [Header("Impact Target Regions")]
    public List<DestinationRegion> tyrantImpactRegions = new();

    private readonly List<ProjectileData> activeProjectiles = new();

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        SimulateProjectiles();
    }


    // ============================================================
    // HUMAN PROJECTILE
    // ============================================================
    public void SpawnHumanProjectile(Vector2 startPos, Dictionary<Currency, int> collectAmounts)
    {
        if (tyrantImpactRegions.Count == 0) return;

        DestinationRegion region =
            tyrantImpactRegions[UnityEngine.Random.Range(0, tyrantImpactRegions.Count)];

        Vector2 target = region.RandomPointInside();

        SpawnProjectileInternal(
            humanProjectilePrefab,
            startPos,
            target,
            defaultProjectileSpeed,
            0,
            humanImpactFXPrefab,
            () =>
            {
                /// Här yeetar gadflies
                foreach (KeyValuePair<Currency, int> item in collectAmounts)
                {
                    EconomyManager.Instance.IncrementCurrency(item.Key, item.Value);
                }
                /// Här yeetar gadflies
            }
        );
    }

    // ============================================================
    // GRENADE PROJECTILE (Enemy)
    // ============================================================
    public void SpawnGrenade(Vector2 startPos, int damage)
    {
        if (tyrantImpactRegions.Count == 0) return;

        DestinationRegion region =
            tyrantImpactRegions[UnityEngine.Random.Range(0, tyrantImpactRegions.Count)];

        Vector2 target = region.RandomPointInside();

        SpawnProjectileInternal(
            grenadeProjectilePrefab,
            startPos,
            target,
            defaultProjectileSpeed,
            damage,
            explosionFXPrefab,
            () =>
            {
                if (healthBar != null)
                {
                    healthBar.ModifyHealth(damage, false);
                }
            }
        );
    }

    // ============================================================
    // ROCKET PROJECTILE (Enemy)
    // ============================================================
    public void SpawnRocket(Vector2 startPos, int damage)
    {
        if (tyrantImpactRegions.Count == 0) return;

        DestinationRegion region =
            tyrantImpactRegions[UnityEngine.Random.Range(0, tyrantImpactRegions.Count)];

        Vector2 target = region.RandomPointInside();

        SpawnProjectileInternal(
            rocketProjectilePrefab,
            startPos,
            target,
            defaultProjectileSpeed * 1.25f,
            damage,
            explosionFXPrefab,
            () =>
            {
                if (healthBar != null)
                {
                    healthBar.ModifyHealth(damage, false);
                }
            }
        );
    }

    // ============================================================
    // CORE PROJECTILE LOGIC
    // ============================================================
    private void SpawnProjectileInternal(
        GameObject projectilePrefab,
        Vector2 startPos,
        Vector2 targetPos,
        float speed,
        int payloadValue,
        GameObject impactFX,
        Action onArrive
    )
    {
        if (!projectilePrefab) return;

        GameObject obj = Instantiate(projectilePrefab, startPos, Quaternion.identity);

        activeProjectiles.Add(new ProjectileData
        {
            obj = obj,
            position = startPos,
            target = targetPos,
            speed = speed,
            value = payloadValue,
            impactFX = impactFX,
            onArrive = onArrive
        });
    }

    // ------------------------------------------------------
    // SIMULATION of projectiles
    // ------------------------------------------------------
    private void SimulateProjectiles()
    {
        for (int i = activeProjectiles.Count - 1; i >= 0; i--)
        {
            var p = activeProjectiles[i];

            Vector2 dir = (p.target - p.position).normalized;
            p.position += dir * p.speed * Time.deltaTime;
            p.obj.transform.position = p.position;

            if (Vector2.Distance(p.position, p.target) < 0.35f)
            {
                if (p.impactFX)
                    Instantiate(p.impactFX, p.position, Quaternion.identity);

                p.onArrive?.Invoke();
                AudioManager.Instance.Play("ProjectileExplosion");
                Destroy(p.obj);
                activeProjectiles.RemoveAt(i);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (tyrantImpactRegions == null) return;

        foreach (var region in tyrantImpactRegions)
            region.DrawGizmo(Color.yellow);
    }
}

public class ProjectileData
{
    public GameObject obj;
    public Vector2 position;
    public Vector2 target;

    public float speed;
    public int value;

    public GameObject impactFX;
    public Action onArrive;
}
