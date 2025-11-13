using UnityEngine;

public class Collector
{
    public ObjectTag IncrementorTag;
    private ValueHolder _values;
    public bool Active = true;

    public delegate void OnCollectorFinished(ObjectTag tag);
    public static event OnCollectorFinished onCollectorFinished;


    public void Update()
    {
        //float targetTime = 1f / (_values.IncrementorTicksPerSecond * _values.IncrementorTicksPerSecondMultiplier);

        // if (Active)
        // {
        //     if (_currentTime <= targetTime)
        //     {
        //         _currentTime += Time.deltaTime;

        //     }
        //     else
        //     {


        //         onIncrementorFinished?.Invoke(IncrementorTag);

        //         if (_values.Currency1 >= _values.GetPriceOfNextUpgrade())
        //         {
        //             _values.IncrementorCanAfford = true;
        //             _buyButton.interactable = true;
        //         }
        //         else
        //         {
        //             _values.IncrementorCanAfford = false;
        //             _buyButton.interactable = false;
        //         }

        //         _currentTime = 0f;

        //         //Debug.Log(_values.Currency1);
        //     }
        // }
    }
}
