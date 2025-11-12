using System;
using Unity.Burst.CompilerServices;
using UnityEngine;

public enum ObjectTag
{
    Tentacle,
    Monster
}

public enum Currencies
{
    Biomass,
    Brains,
    Keratin
}

public struct Currency
{
    public int Value;
}


[Serializable]
public class IncrementorType
{
    public Currency CurrencyIncremented;

    //The incrementor performs TicksPerSecond ticks per second.
    public float TicksPerSecond = 1f;
    public float TicksPerSecondMultiplier = 1f;


    public int BasePrice = 12;
    public float PriceGrowthRate = 1.1f;
    public int NumberOwned = 0;
    public int currentPrice;

    //The collect value is the currency value collected for every tick of the Incrementor.
    public int BaseCollect = 10;
    public float CollectMultiplier = 1f;

    public IncrementorType(Currency currency)
    {
        CurrencyIncremented = currency;
    }

    public void Increment()
    {
        CurrencyIncremented.Value += (int)(BaseCollect * CollectMultiplier);
    }
}

public class ValueHolder : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // public int Currency1 = 0;

    // //The incrementor performs IncrementorTicksPerSecond ticks per second.
    // public float IncrementorTicksPerSecond = 1f;
    // public float IncrementorTicksPerSecondMultiplier = 1f;


    // public int IncrementorBasePrice = 12;
    // public float IncrementorPriceGrowthRate = 1.1f;
    // public int IncrementorCount = 1;
    // public bool IncrementorCanAfford = false;

    //[SerializeField]


    public Currency Biomass = new Currency();

    private IncrementorType Tentacles;



    //The collect value is the currency value collected for every tick of the Incrementor.
    // public int IncrementorBaseCollect = 10;
    // public float IncrementorCollectMultiplier = 1f;

    void OnEnable()
    {
        Incrementor.onIncrementorFinished += HandleIncrementorFinished;

        Tentacles = new IncrementorType(Biomass);
    }

    void OnDisable()
    {
        Incrementor.onIncrementorFinished -= HandleIncrementorFinished;
    }

    void Start()
    {

    }

    public void HandleIncrementorFinished(ObjectTag tag)
    {
        switch (tag)
        {
            case ObjectTag.Tentacle:
                OnTentacleFinished();
                break;
            case ObjectTag.Monster:
                break;
            default:
                break;
        }
    }

    public void OnTentacleFinished()
    {
        Tentacles.Increment();
    }
    // Update is called once per frame

    public int GetPriceOfNextUpgrade()
    {
        float nextPrice = Tentacles.BasePrice * Mathf.Pow(Tentacles.PriceGrowthRate, Tentacles.NumberOwned);
        return (int)nextPrice;
    }

    void Update()
    {

    }
}
