using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TyrantController : MonoBehaviour
{
    [Header("Scene references")]
    [SerializeField] private CapsuleCollider2D absorbArea;
    [SerializeField] private CircleCollider2D catchTrigger;
    [SerializeField] private GameManager gameManager;
    [SerializeField] public Transform mouthPoint;

    [Header("Absorption settings")]

    public TMP_Text liftSpeedCountText;
    public TMP_Text liftSpeedPriceText;
    [SerializeField] public float liftSpeed = 6f;
    [SerializeField] private float liftSpeedMultiplier = 1f;
    [SerializeField] private float liftSpeedBase = 6f;
    [SerializeField] private float liftSpeedPercentageIncrement = 0.25f;
    [SerializeField] private int liftSpeedUpgradesOwned = 0;
    [SerializeField] private int liftSpeedUpgradeBasePrice = 10;
    [SerializeField] private int liftSpeedUpgradeNextPrice = 10;
    [SerializeField] private float liftSpeedUpgradePriceGrowthRate = 1.1f;
    public Animator TentacleAnimator;


    public TMP_Text biomassPerHumanCountText;
    public TMP_Text biomassPerHumanPriceText;
    [SerializeField] public float biomassPerHuman = 5;
    [SerializeField] private float biomassPerHumanBase = 5;
    [SerializeField] private float biomassPerHumanIncrement = 1f;
    [SerializeField] private int biomassPerHumanUpgradesOwned = 0;
    [SerializeField] private int biomassPerHumanUpgradeBasePrice = 10;
    [SerializeField] private int biomassPerHumanUpgradeNextPrice = 10;
    [SerializeField] private float biomassPerHumanUpgradePriceGrowthRate = 1.1f;


    private void Reset()
    {
        absorbArea = GetComponentInChildren<CapsuleCollider2D>();
        catchTrigger = GetComponent<CircleCollider2D>();
        if (gameManager == null) gameManager = FindFirstObjectByType<GameManager>();
        if (mouthPoint == null) mouthPoint = transform;
    }

    private void Awake()
    {
        if (catchTrigger != null) catchTrigger.isTrigger = true;
        if (absorbArea != null) absorbArea.isTrigger = true;

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

            if (absorbArea != null && absorbArea.OverlapPoint(worldPoint))
            {
                TryStartAbsorbingOneHuman();
            }
        }
    }

    private void TryStartAbsorbingOneHuman()
    {
        var results = new Collider2D[32];
        var filter = ContactFilter2D.noFilter;
        int count = Physics2D.OverlapCollider(absorbArea, filter, results);

        HumanMovement chosen = null;
        float bestDist = float.PositiveInfinity;

        for (int i = 0; i < count; i++)
        {
            var col = results[i];
            if (col == null) continue;
            if (!col.CompareTag("Human")) continue;
            if (!col.TryGetComponent(out HumanMovement hm)) continue;
            if (hm.IsBeingAbsorbed) continue;

            float d = ((Vector2)(col.transform.position - transform.position)).sqrMagnitude;
            if (d < bestDist)
            {
                bestDist = d;
                chosen = hm;
            }
        }

        if (chosen)
        {
            Debug.Log(biomassPerHuman);
            chosen.StartAbsorption(mouthPoint, liftSpeed, biomassPerHuman);
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



}