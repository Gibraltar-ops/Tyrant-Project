using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;

public class HealthBar : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
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

    [Header("Variables & Settings")]
    public float maxHealth = 10000;                     // The maximum health value
    public float minHealth = 0;                         // The minimum health value
    public float regenValue = 10;                       // The amount restored per regen cooldown
    public float regenMultiplier = 1;                   // The multiplier for health regeneration
    public float regenCooldown = 1;                     // The set cooldown for health regeneration
    public float eatValue = 10;                         // The amount restored when eating a victim (Combine with a method)

    private float currentHealth;
    private float regenTimer;
    private Color originalColor1;
    private Color originalColor2;
    private Coroutine activeHealthBarCoroutine;

    void Start()
    {
        // Initialize current health
        currentHealth = maxHealth;

        // Initialize sliders
        if (slider1 != null) { slider1.maxValue = maxHealth; slider1.value = currentHealth; }
        if (slider2 != null) { slider2.maxValue = maxHealth; slider2.value = currentHealth; }

        if (fillImage1 != null) originalColor1 = fillImage1.color;
        if (fillImage2 != null) originalColor2 = fillImage2.color;

        UpdateText();
    }

    void Update()
    {
        // Handle regeneration every cooldown
        regenTimer += Time.deltaTime;
        if (regenTimer >= regenCooldown)
        {
            regenTimer = 0f;
            Regenerate();
        }
    }

    private void Regenerate()
    {
        if (currentHealth >= maxHealth) return;

        currentHealth += regenValue * regenMultiplier;
        currentHealth = Mathf.Clamp(currentHealth, minHealth, maxHealth);

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (slider1 != null)
            slider1.value = currentHealth;

        if (slider2 != null)
            slider2.value = currentHealth;

        if (percentageText != null)
            percentageText.text = Mathf.RoundToInt((currentHealth / maxHealth) * 100f) + "%";

        if (currentHealthText != null)
            currentHealthText.text = Mathf.RoundToInt(currentHealth).ToString();
    }

    /// <summary>
    /// Takes damage when isHealing = false.
    /// Heals when isHealing = true.
    /// </summary>
    public void ModifyHealth(float amount, bool isHealing)
    {
        float previousHealth = currentHealth;

        if (isHealing)
            currentHealth += amount;
        else
            currentHealth -= amount;

        currentHealth = Mathf.Clamp(currentHealth, minHealth, maxHealth);

        UpdateText();
        UpdateHealthBarVisuals(previousHealth);

        if (!isHealing)
        {
            StopCoroutine("FlashDamageEffect");
            StartCoroutine(FlashDamageEffect());
        }
    }

    private void UpdateText()
    {
        if (percentageText != null)
            percentageText.text = Mathf.RoundToInt((currentHealth / maxHealth) * 100f) + "%";

        if (currentHealthText != null)
            currentHealthText.text = Mathf.RoundToInt(currentHealth).ToString();
    }

    private void UpdateHealthBarVisuals(float oldHealth)
    {
        if (activeHealthBarCoroutine != null)
            StopCoroutine(activeHealthBarCoroutine);

        activeHealthBarCoroutine = StartCoroutine(AnimateHealthBar());
    }

    private IEnumerator AnimateHealthBar()
    {
        float currentSliderValue = (slider1 != null) ? slider1.value : currentHealth;

        while (Mathf.Abs(currentSliderValue - currentHealth) > 1f)
        {
            currentSliderValue = Mathf.Lerp(
                currentSliderValue,
             currentHealth,
              Time.deltaTime *
              smoothness
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

        yield return new WaitForSeconds(flashDuration);

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
}
