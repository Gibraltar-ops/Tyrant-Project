using UnityEngine;

public class ResistanceWaveDirector : MonoBehaviour
{
    public ResistanceSystem resistanceSystem;

    [Header("Wave Timing")]
    public float waveInterval = 30f;
    private float waveTimer;

    [Header("Budget Logic")]
    public int baseBudget = 0;   // starting per-wave income (Starts zero as to have no spawn early)
    public int perWaveIncrease = 1; // baseBudget += this every wave
    public int upgradeBonus = 2;  // added whenever external upgrades call AddUpgrade()

    private int currentBudget = 0;  // stored points that carry over
    private int waveIndex = 0;  // If I want to track wave number
    private bool altWaveFlag = false; // Flag to track every other wave

    [Header("Enemy Costs")]
    public int basicEnemyCost = 10;

    void Start()
    {
        waveTimer = waveInterval;
    }

    void Update()
    {
        waveTimer -= Time.deltaTime;

        if (waveTimer <= 0f)
        {
            RunWave();
            waveTimer = waveInterval;
        }
    }

    // ---------------------------------------------
    // Called every time waveIntervall == 0 (30 seconds)
    // ---------------------------------------------
    private void RunWave()
    {
        waveIndex++;

        // Add per-wave income
        currentBudget += baseBudget;

        // Increase base income for future waves (every other wave)
        if (altWaveFlag)
            baseBudget += perWaveIncrease;

        if (currentBudget >= 100)   // Ensure budget cap
            currentBudget = 100;

        altWaveFlag = !altWaveFlag;

        SpawnEnemiesFromBudget();
    }

    // ---------------------------------------------
    // Spawn as many enemies as possible
    // ---------------------------------------------
    private void SpawnEnemiesFromBudget()
    {
        int spawned = 0;

        // Try to buy as many as possible
        while (currentBudget >= basicEnemyCost &&
               resistanceSystem.fighters.Count < resistanceSystem.maxResistanceUnits)
        {
            resistanceSystem.SpawnResistance();
            currentBudget -= basicEnemyCost;
            spawned++;
        }

        //// Optional debug log
        // Debug.Log($"Wave {waveIndex}: spawned {spawned} fighters. Remaining budget: {currentBudget}");
    }

    // ---------------------------------------------
    // Called by upgrades
    // ---------------------------------------------
    public void AddUpgrade()
    {
        baseBudget += upgradeBonus;

        //// Optional debug log
        // Debug.Log("Upgrade purchased — future wave budget increased!");
    }
}