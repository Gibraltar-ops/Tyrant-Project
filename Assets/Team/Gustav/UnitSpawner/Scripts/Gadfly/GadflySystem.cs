using System.Collections.Generic;
using UnityEngine;

public class GadflySystem : MonoBehaviour
{
    [Header("General Settings")]
    public CurrencySourceSO gadflyData;
    public int maxGadflyUnits = 500;
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

    void Awake()
    {
        Lood.UpgradeManager.Instance.AddUpgradeableObject(gadflyData);
    }
    // --------------------------------------------
    // START Set up starting variables & behaviors
    // --------------------------------------------
    void Start()
    {
        harvestCooldownTimer = Lood.UpgradeManager.Instance.CurrencySources[gadflyData.Tag].MainStats[MainStat.GadflyCooldown];

        // Spawn some initial crowd (optional)
        for (int i = 0; i < 1; i++)
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
                harvestSpeed = GetCurrentSpeed(),
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
            harvestDurationTimer = Lood.UpgradeManager.Instance.CurrencySources[gadflyData.Tag].MainStats[MainStat.GadflyHarvestDuration];

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
            harvestCooldownTimer = Lood.UpgradeManager.Instance.CurrencySources[gadflyData.Tag].MainStats[MainStat.GadflyCooldown];

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
            float targetSpeed = gadfly.isHarvesting ? GetCurrentSpeed() : gadfly.wanderSpeed;

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



                        var collectStats = Lood.UpgradeManager.Instance.CurrencySources[gadflyData.Tag].CollectStats;
                        Dictionary<Currency, int> collectAmounts = new();

                        foreach (KeyValuePair<Currency, CollectTypeStatBlock> item in collectStats)
                        {
                            int collectAmount = GetCurrentCollectAmount(item.Key, gadflyData.Tag);
                            if (collectAmount != 0)
                            {
                                collectAmounts.Add(item.Key, collectAmount);
                            }
                        }

                        ProjectileDirector.Instance.SpawnHumanProjectile(
                            gadfly.position, collectAmounts
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


    public float GetCurrentSpeed()
    {
        CurrencySourceStatBlock data = Lood.UpgradeManager.Instance.CurrencySources[gadflyData.Tag];
        //Apply base speed (in ticks per second)
        float _speed = data.MainStats[MainStat.BaseSpeed];
        //Apply simple add
        _speed += data.MainStats[MainStat.SpeedAdd];
        //Apply simple multiply
        _speed *= data.MainStats[MainStat.SpeedMultiply];
        //Apply additive percentage
        _speed += _speed * data.MainStats[MainStat.SpeedAdditivePercentage] / 100;
        //Apply multiplicative percentage
        _speed += _speed * data.MainStats[MainStat.SpeedAdditivePercentage] / 100;

        return _speed;
    }

    public int GetCurrentCollectAmount(Currency currency, string tag)
    {
        //
        CurrencySourceStatBlock data = Lood.UpgradeManager.Instance.CurrencySources[tag];
        //Apply base speed (in ticks per second)
        float finalCollectAmount = data.CollectStats[currency].Stats[CollectStat.BaseCollect];
        //Apply simple add
        finalCollectAmount += data.CollectStats[currency].Stats[CollectStat.CollectAdd];
        //Apply simple multiply
        finalCollectAmount *= data.CollectStats[currency].Stats[CollectStat.CollectMultiplier];
        //Apply additive percentage
        finalCollectAmount += finalCollectAmount * data.CollectStats[currency].Stats[CollectStat.CollectAdditivePercentage] / 100;
        //Apply multiplicative percentage
        finalCollectAmount += finalCollectAmount * data.CollectStats[currency].Stats[CollectStat.CollectMultiplicativePercentage] / 100;

        //Set current total collect amount for this currency.
        data.CollectStats[currency].Stats[CollectStat.CollectTotalCurrent] = finalCollectAmount;
        return (int)finalCollectAmount;

    }
}
