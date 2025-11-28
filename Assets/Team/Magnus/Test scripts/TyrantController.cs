using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TyrantController : MonoBehaviour
{
    [Header("Scene references")]
    [SerializeField]
    private CapsuleCollider2D grabArea;

    [SerializeField]
    private CircleCollider2D catchTrigger;

    [SerializeField]
    public GameManager gameManager;

    [SerializeField]
    public Transform mouthPoint;

    [SerializeField]
    private VictimSystem victimSystem;

    [SerializeField]
    private ResistanceSystem resistanceSystem;

    [Header("Tentacle Settings")]
    [SerializeField]
    private List<TentacleController> tentacles;

    [SerializeField]
    private int activeTentacleCount = 1;

    [Header("Larynx Ability Dependencies")]
    [SerializeField]
    private Collider2D tyrantBodyCollider;

    [SerializeField]
    private HumanSpawner humanSpawner;

    [SerializeField]
    private AudioSource larynxSound;

    [Header("Grab settings")]
    public TMP_Text liftSpeedCountText;
    public TMP_Text liftSpeedPriceText;

    [SerializeField]
    public float liftSpeed = 6f;

    [SerializeField]
    public float liftSpeedMultiplier = 1f;

    [SerializeField]
    public float liftSpeedBase = 6f;

    [SerializeField]
    public float liftSpeedPercentageIncrement = 0.25f;

    [SerializeField]
    public int liftSpeedUpgradesOwned = 0;

    [SerializeField]
    public int liftSpeedUpgradeBasePrice = 10;

    [SerializeField]
    public int liftSpeedUpgradeNextPrice = 10;

    [SerializeField]
    public float liftSpeedUpgradePriceGrowthRate = 1.1f;

    [SerializeField]
    private float grabRadius = 5.0f;
    private int lastUsedTentacleIndex = -1;

    /*     public Animator TentacleAnimator;
     */
    public bool larynxIsActive = false;

    [Header("Biomass modification")]
    public TMP_Text biomassPerHumanCountText;
    public TMP_Text biomassPerHumanPriceText;

    [SerializeField]
    public float biomassPerHuman = 5;

    [SerializeField]
    public float biomassPerHumanBase = 5;

    [SerializeField]
    public float biomassPerHumanIncrement = 1f;

    [SerializeField]
    public int biomassPerHumanUpgradesOwned = 0;

    [SerializeField]
    public int biomassPerHumanUpgradeBasePrice = 10;

    [SerializeField]
    public int biomassPerHumanUpgradeNextPrice = 10;

    [SerializeField]
    public float biomassPerHumanUpgradePriceGrowthRate = 1.1f;

    public void SaveTyrant()
    {
        // PrototypeSaveSystem.PrototypeSavePlayer(this);
        // Debug.Log("Saved Tyrant");
    }

    public void LoadTyrant()
    {
        // PrototypeSaveData data = PrototypeSaveSystem.PrototypeLoadPlayer();
        // if (data != null)
        // {
        //     gameManager.biomass = data.biomass;
        //     liftSpeed = data.liftSpeed;
        //     liftSpeedMultiplier = data.liftSpeedMultiplier;
        //     liftSpeedUpgradesOwned = data.liftSpeedUpgradesOwned;
        //     liftSpeedUpgradeNextPrice = data.liftSpeedUpgradeNextPrice;

        //     larynxIsActive = data.larynxIsActive;

        //     biomassPerHuman = data.biomassPerHuman;
        //     biomassPerHumanUpgradesOwned = data.biomassPerHumanUpgradesOwned;
        //     biomassPerHumanUpgradeNextPrice = data.biomassPerHumanUpgradeNextPrice;
        // }
    }

    private void Reset()
    {
        grabArea = GetComponentInChildren<CapsuleCollider2D>();
        catchTrigger = GetComponent<CircleCollider2D>();
        if (gameManager == null)
            gameManager = FindFirstObjectByType<GameManager>();

        if (victimSystem == null)
            victimSystem = FindFirstObjectByType<VictimSystem>();

        if (resistanceSystem == null)
            resistanceSystem = FindFirstObjectByType<ResistanceSystem>();
        if (humanSpawner == null)
            humanSpawner = FindFirstObjectByType<HumanSpawner>();

        if (mouthPoint == null)
            mouthPoint = transform;
    }

    private void Awake()
    {
        if (catchTrigger != null)
            catchTrigger.isTrigger = true;
        if (grabArea != null)
            grabArea.isTrigger = true;

        if (gameManager == null)
            gameManager = FindFirstObjectByType<GameManager>();
        if (victimSystem == null)
            victimSystem = FindFirstObjectByType<VictimSystem>();
        if (resistanceSystem == null)
            resistanceSystem = FindFirstObjectByType<ResistanceSystem>();
        if (humanSpawner == null)
            humanSpawner = FindFirstObjectByType<HumanSpawner>();
    }

    void Start()
    {
        LoadTyrant();
        UpdateTentacleVisuals();
        /*         TentacleAnimator.SetFloat("SpeedMult", liftSpeedMultiplier);
         */
        // liftSpeedCountText.text = liftSpeedUpgradesOwned.ToString("F0");
        // liftSpeedPriceText.text = liftSpeedUpgradeNextPrice.ToString("F0");
        // biomassPerHumanCountText.text = biomassPerHumanUpgradesOwned.ToString("F0");
        // biomassPerHumanPriceText.text = biomassPerHumanUpgradeNextPrice.ToString("F0");
    }

    void OnDestroy()
    {
        SaveTyrant();
    }

    public void UpdateTextFields()
    {
        // liftSpeedCountText.text = liftSpeedUpgradesOwned.ToString("F0");
        // liftSpeedPriceText.text = liftSpeedUpgradeNextPrice.ToString("F0");
        // biomassPerHumanCountText.text = biomassPerHumanUpgradesOwned.ToString("F0");
        // biomassPerHumanPriceText.text = biomassPerHumanUpgradeNextPrice.ToString("F0");
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
                TryGrabTargetAtClick(worldPoint);
            }
        }
    }

    public void ActivateNewTentacle()
    {
        if (activeTentacleCount < tentacles.Count)
        {
            activeTentacleCount++;
            UpdateTentacleVisuals();
        }
    }

    private void UpdateTentacleVisuals()
    {
        for (int i = 0; i < tentacles.Count; i++)
        {
            if (tentacles[i] != null)
            {
                bool shouldBeActive = i < activeTentacleCount;
                tentacles[i].gameObject.SetActive(shouldBeActive);
            }
        }
    }

    private void TryGrabTargetAtClick(Vector2 clickPosition)
    {
        TargetData targetToGrab = FindClosestTarget(clickPosition, grabRadius);

        if (targetToGrab != null)
        {
            TentacleController freeTentacle = FindFreeTentacle();
            if (freeTentacle != null)
            {
                if (targetToGrab.isResistance)
                {
                    Debug.Log("Caught resistance");
                    ResistanceData resistanceData = targetToGrab.resistance;
                    if (!resistanceData.isMarkedForDeath)
                    {
                        freeTentacle.StartGrab(resistanceData, resistanceSystem);
                        resistanceData.isMarkedForDeath = true;
                    }
                }
                else
                {
                    VictimData victimToGrab = targetToGrab.victim;

                    if (!victimToGrab.isMarkedForDeath)
                    {
                        freeTentacle.StartGrab(victimToGrab, victimSystem);
                        victimToGrab.isMarkedForDeath = true;
                    }
                }
            }
            else
            {
                Debug.Log("All tentacles busy");
            }
        }
    }

    public class TargetData
    {
        public bool isResistance;
        public VictimData victim;
        public ResistanceData resistance;
        public Vector2 position;
    }

    private TargetData FindClosestTarget(Vector2 position, float maxDistance)
    {
        VictimData closestVictim = FindClosestVictim(position, maxDistance);

        ResistanceData closestResistance = FindClosestResistance(position, maxDistance);

        float distToVictimSqr =
            (closestVictim != null)
                ? (closestVictim.position - position).sqrMagnitude
                : float.MaxValue;

        float distToResistanceSqr =
            (closestResistance != null)
                ? (closestResistance.position - position).sqrMagnitude
                : float.MaxValue;

        if (distToVictimSqr < distToResistanceSqr)
        {
            if (closestVictim != null)
            {
                return new TargetData
                {
                    isResistance = false,
                    victim = closestVictim,
                    position = closestVictim.position,
                };
            }
        }
        else if (distToResistanceSqr < distToVictimSqr)
        {
            if (closestResistance != null)
            {
                return new TargetData
                {
                    isResistance = true,
                    resistance = closestResistance,
                    position = closestResistance.position,
                };
            }
        }

        return null;
    }

    private VictimData FindClosestVictim(Vector2 position, float maxDistance)
    {
        VictimData closest = null;
        float closestDistSqr = maxDistance * maxDistance;

        foreach (var victim in victimSystem.victims)
        {
            if (victim.isMarkedForDeath)
                continue;

            float distSqr = (victim.position - position).sqrMagnitude;
            if (distSqr < closestDistSqr)
            {
                closestDistSqr = distSqr;
                closest = victim;
            }
        }
        return closest;
    }

    private ResistanceData FindClosestResistance(Vector2 position, float maxDistance)
    {
        ResistanceData closest = null;
        float closestDistSqr = maxDistance * maxDistance;

        if (resistanceSystem != null && resistanceSystem.fighters != null)
        {
            foreach (var resistance in resistanceSystem.fighters)
            {
                if (resistance.isMarkedForDeath)
                    continue;

                float distSqr = (resistance.position - position).sqrMagnitude;
                if (distSqr < closestDistSqr)
                {
                    closestDistSqr = distSqr;
                    closest = resistance;
                }
            }
        }
        return closest;
    }

    private TentacleController FindFreeTentacle()
    {
        if (tentacles == null || activeTentacleCount == 0)
        {
            return null;
        }

        for (int i = 0; i < activeTentacleCount; i++)
        {
            int indexToTry = (lastUsedTentacleIndex + 1 + i) % activeTentacleCount;

            if (!tentacles[indexToTry].IsBusy())
            {
                lastUsedTentacleIndex = indexToTry;
                return tentacles[indexToTry];
            }
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
            liftSpeedUpgradeNextPrice = (int)(
                liftSpeedUpgradeBasePrice
                * Mathf.Pow(liftSpeedUpgradePriceGrowthRate, liftSpeedUpgradesOwned)
            );
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
            biomassPerHumanUpgradeNextPrice = (int)(
                biomassPerHumanUpgradeBasePrice
                * Mathf.Pow(biomassPerHumanUpgradePriceGrowthRate, biomassPerHumanUpgradesOwned)
            );
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
