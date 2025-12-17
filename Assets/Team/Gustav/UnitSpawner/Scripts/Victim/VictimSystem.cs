using System;
using System.Collections.Generic;
using UnityEngine;

public class VictimSystem : MonoBehaviour
{
    [Header("Spawning")]
    //-----------------------------------------------------
    //Public variables deleted.
    //All spawning now draws values from the upgrade system.
    //-----------------------------------------------------

    [Tooltip("Currency Source Scriptable object")]
    public CurrencySourceSO SongData;
    public bool enableSpawning = true;
    [Tooltip("Base interval between individual spawns.")]
    public int maxVictims = 10000;

    [Header("Internal Stats")]
    public float minScale = 0.15f;
    public float maxScale = 0.16f;

    public List<SpawnRegion> spawnRegions = new List<SpawnRegion>();
    public List<DestinationRegion> destinationRegions = new List<DestinationRegion>();

    // Timers
    private float spawnTimer;
    private float waveTimer;

    public List<VictimData> victims = new List<VictimData>();
    public event Action<int> OnVictimCountChanged;

    //void Awake() // Använd Awake för att logga tidigt
    //{
    //    Debug.Log(
    //        $"VICTIMSYSTEM AWAKE! Instance ID: {this.GetInstanceID()}, Name: {this.gameObject.name}",
    //        this.gameObject
    //    );
    //} // Magnus kommentera bort det här sedan

    void Awake()
    {
        Lood.UpgradeManager.Instance.AddUpgradeableObject(SongData);
    }
    void Start()
    {
        spawnTimer = CurrentTrickleFrequency();
        waveTimer = CurrentPulseFrequency();

        // Optional: spawn initial population
        for (int i = 0; i < 10; i++)
            SpawnVictim();
    }

    void Update()
    {
        if (!enableSpawning) return;

        // -------------------------
        // INDIVIDUAL SPAWNING
        // -------------------------
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f && victims.Count < maxVictims)
        {
            int smallWaveAmount = Mathf.RoundToInt(CurrentTrickleStrength());
            for (int i = 0; i < smallWaveAmount; i++)
            {
                if (victims.Count < maxVictims)
                    SpawnVictim();
            }
            spawnTimer = CurrentTrickleFrequency();
        }

        // -------------------------
        // WAVE SPAWNING 
        // Every X seconds spawn Y victims instantly
        // -------------------------
        waveTimer -= Time.deltaTime;

        if (waveTimer <= 0f)
        {
            int largeWaveAmount = Mathf.RoundToInt(CurrentPulseStrength());

            for (int i = 0; i < largeWaveAmount; i++)
            {
                if (victims.Count < maxVictims)
                    SpawnVictim();
            }

            AudioManager.Instance.Play("TyrantLarynx");
            waveTimer = CurrentPulseFrequency();
        }

        // Simulate victim units
        SimulateVictims();
    }

    // ----------------------------------------------------
    // SPAWNING LOGIC
    // ----------------------------------------------------
    private void SpawnVictim()
    {
        if (spawnRegions.Count == 0 || destinationRegions.Count == 0)
            return;

        var spawnRegion = spawnRegions[UnityEngine.Random.Range(0, spawnRegions.Count)];
        var centerRegion = destinationRegions[UnityEngine.Random.Range(0, destinationRegions.Count)];

        victims.Add(new VictimData
        {
            position = spawnRegion.RandomPointInside(),
            homeRegion = centerRegion,
            destination = centerRegion.RandomPointInside(),
            speed = UnityEngine.Random.Range(1f, 1.1f),
            scale = UnityEngine.Random.Range(minScale, maxScale),
            wanderDirection = UnityEngine.Random.insideUnitCircle.normalized,
            wanderTimer = UnityEngine.Random.Range(1f, 2f),
            isCenter = false
        });

        OnVictimCountChanged?.Invoke(victims.Count);
    }

    // --------------------------------------------
    // MOVEMENT
    // --------------------------------------------
    private void SimulateVictims()
    {
        foreach (var victim in victims)
        {
            if (victim.isMarkedForDeath)
            {
                continue;
            }
            if (!victim.isCenter)
            {
                Vector2 dir = (victim.destination - victim.position).normalized;
                victim.position += dir * victim.speed * Time.deltaTime;

                if (Vector2.Distance(victim.position, victim.destination) < 0.1f)
                {
                    victim.isCenter = true;
                    victim.wanderTimer = UnityEngine.Random.Range(0.5f, 1.5f);
                    victim.wanderDirection = UnityEngine.Random.insideUnitCircle.normalized;
                }
            }
            else // Wandering
            {
                victim.wanderTimer -= Time.deltaTime;

                if (victim.wanderTimer <= 0f)
                {
                    victim.wanderDirection = UnityEngine.Random.insideUnitCircle.normalized;
                    victim.wanderTimer = UnityEngine.Random.Range(0.5f, 1.5f);
                }

                // Apply wandering movement
                victim.position += victim.wanderDirection * (victim.speed * 0.5f) * Time.deltaTime;

                // If victim has wandered outside the circle walk back inside
                if (!victim.homeRegion.Contains(victim.position))
                {
                    Vector2 returnPoint = victim.homeRegion.RandomPointInside();
                    Vector2 dir = (returnPoint - victim.position).normalized;

                    victim.position += dir * victim.speed * Time.deltaTime;
                }
            }
        }
    }

    // --------------------------------------------
    // Editor Gizmos
    // --------------------------------------------
    private void OnDrawGizmosSelected()
    {
        if (spawnRegions == null)
            return;

        foreach (var s in spawnRegions)
            s.DrawGizmo(Color.yellow);

        if (destinationRegions != null)
            foreach (var d in destinationRegions)
                d.DrawGizmo(Color.green);
    }

    // --------------------------------------------
    // Value calculations
    // --------------------------------------------
    public float CurrentTrickleFrequency()
    {
        CurrencySourceStatBlock data = Lood.UpgradeManager.Instance.CurrencySources[SongData.Tag];

        //Apply base frequency.
        float _trickleFrequency = data.MainStats[MainStat.SongTrickleFrequencyBase];
        //Apply additive percentage (If frequency goes up by 100%: frequency is doubled / trickle timer is halved). This calculation ensures frequency/timer never reaches zero.
        _trickleFrequency *= 1f / (1f + (data.MainStats[MainStat.SongTrickleFrequencyAdditivePercentage] / 100));

        //This number should be used as a timer value. It represents "spawns per second".
        return _trickleFrequency;
    }

    public float CurrentPulseFrequency()
    {
        CurrencySourceStatBlock data = Lood.UpgradeManager.Instance.CurrencySources[SongData.Tag];

        //Apply base frequency.
        float _pulseFrequency = data.MainStats[MainStat.SongPulseFrequencyBase];
        //Apply additive percentage (If frequency goes up by 100%: frequency is doubled / pulse timer is halved). This calculation ensures frequency/timer never reaches zero.
        _pulseFrequency *= 1f / (1f + (data.MainStats[MainStat.SongPulseFrequencyAdditivePercentage] / 100));

        //This number should be used as a timer value. It represents "spawns per second".
        return _pulseFrequency;
    }

    public float CurrentTrickleStrength()
    {
        CurrencySourceStatBlock data = Lood.UpgradeManager.Instance.CurrencySources[SongData.Tag];

        //Apply base strength
        float _trickleStrength = data.MainStats[MainStat.SongTrickleStrengthBase];

        //Apply add
        _trickleStrength += data.MainStats[MainStat.SongTrickleStrengthAdd];

        //Apply additivePercentage
        _trickleStrength += _trickleStrength * data.MainStats[MainStat.SongTrickleStrengthAdditivePercentage];

        //This number represents a simple spawn value every time the trickle spawn happens.
        return _trickleStrength;
    }

    public float CurrentPulseStrength()
    {
        CurrencySourceStatBlock data = Lood.UpgradeManager.Instance.CurrencySources[SongData.Tag];

        //Apply base strength
        float _pulseStrength = data.MainStats[MainStat.SongPulseStrengthBase];

        //Apply add
        _pulseStrength += data.MainStats[MainStat.SongTrickleStrengthAdd];

        //Apply additivePercentage
        _pulseStrength += _pulseStrength * data.MainStats[MainStat.SongPulseStrengthAdditivePercentage];

        //This number represents a simple spawn value every time the trickle spawn happens.
        return _pulseStrength;
    }
}
