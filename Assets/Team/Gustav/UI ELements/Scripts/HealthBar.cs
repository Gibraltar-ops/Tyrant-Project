using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private VictimSystem victimSystem;

    [Header("References")]
    public Slider slider1;
    public Slider slider2;

    [Header("Variables & Settings")]
    public float maxHealth = 10000;         // The maximum health value
    public float minHealth = 0;             // The minimum health value
    public float regenValue = 10;           // The amount restored per regen cooldown
    public float regenMultiplier = 1;       // The multiplier for health regeneration
    public float regenCooldown = 1;         // The set cooldown for health regeneration
    public float eatValue = 10;             // The amount restored when eating a victim (Combine with a method)


    private float currentHealth;
    private float regenTimer;



    public bool isPressed = false;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    
}
