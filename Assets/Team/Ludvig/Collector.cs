using UnityEngine;

public class Collector
{
    public float CurrentTime;
    public float NormalizedTime;

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
            return true;
        }

        return false;
    }
}

