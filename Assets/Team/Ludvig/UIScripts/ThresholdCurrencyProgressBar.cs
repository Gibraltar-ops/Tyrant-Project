using UnityEngine;

public class ThresholdCurrencySlider : MonoBehaviour
{
    public Currency currency;
    void Start()
    {
        
    }
    public void UpdateSlider()
    {
         long currentCurrency = EconomyManager.Instance.GetCurrent(currency);

            float normalizedProgressBarScale;

            if(currentCurrency >= 1)
            {
                normalizedProgressBarScale = Mathf.Clamp((float)currentCurrency/EconomyManager.Instance.BiomassWinThreshold, 0f, 1f);
            }
            else
            {
                normalizedProgressBarScale = 0f;
            }
            transform.localScale = new Vector3(normalizedProgressBarScale, 1f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
