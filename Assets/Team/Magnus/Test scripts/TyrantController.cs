using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TyrantController : MonoBehaviour
{
    [Header("Scene references")]
    [SerializeField] private CapsuleCollider2D grabArea;
    [SerializeField] private CircleCollider2D catchTrigger;
    [SerializeField] public GameManager gameManager;
    [SerializeField] public Transform mouthPoint;
    [SerializeField] private VictimSystem victimSystem;

    [Header("Tentacle Settings")]
    [SerializeField] private List <TentacleController> tentacles;

    [Header("Larynx Ability Dependencies")]
    [SerializeField] private Collider2D tyrantBodyCollider;
    [SerializeField] private HumanSpawner humanSpawner;
    [SerializeField] private AudioSource larynxSound;

    [Header("Grab settings")]
    public TMP_Text liftSpeedCountText;
    public TMP_Text liftSpeedPriceText;
    [SerializeField] public float liftSpeed = 6f;
    [SerializeField] public float liftSpeedMultiplier = 1f;
    [SerializeField] public float liftSpeedBase = 6f;
    [SerializeField] public float liftSpeedPercentageIncrement = 0.25f;
    [SerializeField] public int liftSpeedUpgradesOwned = 0;
    [SerializeField] public int liftSpeedUpgradeBasePrice = 10;
    [SerializeField] public int liftSpeedUpgradeNextPrice = 10;
    [SerializeField] public float liftSpeedUpgradePriceGrowthRate = 1.1f;
    [SerializeField] private float grabRadius = 5.0f;
/*     public Animator TentacleAnimator;
 */
    public bool larynxIsActive = false;

    [Header("Biomass modification")]
    public TMP_Text biomassPerHumanCountText;
    public TMP_Text biomassPerHumanPriceText;
    [SerializeField] public float biomassPerHuman = 5;
    [SerializeField] public float biomassPerHumanBase = 5;
    [SerializeField] public float biomassPerHumanIncrement = 1f;
    [SerializeField] public int biomassPerHumanUpgradesOwned = 0;
    [SerializeField] public int biomassPerHumanUpgradeBasePrice = 10;
    [SerializeField] public int biomassPerHumanUpgradeNextPrice = 10;
    [SerializeField] public float biomassPerHumanUpgradePriceGrowthRate = 1.1f;

    public void SaveTyrant()
    {
        PrototypeSaveSystem.PrototypeSavePlayer(this);
        Debug.Log("Saved Tyrant");
    }

    public void LoadTyrant()
    {
        PrototypeSaveData data = PrototypeSaveSystem.PrototypeLoadPlayer();
        if (data != null)
        {
            gameManager.biomass = data.biomass;
            liftSpeed = data.liftSpeed;
            liftSpeedMultiplier = data.liftSpeedMultiplier;
            liftSpeedUpgradesOwned = data.liftSpeedUpgradesOwned;
            liftSpeedUpgradeNextPrice = data.liftSpeedUpgradeNextPrice;

            larynxIsActive = data.larynxIsActive;

            biomassPerHuman = data.biomassPerHuman;
            biomassPerHumanUpgradesOwned = data.biomassPerHumanUpgradesOwned;
            biomassPerHumanUpgradeNextPrice = data.biomassPerHumanUpgradeNextPrice;
        }
    }


    private void Reset()
    {
        grabArea = GetComponentInChildren<CapsuleCollider2D>();
        catchTrigger = GetComponent<CircleCollider2D>();
        if (gameManager == null) gameManager = FindFirstObjectByType<GameManager>();
         if (humanSpawner == null) humanSpawner = FindFirstObjectByType<HumanSpawner>();
        if (victimSystem == null) victimSystem = FindFirstObjectByType<VictimSystem>(); 
        if (mouthPoint == null) mouthPoint = transform;
    }

    private void Awake()
    {
        if (catchTrigger != null) catchTrigger.isTrigger = true;
        if (grabArea != null) grabArea.isTrigger = true;

    }
    void Start()
    {
        LoadTyrant();
/*         TentacleAnimator.SetFloat("SpeedMult", liftSpeedMultiplier);
 */
        liftSpeedCountText.text = liftSpeedUpgradesOwned.ToString("F0");
        liftSpeedPriceText.text = liftSpeedUpgradeNextPrice.ToString("F0");
        biomassPerHumanCountText.text = biomassPerHumanUpgradesOwned.ToString("F0");
        biomassPerHumanPriceText.text = biomassPerHumanUpgradeNextPrice.ToString("F0");


    }

    void OnDestroy()
    {
        SaveTyrant();
    }

    public void UpdateTextFields()
    {
        liftSpeedCountText.text = liftSpeedUpgradesOwned.ToString("F0");
        liftSpeedPriceText.text = liftSpeedUpgradeNextPrice.ToString("F0");
        biomassPerHumanCountText.text = biomassPerHumanUpgradesOwned.ToString("F0");
        biomassPerHumanPriceText.text = biomassPerHumanUpgradeNextPrice.ToString("F0");
    }

    private void Update()
    {


        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (larynxIsActive && tyrantBodyCollider.OverlapPoint(worldPoint))
            {
                Sing();
            }
else
            {
                TryGrabVictimAtClick(worldPoint);
            }
        }

    }

  private void TryGrabVictimAtClick(Vector2 clickPosition)
    {
        VictimData victimToGrab = FindClosestVictim(clickPosition, grabRadius);

        if (victimToGrab != null)
        {
            if (!victimToGrab.isMarkedForDeath)
            {
                TentacleController freeTentacle = FindFreeTentacle();
                if (freeTentacle != null)
                {
                    freeTentacle.StartGrab(victimToGrab, victimSystem);
                    victimToGrab.isMarkedForDeath = true; 
                }
                else
                {
                    Debug.Log("Alla tentakler är upptagna");
                }
            }
        }
    }

    private VictimData FindClosestVictim(Vector2 position, float maxDistance)
    {
        VictimData closest = null;
        float closestDistSqr = maxDistance * maxDistance;

        foreach (var victim in victimSystem.victims)
        {
            if (victim.isMarkedForDeath) continue; 

            float distSqr = (victim.position - position).sqrMagnitude;
            if (distSqr < closestDistSqr)
            {
                closestDistSqr = distSqr;
                closest = victim;
            }
        }
        return closest;
    }


    private TentacleController FindFreeTentacle()
    {
        foreach (var tentacle in tentacles)
        {
            if (!tentacle.IsBusy())
            {return tentacle;}
        }
        return null;
    }

    public void IncrementLiftSpeed()
    {
        if (gameManager.biomass >= liftSpeedUpgradeNextPrice)
        {
            gameManager.biomass -= liftSpeedUpgradeNextPrice;
            liftSpeed += liftSpeedBase * liftSpeedPercentageIncrement;
            liftSpeedMultiplier += liftSpeedPercentageIncrement;

/*             TentacleAnimator.SetFloat("SpeedMult", liftSpeedMultiplier);
 */
            liftSpeedUpgradesOwned++;
            liftSpeedUpgradeNextPrice = (int)(liftSpeedUpgradeBasePrice * Mathf.Pow(liftSpeedUpgradePriceGrowthRate, liftSpeedUpgradesOwned));
            UpdateTextFields();
        }

    }

    public void IncrementEfficiency()
    {
        if (gameManager.biomass >= biomassPerHumanUpgradeNextPrice)
        {
            gameManager.biomass -= biomassPerHumanUpgradeNextPrice;
            biomassPerHuman += biomassPerHumanIncrement;
            biomassPerHumanUpgradesOwned++;
            biomassPerHumanUpgradeNextPrice = (int)(biomassPerHumanUpgradeBasePrice * Mathf.Pow(biomassPerHumanUpgradePriceGrowthRate, biomassPerHumanUpgradesOwned));
            UpdateTextFields();
        }

    }

    public void ActivateLarynx()
    {
        larynxIsActive = true;
    }

    private void Sing()
    {
        if (larynxSound != null)
        {
            larynxSound.Play();
        }

        if (humanSpawner != null && humanSpawner.victimCount < humanSpawner.maxCount)
        {
            humanSpawner.SpawnHuman();
            humanSpawner.SpawnHuman();
            humanSpawner.SpawnHuman();
            humanSpawner.SpawnHuman();
            humanSpawner.victimCount += 4;
        }
        else
        {
            return;
        }
    }



}