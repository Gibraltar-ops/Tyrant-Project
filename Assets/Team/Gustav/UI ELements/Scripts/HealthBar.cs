using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class HealthBar : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Data")]
    public CurrencySourceSO HealthData;
    [Header("References")]
    public Slider slider1;
    public Slider slider2;



    public TextMeshProUGUI percentageText;              // Shows 0�100% health
    public TextMeshProUGUI currentHealthText;           // Shows raw current health value
    public Image hoverHideImage;                        // Image to hide when mouse hovers

    [Header("Smoothing")]
    public float smoothness = 5f;
    public Image fillImage1;
    public Image fillImage2;
    public Color damageFlash = Color.white;
    public float flashDuration = 0.1f; //Flash duration
    public float shakeMagnitude = 2f;

    [Header("Variables & Settings")]
    // public float maxHealth = 10000;   // The maximum health value // "HealthMax" Use to calculate bar length
    // public float minHealth = 0;       // The minimum health value // "Nil"
    // public float regenValue = 10;     // The amount restored per regen cooldown // "HealthPointsCollect" - Standard collection pipeline.
    // public float regenMultiplier = 1;  // The multiplier for health regeneration // "HealthPointsCollect" - Standard collection pipeline.
    // public float regenCooldown = 1;    // The set cooldown for health regeneration // "BaseSpeed" and apply Additive percentage: current * 1/1+(x/100) - Show as "Increase speed of regen" or something.
    // public float eatValue = 10;       // The amount restored when eating a victim //Apply to the things that actually collect victims instead. (as a HP up upgrade).

    private float currentHealth;
    private float regenTimer;
    private Color originalColor1;
    private Color originalColor2;
    private Coroutine activeHealthBarCoroutine;
    private Vector3 initialPosition;

    void OnEnable()
    {
        EconomyManager.onCurrencyChanged += UpdateHealthBar;
    }
    void Awake()
    {
        Lood.UpgradeManager.Instance.AddUpgradeableObject(HealthData);
    }
    void OnDisable()
    {
        EconomyManager.onCurrencyChanged -= UpdateHealthBar;
    }
    void Start()
    {
        // Initialize current health
        EconomyManager.Instance.HealthPoints = (long)CurrentMaxHealth();

        initialPosition = transform.localPosition;

        // Initialize sliders
        if (slider1 != null) { slider1.maxValue = (long)CurrentMaxHealth(); slider1.value = EconomyManager.Instance.HealthPoints; }
        if (slider2 != null) { slider2.maxValue = (long)CurrentMaxHealth(); slider2.value = EconomyManager.Instance.HealthPoints; }

        if (fillImage1 != null) originalColor1 = fillImage1.color;
        if (fillImage2 != null) originalColor2 = fillImage2.color;

        UpdateText();
    }

    void Update()
    {
        // Handle regeneration every cooldown
        regenTimer += Time.deltaTime;
        if (regenTimer >= CurrentCooldown())
        {
            regenTimer = 0f;
            Regenerate();
        }
    }

    private void Regenerate()
    {
        var collectStats = Lood.UpgradeManager.Instance.CurrencySources[HealthData.Tag].CollectStats;

        //The currencies collected are held in a dictionary "CollectStats". 
        //Key: Currency, Value: CollectTypeStatBlock (Holds stuff like BaseCollect and multipliers.)
        foreach (KeyValuePair<Currency, CollectTypeStatBlock> item in collectStats)
        {
            int collectAmount = GetCurrentCollectAmount(item.Key, HealthData.Tag);
            if (collectAmount != 0)
            {
                CollectCurrency(item.Key, collectAmount);
            }
        }
        // if (currentHealth >= maxHealth) return;

        // currentHealth += regenValue * regenMultiplier;
        // currentHealth = Mathf.Clamp(currentHealth, minHealth, maxHealth);

        UpdateUI();
    }

    private void CollectCurrency(Currency collectedCurrency, int amount)
    {
        EconomyManager.Instance.IncrementCurrency(collectedCurrency, amount);
    }

    private void UpdateUI()
    {
        if (slider1 != null)
            slider1.value = EconomyManager.Instance.HealthPoints;

        if (slider2 != null)
            slider2.value = EconomyManager.Instance.HealthPoints;

        if (percentageText != null)
            percentageText.text = Mathf.RoundToInt(EconomyManager.Instance.HealthPoints / CurrentMaxHealth() * 100f) + "%";

        if (currentHealthText != null)
            currentHealthText.text = Mathf.RoundToInt(EconomyManager.Instance.HealthPoints).ToString();
    }

    public void UpdateHealthBar(Currency currency)
    {



        if (currency != Currency.HealthPoints)
        {
            return;
        }

        if (EconomyManager.Instance.HealthPoints < slider1.value)
        {
            StopCoroutine("FlashDamageEffect");
            StartCoroutine(FlashDamageEffect());

            // if (currentHealth <= 0)
            // {
            //     Death();
            // }
        }
        UpdateUI();
        UpdateText();
        UpdateHealthBarVisuals();
    }
    /// <summary>
    /// Takes damage when isHealing = false.
    /// Heals when isHealing = true.
    /// </summary>
    public void ModifyHealth(float amount, bool isHealing)
    {

        // if (isHealing)
        //     currentHealth += amount;
        // else
        //     currentHealth -= amount;

        // currentHealth = Mathf.Clamp(currentHealth, minHealth, maxHealth);

        // UpdateText();
        // UpdateHealthBarVisuals();

        // if (!isHealing)
        // {
        //     StopCoroutine("FlashDamageEffect");
        //     StartCoroutine(FlashDamageEffect());

        //     if (currentHealth <= 0)
        //     {
        //         Death();
        //     }
        // }
    }

    private void UpdateText()
    {
        if (percentageText != null)
            percentageText.text = Mathf.RoundToInt(EconomyManager.Instance.HealthPoints / CurrentMaxHealth() * 100f) + "%";

        if (currentHealthText != null)
            currentHealthText.text = Mathf.RoundToInt(EconomyManager.Instance.HealthPoints).ToString();
    }

    private void UpdateHealthBarVisuals()
    {
        if (activeHealthBarCoroutine != null)
            StopCoroutine(activeHealthBarCoroutine);

        activeHealthBarCoroutine = StartCoroutine(AnimateHealthBar());
    }

    private IEnumerator AnimateHealthBar()
    {
        float currentSliderValue = (slider1 != null) ? slider1.value : EconomyManager.Instance.HealthPoints;
        float currentHealth = EconomyManager.Instance.HealthPoints;

        while (Mathf.Abs(currentSliderValue - EconomyManager.Instance.HealthPoints) > 1f)
        {
            currentSliderValue = Mathf.Lerp(
                currentSliderValue,
             currentHealth,
            Time.deltaTime * smoothness
              );

            if (slider1 != null) slider1.value = currentSliderValue;
            if (slider2 != null) slider2.value = currentSliderValue;

            yield return null;
        }
        if (slider1 != null) slider1.value = currentHealth;
        if (slider2 != null) slider2.value = currentHealth;

        activeHealthBarCoroutine = null;
    }

    private IEnumerator FlashDamageEffect()
    {
        if (fillImage1 != null) fillImage1.color = damageFlash;
        if (fillImage2 != null) fillImage2.color = damageFlash;

        float elapsed = 0f;

        while (elapsed < flashDuration)
        {
            float offsetX = UnityEngine.Random.Range(-1f, 1f) * shakeMagnitude;
            float offsetY = UnityEngine.Random.Range(-1f, 1f) * shakeMagnitude;

            transform.localPosition = initialPosition + new Vector3(offsetX, offsetY, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = initialPosition;

        /*  yield return new WaitForSeconds(flashDuration); */

        // Återställ till originalfärgerna
        if (fillImage1 != null) fillImage1.color = originalColor1;
        if (fillImage2 != null) fillImage2.color = originalColor2;
    }

    // Mouse hover logic
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverHideImage != null)
            hoverHideImage.enabled = false;

        if (currentHealthText != null)
            currentHealthText.enabled = true;
        /*  Debug.Log("ENTER"); */
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (hoverHideImage != null)
            hoverHideImage.enabled = true;

        if (currentHealthText != null)
            currentHealthText.enabled = false;
        /* Debug.Log("EXIT"); */
    }

    private bool isDead = false;
    public void Death()
    {
        if (isDead || currentHealth > 0) return;

        isDead = true;

        string sceneToLoad = "GameOverScene";

        if (TransitionManager.Instance != null)
        {
            TransitionManager.Instance.TransitionTo(sceneToLoad);
        }
        else
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    public float CurrentMaxHealth()
    {
        CurrencySourceStatBlock data = Lood.UpgradeManager.Instance.CurrencySources[HealthData.Tag];

        //Apply base frequency.
        float _maxHealth = data.MainStats[MainStat.HealthMaxBase];
        //Apply mult.
        _maxHealth *= data.MainStats[MainStat.HealthMaxMultiplier];

        _maxHealth += _maxHealth * data.MainStats[MainStat.HealthMaxAdditivePercentage] / 100;

        data.MainStats[MainStat.HealthMaxCurrentTotal] = _maxHealth;
        EconomyManager.Instance.MaxHealth = (long)_maxHealth;

        if (slider1.maxValue != _maxHealth)
        {
            if (slider1 != null) { slider1.maxValue = _maxHealth; }
            if (slider2 != null) { slider2.maxValue = _maxHealth; }
        }


        return _maxHealth;
    }

    public float CurrentCooldown()
    {
        CurrencySourceStatBlock data = Lood.UpgradeManager.Instance.CurrencySources[HealthData.Tag];

        //Apply base speed.
        float _cooldown = data.MainStats[MainStat.BaseSpeed];

        //Apply additive percentage (If frequency goes up by 100%: frequency is doubled / pulse timer is halved). This calculation ensures frequency/timer never reaches zero.
        _cooldown *= 1f / (1f + (data.MainStats[MainStat.SpeedAdditivePercentage] / 100));

        //This number should be used as a timer value. It represents "spawns per second".
        return _cooldown;
    }

    public int GetCurrentCollectAmount(Currency currency, string tag)
    {
        //
        CurrencySourceStatBlock data = Lood.UpgradeManager.Instance.CurrencySources[tag];
        //Apply base speed (in ticks per second)
        float finalCollectAmount = data.CollectStats[currency].Stats[CollectStat.BaseCollect];
        //Apply simple add
        finalCollectAmount += data.CollectStats[currency].Stats[CollectStat.CollectAdd];
        //Apply simple multiply
        finalCollectAmount *= data.CollectStats[currency].Stats[CollectStat.CollectMultiplier];
        //Apply special bonus - Per Tentacle Count.
        finalCollectAmount += data.CollectStats[currency].Stats[CollectStat.CollectBonus1] * EconomyManager.Instance.TentacleCount;
        //Apply additive percentage
        finalCollectAmount = finalCollectAmount * (1 + (data.CollectStats[currency].Stats[CollectStat.CollectAdditivePercentage] / 100));

        //Apply special bonus - Per Tentacle Count.
        finalCollectAmount += data.CollectStats[currency].Stats[CollectStat.CollectBonus1] * EconomyManager.Instance.TentacleCount;
        //Apply multiplicative percentage
        finalCollectAmount += finalCollectAmount * data.CollectStats[currency].Stats[CollectStat.CollectMultiplicativePercentage] / 100;

        //Set current total collect amount for this currency.
        data.CollectStats[currency].Stats[CollectStat.CollectTotalCurrent] = finalCollectAmount;
        return (int)finalCollectAmount;

    }
}
