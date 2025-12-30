using System;
using System.Collections;
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
            "FleshImpact",
            () =>
            {
                /// Här yeetar gadflies
                foreach (KeyValuePair<Currency, int> item in collectAmounts)
                {
                    EconomyManager.Instance.IncrementCurrency(item.Key, item.Value);
                    DoPopup(item.Key, item.Value, target);
                }
                /// Här yeetar gadflies
            }
        );
    }

    private void DoPopup(Currency collectedCurrency, int amount, Vector2 position)
    {
        Vector2 offsetPosition = position + UnityEngine.Random.insideUnitCircle;
        PopupTextData data = new PopupTextData(offsetPosition, FormatNumbers.FormatValueString(amount, ValueFormatType.IntegerAdditive), Color.white, collectedCurrency);
        StartCoroutine(DelayedPopup(data));

    }
    private IEnumerator DelayedPopup(PopupTextData data)
    {
        float delay = UnityEngine.Random.Range(0f, 0.5f);
        yield return new WaitForSeconds(delay);
        EconomyManager.Instance.PopUp(data);
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
            "ProjectileExplosion",
            () =>
            {
                EconomyManager.Instance.IncrementCurrency(Currency.HealthPoints, -damage);
                // if (healthBar != null)
                // {

                //     healthBar.ModifyHealth(damage, false);
                // }
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
            "ProjectileExplosion",
            () =>
            {
                EconomyManager.Instance.IncrementCurrency(Currency.HealthPoints, -damage);
                // if (healthBar != null)
                // {

                //     healthBar.ModifyHealth(damage, false);
                // }
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
        string impactAudioKey,
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
            impactAudioKey = impactAudioKey,
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
            var projectile = activeProjectiles[i];

            Vector2 dir = (projectile.target - projectile.position).normalized;
            projectile.position += dir * projectile.speed * Time.deltaTime;
            projectile.obj.transform.position = projectile.position;

            if (Vector2.Distance(projectile.position, projectile.target) < 0.35f)
            {
                if (projectile.impactFX)
                    Instantiate(projectile.impactFX, projectile.position, Quaternion.identity);

                projectile.onArrive?.Invoke();

                if (!string.IsNullOrEmpty(projectile.impactAudioKey))
                    AudioManager.Instance.Play(projectile.impactAudioKey);

                Destroy(projectile.obj);
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

    public string impactAudioKey;
}
