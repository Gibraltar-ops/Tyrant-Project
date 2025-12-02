using UnityEngine;

public class Collector
{
    public float CurrentTime;
    public float NormalizedTime;
    public bool CanCollect = true;

    public Collector(float currentTime)
    {
        CurrentTime = currentTime;
    }

    public bool Collect(float timeStep, float ticksPerSecond)
    {
        float targetTime = 1f / ticksPerSecond;
        CurrentTime += timeStep;
        NormalizedTime = CurrentTime / targetTime;

        if (CurrentTime >= targetTime)
        {
            CurrentTime = 0f;
            if (CanCollect)
            {
                return true;
            }
            
        }

        return false;
    }
    public void Reset()
    {
        CurrentTime = 0f;
    }
    public void SetCanCollect(bool value)
    {
        CanCollect = value;
    }
}

