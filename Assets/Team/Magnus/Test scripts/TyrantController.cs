using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TyrantController : MonoBehaviour
{
    [Header("Scene references")]
    [SerializeField] private CapsuleCollider2D grabArea;
    [SerializeField] private CircleCollider2D catchTrigger;
    [SerializeField] private GameManager gameManager;
    [SerializeField] public Transform mouthPoint;

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
    public Animator TentacleAnimator;

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


    private void Reset()
    {
        grabArea = GetComponentInChildren<CapsuleCollider2D>();
        catchTrigger = GetComponent<CircleCollider2D>();
        if (gameManager == null) gameManager = FindFirstObjectByType<GameManager>();
        if (mouthPoint == null) mouthPoint = transform;
    }

    private void Awake()
    {
        if (catchTrigger != null) catchTrigger.isTrigger = true;
        if (grabArea != null) grabArea.isTrigger = true;

    }
    void Start()
    {
        liftSpeedCountText.text = liftSpeedUpgradesOwned.ToString("F0");
        liftSpeedPriceText.text = liftSpeedUpgradeNextPrice.ToString("F0");
        biomassPerHumanCountText.text = biomassPerHumanUpgradesOwned.ToString("F0");
        biomassPerHumanPriceText.text = biomassPerHumanUpgradeNextPrice.ToString("F0");


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

            else if (grabArea != null && grabArea.OverlapPoint(worldPoint))
            {
                TryStartGrabbingOneHuman();
            }
        }

    }

    private void TryStartGrabbingOneHuman()
    {
        var results = new Collider2D[32];
        var filter = ContactFilter2D.noFilter;
        int count = Physics2D.OverlapCollider(grabArea, filter, results);

        bool canStartGrab = false;

        for (int i = 0; i < count; i++)
        {
            if (results[i].TryGetComponent<HumanMovement>(out var human))
            {
                if (!human.IsBeingGrabbed)
                {
                    canStartGrab = true;
                    break;
                }
            }
        }

        if (canStartGrab)
        {
            Debug.Log(biomassPerHuman);

            TentacleAnimator.SetTrigger("StartGrab");
        }
    }

    public void IncrementLiftSpeed()
    {
        if (gameManager.biomass >= liftSpeedUpgradeNextPrice)
        {
            gameManager.biomass -= liftSpeedUpgradeNextPrice;
            liftSpeed += liftSpeedBase * liftSpeedPercentageIncrement;
            liftSpeedMultiplier += liftSpeedPercentageIncrement;

            TentacleAnimator.SetFloat("SpeedMult", liftSpeedMultiplier);

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
        if (humanSpawner != null)
        {
            humanSpawner.SpawnHuman();
            humanSpawner.SpawnHuman();
            humanSpawner.SpawnHuman();
            humanSpawner.SpawnHuman();
        }
    }



}