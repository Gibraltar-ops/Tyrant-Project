using UnityEngine;

public class ValueHolder : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int Currency1 = 0;

    //The incrementor performs IncrementorTicksPerSecond ticks per second.
    public float IncrementorTicksPerSecond = 1f;
    public float IncrementorTicksPerSecondMultiplier = 1f;


    public int IncrementorBasePrice;
    public float IncrementorPriceGrowthRate;
    public int IncrementorCount;

    //The collect value is the currency value collected for every tick of the Incrementor.
    public int IncrementorBaseCollect = 10;
    public float IncrementorCollectMultiplier = 1f;

    // Update is called once per frame

    public int GetPriceOfNextUpgrade()
    {
        float nextPrice = IncrementorBasePrice * Mathf.Pow(IncrementorPriceGrowthRate, IncrementorCount);
        return (int)nextPrice;
    }

    void Update()
    {

    }
}
