using UnityEngine;
using UnityEngine.UI;

public class Incrementor : MonoBehaviour
{
    public ValueHolder Values;
    private float _currentTime = 0f;
    public bool Active = true;
    public Slider slider;

    void Start()
    {


    }


    void Update()
    {
        float targetTime = 1f / (Values.IncrementorTicksPerSecond * Values.IncrementorTicksPerSecondMultiplier);

        if (Active)
        {
            if (_currentTime <= targetTime)
            {
                _currentTime += Time.deltaTime;

            }
            else
            {
                Values.Currency1 += (int)(Values.IncrementorBaseCollect * Values.IncrementorCollectMultiplier);
                _currentTime = 0f;

                Debug.Log(Values.Currency1);
            }
        }

        slider.normalizedValue = _currentTime / targetTime;
    }
}
