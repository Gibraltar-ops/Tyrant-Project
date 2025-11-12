using UnityEngine;
using UnityEngine.UI;

public class Incrementor : MonoBehaviour
{
    public ObjectTag IncrementorTag;
    private ValueHolder _values;
    private Button _buyButton;
    private float _currentTime = 0f;
    public bool Active = true;
    public Slider slider;

    public delegate void OnIncrementorFinished(ObjectTag tag);
    public static event OnIncrementorFinished onIncrementorFinished;



    void Start()
    {
        _values = GameObject.Find("ValueHolder").GetComponent<ValueHolder>();
        _buyButton = GameObject.Find("UpgradeButton").GetComponent<Button>();

    }


    void Update()
    {
        // float targetTime = 1f / (_values.IncrementorTicksPerSecond * _values.IncrementorTicksPerSecondMultiplier);

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

        // slider.normalizedValue = _currentTime / targetTime;
    }
}
