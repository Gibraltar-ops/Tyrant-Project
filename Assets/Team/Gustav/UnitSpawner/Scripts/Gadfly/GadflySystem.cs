using System.Collections.Generic;
using UnityEngine;

public class GadflySystem : MonoBehaviour
{
    [Header("General Settings")]
    public CurrencySourceSO gadflyData;
    public int maxGadflyUnits = 500;

    [Header("REGION Zones")]
    public List<SpawnRegion> spawnArea = new();


    public List<DestinationRegion> gatherAreaFront = new();
    public List<DestinationRegion> gatherAreaBack = new();

    public List<DestinationRegion> harvestAreaLeft = new();
    public List<DestinationRegion> harvestAreaRight = new();

    [Header("Layer Settings")]
    public string frontLayerName = "Gadflies_Front";
    public string backLayerName = "Gadflies_Behind";


    public List<GadflyData> gadflies = new();

    void Awake()
    {
        //Lood.UpgradeManager.Instance.AddUpgradeableObject(gadflyData);
    }

    void Start()
    {
        // Optional start spawn
    }

    void OnEnable()
    {
        EconomyManager.onCurrencyChanged += UpdateValues;
    }

    void OnDisable()
    {
        EconomyManager.onCurrencyChanged -= UpdateValues;
    }

    void Update()
    {
        SimulateAndUpdateGadflies();
    }

    // --------------------------------------------
    // SPAWNING 
    // --------------------------------------------
    public void SpawnGatfly()
    {
        if (gadflies.Count >= maxGadflyUnits) return;

        var spawnRegion = spawnArea[Random.Range(0, spawnArea.Count)];
        var harvestRegion = GetRandomHarvestRegion();
        if (harvestRegion == null) return;

        bool startInFront = Random.value > 0.5f;
        var targetList = startInFront ? gatherAreaFront : gatherAreaBack;

        if (targetList.Count == 0) targetList = startInFront ? gatherAreaBack : gatherAreaFront;
        if (targetList.Count == 0) return;

        var gatherRegion = targetList[Random.Range(0, targetList.Count)];
        string startLayer = startInFront ? frontLayerName : backLayerName;

        float currentCooldown = Lood.UpgradeManager.Instance.CurrencySources[gadflyData.Tag].MainStats[MainStat.GadflyCooldown];

        gadflies.Add(
            new GadflyData
            {
                position = spawnRegion.RandomPointInside(),
                homeRegion = gatherRegion,
                harvestRegion = harvestRegion,
                destination = gatherRegion.RandomPointInside(),

                currentLayerName = startLayer,

                noiseOffset = Random.Range(0f, 100f),
                swarmSpeed = Random.Range(0.5f, 1.5f),

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

                    HandleSwarmBehavior(gadfly);

                    if (gadfly.personalCooldownTimer <= 0f)
                    {
                        gadfly.state = GadflyData.GadflyState.TravelToHarvest;
                        gadfly.harvestRegion = GetRandomHarvestRegion();

                        if (gadfly.harvestRegion != null)
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

            if (!gadfly.isHome)
            {
                Vector2 perp = new Vector2(-targetDir.y, targetDir.x);
                targetDir += perp * gadfly.arcStrength;
                targetDir.Normalize();
            }
            else
            {
                float noiseX = Mathf.PerlinNoise(Time.time * gadfly.swarmSpeed, gadfly.noiseOffset) - 0.5f;
                float noiseY = Mathf.PerlinNoise(Time.time * gadfly.swarmSpeed, gadfly.noiseOffset + 50f) - 0.5f;
                Vector2 noiseDir = new Vector2(noiseX, noiseY).normalized;

                targetDir = Vector2.Lerp(targetDir, noiseDir, 0.6f).normalized;
            }

            // Smooth velocity blend
            gadfly.velocity = Vector2.Lerp(gadfly.velocity, targetDir * targetSpeed, Time.deltaTime * 3.5f);

            // Move Gadfly
            gadfly.position += gadfly.velocity * Time.deltaTime;

            // --- 3. DESTINATION REACHED CHECK ---
            if (distToTarget < 3f)
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

                    PickNewSwarmDestination(gadfly);
                }
                else if (gadfly.state == GadflyData.GadflyState.IdleAtHome)
                {
                    PickNewSwarmDestination(gadfly);
                }
            }
        }
    }

    private DestinationRegion GetRandomHarvestRegion()
    {
        bool goLeft = Random.value > 0.5f;
        var targetList = goLeft ? harvestAreaLeft : harvestAreaRight;

        if (targetList.Count == 0) targetList = goLeft ? harvestAreaRight : harvestAreaLeft;

        if (targetList.Count == 0) return null;

        return targetList[Random.Range(0, targetList.Count)];
    }
    private void PickNewSwarmDestination(GadflyData gadfly)
    {
        bool switchToFront = Random.value > 0.5f;

        var targetList = switchToFront ? gatherAreaFront : gatherAreaBack;
        if (targetList.Count == 0) targetList = switchToFront ? gatherAreaBack : gatherAreaFront;

        gadfly.homeRegion = targetList[Random.Range(0, targetList.Count)];
        gadfly.destination = gadfly.homeRegion.RandomPointInside();

        gadfly.currentLayerName = switchToFront ? frontLayerName : backLayerName;

        // Reset timer
        gadfly.wanderTimer = Random.Range(1f, 3f);
    }

    private void HandleSwarmBehavior(GadflyData gadfly)
    {
        gadfly.wanderTimer -= Time.deltaTime;

        if (gadfly.wanderTimer <= 0f)
        {
            PickNewSwarmDestination(gadfly);
        }

        if (!gadfly.homeRegion.Contains(gadfly.destination))
            gadfly.destination = gadfly.homeRegion.RandomPointInside();
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
            AudioManager.Instance.Play("HumanScream");
        }
    }

    public void UpdateValues(Currency currency)
    {
        var collectStats = Lood.UpgradeManager.Instance.CurrencySources[gadflyData.Tag].CollectStats;
        foreach (KeyValuePair<Currency, CollectTypeStatBlock> item in collectStats)
        {
            GetCurrentCollectAmount(item.Key, gadflyData.Tag);
        }
    }

    private void OnDrawGizmosSelected()
    {
        foreach (var s in spawnArea) s.DrawGizmo(Color.yellow);
        foreach (var g in gatherAreaFront) g.DrawGizmo(Color.cyan);
        foreach (var g in gatherAreaBack) g.DrawGizmo(Color.blue);
        foreach (var h in harvestAreaLeft) h.DrawGizmo(Color.red);
        foreach (var h in harvestAreaRight) h.DrawGizmo(Color.magenta);
    }

    public float GetCurrentSpeed()
    {
        CurrencySourceStatBlock data = Lood.UpgradeManager.Instance.CurrencySources[gadflyData.Tag];
        float _speed = data.MainStats[MainStat.BaseSpeed];
        _speed += data.MainStats[MainStat.SpeedAdd];
        _speed *= data.MainStats[MainStat.SpeedMultiply];
        _speed *= 1 + (data.MainStats[MainStat.SpeedAdditivePercentage] / 100);
        _speed += _speed * data.MainStats[MainStat.SpeedAdditivePercentage] / 100;

        return _speed;
    }

    public int GetCurrentCollectAmount(Currency currency, string tag)
    {
        CurrencySourceStatBlock data = Lood.UpgradeManager.Instance.CurrencySources[tag];
        float finalCollectAmount = data.CollectStats[currency].Stats[CollectStat.BaseCollect];
        finalCollectAmount += data.CollectStats[currency].Stats[CollectStat.CollectAdd];
        finalCollectAmount *= data.CollectStats[currency].Stats[CollectStat.CollectMultiplier];
        finalCollectAmount = finalCollectAmount * (1 + (data.CollectStats[currency].Stats[CollectStat.CollectAdditivePercentage] / 100));
        finalCollectAmount += finalCollectAmount * data.CollectStats[currency].Stats[CollectStat.CollectMultiplicativePercentage] / 100;

        data.CollectStats[currency].Stats[CollectStat.CollectTotalCurrent] = finalCollectAmount;
        return (int)finalCollectAmount;
    }
}