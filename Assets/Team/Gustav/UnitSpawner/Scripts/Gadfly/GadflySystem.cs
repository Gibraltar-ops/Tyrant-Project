using System.Collections.Generic;
using UnityEngine;

public class GadflySystem : MonoBehaviour
{
    [Header("General Settings")]
    public CurrencySourceSO gadflyData;
    public int maxGadflyUnits = 500;

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
    // START 
    // --------------------------------------------
    void Start()
    {
        // Spawn some initial crowd (optional)
        // for (int i = 0; i < 1; i++)
        //     SpawnGatfly();
    }

    // --------------------------------------------
    // UPDATE Simulation
    // --------------------------------------------
    void Update()
    {
        SimulateAndUpdateGadflies();
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

        float currentCooldown = Lood.UpgradeManager.Instance.CurrencySources[gadflyData.Tag].MainStats[MainStat.GadflyCooldown];

        gadflies.Add(
            new GadflyData
            {
                position = spawnRegion.RandomPointInside(),
                homeRegion = gatherRegion,
                harvestRegion = harvestRegion,
                destination = gatherRegion.RandomPointInside(),

                state = GadflyData.GadflyState.IdleAtHome,
                personalCooldownTimer = currentCooldown + Random.Range(0f, 2f),
                personalHarvestTimer = 0f,

                wanderSpeed = Random.Range(2f, 2.1f),
                harvestSpeed = GetCurrentSpeed(),
                scale = Random.Range(0.2f, 0.21f),

                wanderDirection = Random.insideUnitCircle.normalized,
                wanderTimer = Random.Range(1f, 2f),

                velocity = Vector2.zero,
                arcStrength = Random.Range(-0.6f, 0.6f),
                jitterStrength = Random.Range(0.6f, 1.2f),

                isHome = true,
                hasPayload = false,
                isHarvesting = false
            }
        );
    }

    // --------------------------------------------
    // MOVEMENT & LOGIC Simulation
    // --------------------------------------------
    private void SimulateAndUpdateGadflies()
    {

        float baseCooldown = Lood.UpgradeManager.Instance.CurrencySources[gadflyData.Tag].MainStats[MainStat.GadflyCooldown];
        float baseHarvestDuration = Lood.UpgradeManager.Instance.CurrencySources[gadflyData.Tag].MainStats[MainStat.GadflyHarvestDuration];
        float speed = GetCurrentSpeed();

        foreach (var gadfly in gadflies)
        {
            // --- 1. STATE MACHINE LOGIC ---

            switch (gadfly.state)
            {
                case GadflyData.GadflyState.IdleAtHome:
                    gadfly.personalCooldownTimer -= Time.deltaTime;


                    gadfly.isHome = true;
                    gadfly.hasPayload = false;
                    gadfly.isHarvesting = false;


                    if (gadfly.personalCooldownTimer <= 0f)
                    {
                        gadfly.state = GadflyData.GadflyState.TravelToHarvest;
                        gadfly.destination = gadfly.harvestRegion.RandomPointInside();
                        gadfly.isHome = false;
                    }
                    break;

                case GadflyData.GadflyState.TravelToHarvest:
                    gadfly.isHome = false;
                    gadfly.isHarvesting = true;
                    break;

                case GadflyData.GadflyState.Harvesting:
                    gadfly.personalHarvestTimer -= Time.deltaTime;
                    gadfly.isHome = false;


                    if (gadfly.personalHarvestTimer <= 0f)
                    {
                        gadfly.state = GadflyData.GadflyState.ReturningHome;
                        gadfly.destination = gadfly.homeRegion.RandomPointInside();
                        gadfly.hasPayload = true;
                    }
                    break;

                case GadflyData.GadflyState.ReturningHome:
                    gadfly.isHome = false;
                    break;
            }

            // --- 2. MOVEMENT PHYSICS  ---


            float targetSpeed = (gadfly.state == GadflyData.GadflyState.IdleAtHome) ? gadfly.wanderSpeed : speed;

            Vector2 toTarget = (gadfly.destination - gadfly.position);
            Vector2 targetDir = toTarget.normalized;
            float distToTarget = toTarget.magnitude;

            // Arc Flight
            if (!gadfly.isHome)
            {
                Vector2 perp = new Vector2(-targetDir.y, targetDir.x);
                targetDir += perp * gadfly.arcStrength;
                targetDir.Normalize();
            }

            // Smooth velocity blend
            gadfly.velocity = Vector2.Lerp(gadfly.velocity, targetDir * targetSpeed, Time.deltaTime * 3.5f);

            // Apply "Micro Jitter" when home
            if (gadfly.isHome)
            {
                Vector2 jitter = Random.insideUnitCircle * gadfly.jitterStrength;
                gadfly.velocity += jitter * 0.4f;
            }

            // Move Gadfly
            gadfly.position += gadfly.velocity * Time.deltaTime;

            // --- 3. DESTINATION REACHED CHECK ---

            if (distToTarget < 0.45f)
            {


                if (gadfly.state == GadflyData.GadflyState.TravelToHarvest)
                {

                    gadfly.state = GadflyData.GadflyState.Harvesting;
                    gadfly.hasReachedHarvest = true;
                    gadfly.personalHarvestTimer = baseHarvestDuration;
                }
                else if (gadfly.state == GadflyData.GadflyState.ReturningHome)
                {

                    gadfly.state = GadflyData.GadflyState.IdleAtHome;
                    gadfly.personalCooldownTimer = baseCooldown;


                    ThrowPayload(gadfly);


                    gadfly.destination = gadfly.homeRegion.RandomPointInside();
                }
                else if (gadfly.state == GadflyData.GadflyState.IdleAtHome)
                {

                    HandleIdleWander(gadfly);
                }
            }

            // --- 4. IDLE WANDER LOGIC ---
            if (gadfly.state == GadflyData.GadflyState.IdleAtHome)
            {
                HandleIdleWander(gadfly);
            }
        }
    }


    private void ThrowPayload(GadflyData gadfly)
    {

        if (gadfly.hasPayload)
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


    private void HandleIdleWander(GadflyData gadfly)
    {
        gadfly.wanderTimer -= Time.deltaTime;

        if (gadfly.wanderTimer <= 0f)
        {
            gadfly.wanderDirection = Random.insideUnitCircle.normalized;
            gadfly.wanderTimer = Random.Range(1f, 2f);
        }


        if (Vector2.Distance(gadfly.position, gadfly.destination) < 0.5f || gadfly.wanderTimer <= 0f)
        {
            gadfly.destination = gadfly.position + gadfly.wanderDirection * Random.Range(0.4f, 0.8f);
        }

        if (!gadfly.homeRegion.Contains(gadfly.destination))
            gadfly.destination = gadfly.homeRegion.RandomPointInside();
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
        float _speed = data.MainStats[MainStat.BaseSpeed];
        _speed += data.MainStats[MainStat.SpeedAdd];
        _speed *= data.MainStats[MainStat.SpeedMultiply];
        _speed += _speed * data.MainStats[MainStat.SpeedAdditivePercentage] / 100;
        _speed += _speed * data.MainStats[MainStat.SpeedAdditivePercentage] / 100;

        return _speed;
    }

    public int GetCurrentCollectAmount(Currency currency, string tag)
    {
        CurrencySourceStatBlock data = Lood.UpgradeManager.Instance.CurrencySources[tag];
        float finalCollectAmount = data.CollectStats[currency].Stats[CollectStat.BaseCollect];
        finalCollectAmount += data.CollectStats[currency].Stats[CollectStat.CollectAdd];
        finalCollectAmount *= data.CollectStats[currency].Stats[CollectStat.CollectMultiplier];
        finalCollectAmount += finalCollectAmount * data.CollectStats[currency].Stats[CollectStat.CollectAdditivePercentage] / 100;
        finalCollectAmount += finalCollectAmount * data.CollectStats[currency].Stats[CollectStat.CollectMultiplicativePercentage] / 100;

        data.CollectStats[currency].Stats[CollectStat.CollectTotalCurrent] = finalCollectAmount;
        return (int)finalCollectAmount;
    }
}