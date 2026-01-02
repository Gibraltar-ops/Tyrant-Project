using UnityEngine;

public class EatManager : MonoBehaviour
{
    public static EatManager Instance;
    public long totalHumansConsumed = 0;

    public long earthPopulation = 8267569687;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        TyrantTentacleManager.VictimEaten += IncrementEatenCount;
    }
    private void OnDisable()
    {
        TyrantTentacleManager.VictimEaten -= IncrementEatenCount;
    }
    private void IncrementEatenCount()
    {
        totalHumansConsumed++;
    }

    public void ResetStats() /// Call on this when leaving scene (To main menu)
    {
        totalHumansConsumed = 0;
    }
    public double PercentageOfEarthEaten()
    {
        if (earthPopulation <= 0) return 0;

        return ((double)totalHumansConsumed / earthPopulation) * 100.0;
    }

    public string GetStatsSummary()
    {
        double percent = PercentageOfEarthEaten();
        string standardMessage = $"You consumed {totalHumansConsumed} humans. That is {percent.ToString("F8")}% of this planet.";

        if (percent >= 50.0)
        {
            return standardMessage + " Are you still hungry?";
        }

        return standardMessage;
    }
}
