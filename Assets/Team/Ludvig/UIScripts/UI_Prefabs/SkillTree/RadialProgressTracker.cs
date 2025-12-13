using System.Collections;
using Lood;
using UnityEngine;
using UnityEngine.UI;

public class RadialProgressTracker : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject upgradeButton;
    public PurchaseableSO trackedPurchaseable;

    private UpgradeType _upgradeType;
    private bool _maxedOut;
    private Currency _boughtWith;
    private float _lastValue = 0f;
    private Image image;

   

    private enum UpgradeType
    {
        currencySource,
        upgrade
    }
    void OnEnable()
    {
        EconomyManager.onCurrencyChanged += UpdateProgressBar;
    }
    void OnDisable()
    {
        EconomyManager.onCurrencyChanged -= UpdateProgressBar;
    }
    void Start()
    {
      Invoke("Initialize", 0.01f);

    }
    private void Initialize()
    {
          if(upgradeButton.GetComponent<CurrencySourceShopButton>() != null)
        {   
            trackedPurchaseable = upgradeButton.GetComponent<CurrencySourceShopButton>().CurrencySourceData;
            _upgradeType = UpgradeType.currencySource;
        }

        if(upgradeButton.GetComponent<UpgradeShopButton>() != null)
        {
            trackedPurchaseable = upgradeButton.GetComponent<UpgradeShopButton>().upgradeData;
            _upgradeType = UpgradeType.upgrade;
        }

        
        _boughtWith = trackedPurchaseable.Shop.BoughtWithCurrency;

        image = GetComponent<Image>();
        UpdateProgressBar(_boughtWith);
    }


    // Update is called once per frame
    public void UpdateProgressBar(Currency currency)
    {   
        

        if(_maxedOut) return;

        long nextPrice = 1;
        if(_upgradeType == UpgradeType.currencySource)
        {
            if (upgradeButton.GetComponent<CurrencySourceShopButton>()._maxedOut)
            {   GetComponent<Image>().enabled = false;
                _maxedOut = true;
                return;
            }

            if(currency != Lood.UpgradeManager.Instance.CurrencySources[trackedPurchaseable.Tag].Shop.BoughtWithCurrency)
            {
                return;
            }

            nextPrice = Lood.UpgradeManager.Instance.CurrencySources[trackedPurchaseable.Tag].Shop.NextPrice;
        }

        if(_upgradeType == UpgradeType.upgrade)
        {   
             if (upgradeButton.GetComponent<UpgradeShopButton>()._maxedOut)
            {   
                GetComponent<Image>().enabled = false;
                _maxedOut = true;
                return;
            }
            if(currency != Lood.UpgradeManager.Instance.Upgrades[trackedPurchaseable.Tag].Shop.BoughtWithCurrency)
            {
                return;
            }

            nextPrice = Lood.UpgradeManager.Instance.Upgrades[trackedPurchaseable.Tag].Shop.NextPrice;
        }

        StopAllCoroutines();
        _lastValue = image.fillAmount;

        long currentCurrency = EconomyManager.Instance.GetCurrent(currency);

        float normalizedProgressBarScale;

        if(currentCurrency >= 1)
        {
            normalizedProgressBarScale = Mathf.Clamp((float)currentCurrency/nextPrice, 0f, 1f);
        }
        else
        {
            normalizedProgressBarScale = 0f;
        }

        StartCoroutine(LerpPos(_lastValue, normalizedProgressBarScale, 0.2f, image));

    }

    IEnumerator LerpPos(float start, float end, float timeToMove, Image image)
	{
		float t = 0;
		while(t < 1)
		{
			image.fillAmount = Mathf.Lerp(start,end,t);
			t += Time.deltaTime / timeToMove;
			yield return null;//Stops here until right after the next update loop, then continues
		}

		//t won't end up exactly at 1, so let's snap it to be precisely where we want.
		image.fillAmount = end;
		

	}
}
