using System.Collections.Generic;
using UnityEngine;

public class TyrantTentacleManager : MonoBehaviour
{
    [Header("Tentacle Management")]
    [SerializeField]
    private List<TentacleControllerIK> tentacles;

    [SerializeField]
    private int activeTentacleCount = 1;
    private int lastUsedTentacleIndex = -1;

    [Header("Targeting")]
    [SerializeField]
    private float grabRadius = 5.0f;

    [SerializeField]
    private VictimSystem victimSystem;

    [SerializeField]
    private ResistanceSystem resistanceSystem;

    [Header("Dependencies")]
    [SerializeField]
    private Transform mouthPoint; // all tentacles need this variable

    [SerializeField]
    private KebabThrust kebabManager;

    public CollectionType collectorData;

    public GameObject BloodSplatter;

    private float _autoClickInterval = 0.05f;
    private float _currentAutoClickTime = 0f;

    void Start() { }

    void Update()
    {
        _currentAutoClickTime += Time.deltaTime;

        if (_currentAutoClickTime >= _autoClickInterval)
        {
            _currentAutoClickTime = 0f;
            if (Input.GetMouseButton(0))
            {
                HandleInteraction(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
        }
    }

    private void HandleInteraction(Vector2 clickPosition)
    {
        IGrabbable closestVictim = FindClosestGrabbable(clickPosition, grabRadius);
        float distToVictim = float.MaxValue;
        float distToFighter = float.MaxValue;

        if (closestVictim != null)
        {
            distToVictim = Vector2.Distance(clickPosition, closestVictim.Position);
        }

        ResistanceData closestFighter = FindClosestFighter(clickPosition, grabRadius);

        if (closestFighter != null)
        {
            distToFighter = Vector2.Distance(clickPosition, closestFighter.Position);
        }

        if (closestVictim == null && closestFighter == null)
        {
            return;
        }

        if (distToFighter < distToVictim)
        {
            if (kebabManager != null)
            {
                kebabManager.AttackAtPosition(closestFighter.position);
            }
        }
        else
        {
            if (!closestVictim.IsGrabbed)
            {
                TentacleControllerIK freeTentacle = FindFreeTentacle();
                if (freeTentacle != null)
                {
                    closestVictim.IsGrabbed = true;
                    freeTentacle.StartGrab(closestVictim, mouthPoint);
                }
            }
        }
    }

    private TentacleControllerIK FindFreeTentacle()
    {
        if (tentacles == null || activeTentacleCount == 0)
            return null;

        List<int> freeIndices = new List<int>();
        for (int i = 0; i < activeTentacleCount; i++)
        {
            if (!tentacles[i].IsBusy())
                freeIndices.Add(i);
        }

        if (freeIndices.Count == 0)
            return null;

        int randomIndex = freeIndices[Random.Range(0, freeIndices.Count)];
        lastUsedTentacleIndex = randomIndex;
        return tentacles[randomIndex];
    }

    private IGrabbable FindClosestGrabbable(Vector2 position, float maxDistance)
    {
        IGrabbable closest = null;
        float closestDistSqr = maxDistance * maxDistance;

        var grabbables = new List<IGrabbable>();

        if (victimSystem != null)
        {
            grabbables.AddRange(victimSystem.victims);
        }

        foreach (var grabbable in grabbables)
        {
            if (grabbable.IsGrabbed)
                continue;

            float distSqr = (grabbable.Position - position).sqrMagnitude;
            if (distSqr < closestDistSqr)
            {
                closestDistSqr = distSqr;
                closest = grabbable;
            }
        }
        return closest;
    }

    private ResistanceData FindClosestFighter(Vector2 position, float maxDistance)
    {
        ResistanceData closest = null;
        float closestDistSqr = maxDistance * maxDistance;

        if (resistanceSystem == null || resistanceSystem.fighters == null)
            return null;

        foreach (var fighter in resistanceSystem.fighters)
        {
            if (fighter == null)
                continue;

            float distSqr = (fighter.position - position).sqrMagnitude;

            if (distSqr < closestDistSqr)
            {
                closestDistSqr = distSqr;
                closest = fighter;
            }
        }
        return closest;
    }

    public void ActivateNextTentacle()
    {
        if (tentacles == null)
            return;

        if (activeTentacleCount < tentacles.Count)
        {
            tentacles[activeTentacleCount].transform.parent.gameObject.SetActive(true);
            activeTentacleCount++;

            // Here we can add visual feedback or stuff!! / Magnus
        }
        else { }
    }

    public void DestroyGrabbable(IGrabbable grabbable)
    {
        if (grabbable is VictimData victim)
        {
            if (victimSystem != null && victimSystem.victims.Contains(victim))
            {
                Instantiate(BloodSplatter, victim.position, Quaternion.identity);
                victimSystem.victims.Remove(victim);
            }
        }
        else if (grabbable is ResistanceData fighter)
        {
            if (resistanceSystem != null && resistanceSystem.fighters.Contains(fighter))
            {
                Instantiate(BloodSplatter, fighter.position, Quaternion.identity);
                resistanceSystem.fighters.Remove(fighter);
            }
        }

        EconomyManager.Instance.IncrementCurrency(Currency.Biomass, ModifiedCollectAmount());
    }

    public void IncrementBaseValue(float amount, HarvesterBaseValue value, Currency currency)
    {
        switch (value)
        {
            case HarvesterBaseValue.CollectAdd:
                collectorData.CollectAdd += (int)amount;
                break;
            case HarvesterBaseValue.CollectMultiplier:
                collectorData.CollectMultiplier += amount;
                break;
            case HarvesterBaseValue.TicksPerSecondMultiplier:
                break;
            case HarvesterBaseValue.CollectAdditivePercentage:
                collectorData.CollectAdditivePercentage += amount;
                break;
            case HarvesterBaseValue.CollectMultiplicativePercentage:
                collectorData.CollectMultiplicativePercentage *= 1f + amount;
                break;
            default:
                break;
        }
    }

    public int ModifiedCollectAmount()
    {
        float finalCurrency = collectorData.BaseCollect;

        finalCurrency += collectorData.CollectAdd;

        finalCurrency *= collectorData.CollectMultiplier;

        finalCurrency += finalCurrency * (collectorData.CollectAdditivePercentage / 100f);

        finalCurrency += finalCurrency * (collectorData.CollectMultiplicativePercentage / 100f);

        return (int)finalCurrency;
    }
}
