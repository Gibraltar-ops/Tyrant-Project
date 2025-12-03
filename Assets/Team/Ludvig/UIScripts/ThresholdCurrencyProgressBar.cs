using UnityEngine;
using UnityEngine.SceneManagement;

public class ThresholdCurrencySlider : MonoBehaviour
{
    public Currency Currency;
    private bool _thresholdMet = false;

    public delegate void OnWinThresholdMet();
    public static event OnWinThresholdMet onWinThresholdMet;
    void OnEnable()
    {
        EconomyManager.onCurrencyChanged += UpdateSlider;
    }
    void OnDisable()
    {
        EconomyManager.onCurrencyChanged -= UpdateSlider;
    }

    void Start()
    {
        transform.localScale = new Vector3(0f, 1f, 1f);
    }

    public void UpdateSlider(Currency currency)
    {
        if(!_thresholdMet)
        {
            long currentCurrency = EconomyManager.Instance.GetCurrent(Currency.BiomassLifetime);

            

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

            if(currentCurrency >= EconomyManager.Instance.BiomassWinThreshold)
            {
                _thresholdMet = true;
                onWinThresholdMet?.Invoke();
                EndGame();
            }

            
        }
         
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void EndGame()
    {
        SceneManager.LoadScene("OutroCinematic");
    }
}
