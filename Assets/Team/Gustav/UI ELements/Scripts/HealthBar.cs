using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    [Header("References")]
    public Slider slider1;
    public Slider slider2;

    public TextMeshProUGUI percentageText;              // Shows 0–100% health
    public TextMeshProUGUI currentHealthText;           // Shows raw current health value
    public Image hoverHideImage;                        // Image to hide when mouse hovers

    [Header("Variables & Settings")]
    public float maxHealth = 10000;                     // The maximum health value
    public float minHealth = 0;                         // The minimum health value
    public float regenValue = 10;                       // The amount restored per regen cooldown
    public float regenMultiplier = 1;                   // The multiplier for health regeneration
    public float regenCooldown = 1;                     // The set cooldown for health regeneration
    public float eatValue = 10;                         // The amount restored when eating a victim (Combine with a method)

    private float currentHealth;
    private float regenTimer;

    void Start()
    {
        // Initialize current health
        currentHealth = maxHealth;

        // Initialize sliders
        if (slider1 != null) slider1.maxValue = maxHealth;
        if (slider2 != null) slider2.maxValue = maxHealth;

        UpdateUI();
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

        // Update the sliders and text each frame
        UpdateUI();
    }

    private void Regenerate()
    {
        currentHealth += regenValue * regenMultiplier;
        currentHealth = Mathf.Clamp(currentHealth, minHealth, maxHealth);
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
        if (isHealing)
            currentHealth += amount;
        else
            currentHealth -= amount;

        currentHealth = Mathf.Clamp(currentHealth, minHealth, maxHealth);
    }
    
    // Mouse hover logic
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverHideImage != null)
            hoverHideImage.enabled = false;

        if (currentHealthText != null)
            currentHealthText.enabled = true;
        Debug.Log("ENTER");
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (hoverHideImage != null)
            hoverHideImage.enabled = true;

        if (currentHealthText != null)
            currentHealthText.enabled = false;
        Debug.Log("EXIT");
    }
}
