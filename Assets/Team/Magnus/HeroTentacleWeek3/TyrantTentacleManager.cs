using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

public class TyrantTentacleManager : MonoBehaviour
{
    [Header("Tentacle Management")]
    private List<TentacleControllerIK> tentacles;

    [Tooltip("CurrencySourceSO that contains upgradeable parameters for player tentacles")]
    public CurrencySourceSO PlayerTentacleData;

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

    [Header("VFX")]
    public GameObject BloodSplatter;

    public GameObject Explosion;

    [SerializeField] private ParticleSystem eatingParticleSystem;


    [SerializeField] private float eatingEffectDelay = 0.2f;

    [SerializeField]
    private int explosionCount = 5;

    [SerializeField]
    private float explosionSpread = 1f;

    [SerializeField]
    private ShakeRequestManagerCinemachine shakeManager;

    [SerializeField]
    private float explosionShakeAmplitude = 1.0f;

    [SerializeField]
    private float explosionShakeDuration = 0.5f;

    private float _autoClickInterval = 0.2f;
    private float _currentAutoClickTime = 0f;

    ///Eat counter
    public static System.Action VictimEaten;

    void Awake()
    {
        //Lood.UpgradeManager.Instance.AddUpgradeableObject(PlayerTentacleData);
        //Gather all tentacles
        TentacleControllerIK[] tentacleArray = GetComponentsInChildren<TentacleControllerIK>(true);
        tentacles = tentacleArray.ToList();
    }
    void Start() { }

    void OnEnable()
    {
        EconomyManager.onCurrencyChanged += UpdateValues;
    }
    void OnDisable()
    {
        EconomyManager.onCurrencyChanged -= UpdateValues;
    }
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
                // how many strikes check
                int strikeCount = kebabManager.CurrentStrikeCount;


                List<ResistanceData> targets = FindClosestFighters(clickPosition, grabRadius, strikeCount);


                List<Vector3> targetPositions = new List<Vector3>();
                foreach (var t in targets) targetPositions.Add(t.position);

                kebabManager.AttackMultipleTargets(
                    targetPositions,
                    (index) =>
                    {
                        if (index >= 0 && index < targets.Count)
                        {
                            var fighter = targets[index];
                            if (fighter != null)
                            {
                                AudioManager.Instance.Play("TyrantStrike");
                                AudioManager.Instance.Play("ProjectileExplosion");
                                DestroyFighter(fighter);
                            }
                        }
                    }
                );
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


    private List<ResistanceData> FindClosestFighters(Vector2 position, float maxDistance, int count)
    {
        if (resistanceSystem == null || resistanceSystem.fighters == null)
            return new List<ResistanceData>();
        return resistanceSystem.fighters
            .Where(f => f != null)
            .Select(f => new { Fighter = f, DistSq = (f.position - position).sqrMagnitude })
            .Where(x => x.DistSq <= maxDistance * maxDistance)
            .OrderBy(x => x.DistSq)
            .Take(count)
            .Select(x => x.Fighter)
            .ToList();
    }

    public void DestroyFighter(ResistanceData fighter)
    {
        if (resistanceSystem != null && resistanceSystem.fighters.Contains(fighter))
        {
            SpawnExplosion(fighter.position);
            resistanceSystem.fighters.Remove(fighter);
        }
    }

    private void SpawnExplosion(Vector2 centerPos)
    {
        if (shakeManager != null)
        {
            shakeManager.Request(explosionShakeAmplitude, explosionShakeDuration, centerPos);
        }
        for (int i = 0; i < explosionCount; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * explosionSpread;
            Vector3 spawnPos = centerPos + randomOffset;

            GameObject vfx = Instantiate(Explosion, spawnPos, Quaternion.identity);

            float randomScale = Random.Range(0.8f, 1.5f);
            vfx.transform.localScale *= randomScale;

            vfx.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
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

            // Here we can add visual feedback (on spawn) or stuff!! / Magnus
        }
        else { }
    }

    public void DestroyGrabbable(IGrabbable grabbable)
    {
        Vector2 grabbedPosition = Vector2.zero;
        if (grabbable is VictimData victim)
        {
            if (victimSystem != null && victimSystem.victims.Contains(victim))
            {
                Instantiate(BloodSplatter, victim.position, Quaternion.identity);
                grabbedPosition = victim.position;
                victimSystem.victims.Remove(victim);

                VictimEaten?.Invoke();
            }
        }

        if (eatingParticleSystem != null)
        {
            StartCoroutine(PlayEatingParticles());
        }

        AudioManager.Instance.Play("TyrantEating");
        AwardCurrency(grabbedPosition);
    }

    private IEnumerator PlayEatingParticles()
    {
        yield return new WaitForSeconds(eatingEffectDelay);
        if (eatingParticleSystem != null)
        {
            eatingParticleSystem.Play();
        }
    }



    //Currency Gathering
    private void AwardCurrency(Vector2 collectPosition)
    {
        var collectStats = Lood.UpgradeManager.Instance.CurrencySources[PlayerTentacleData.Tag].CollectStats;

        //The currencies collected are held in a dictionary "CollectStats". 
        //Key: Currency, Value: CollectTypeStatBlock (Holds stuff like BaseCollect and multipliers.)
        foreach (KeyValuePair<Currency, CollectTypeStatBlock> item in collectStats)
        {
            int collectAmount = GetCurrentCollectAmount(item.Key, PlayerTentacleData.Tag);
            if (collectAmount != 0)
            {
                CollectCurrency(item.Key, collectAmount);
                DoPopup(item.Key, collectAmount, collectPosition);
            }
        }
    }

    private void CollectCurrency(Currency collectedCurrency, int amount)
    {
        EconomyManager.Instance.IncrementCurrency(collectedCurrency, amount);
    }

    private void DoPopup(Currency collectedCurrency, int amount, Vector2 position)
    {
        Vector2 offsetPosition = position + Random.insideUnitCircle + new Vector2(0f, 4f); ;
        PopupTextData data = new PopupTextData(offsetPosition, FormatNumbers.FormatValueString(amount, ValueFormatType.IntegerAdditive), Color.white, collectedCurrency);
        StartCoroutine(DelayedPopup(data));

    }
    private IEnumerator DelayedPopup(PopupTextData data)
    {
        float delay = Random.Range(0f, 0.5f);
        yield return new WaitForSeconds(delay);
        EconomyManager.Instance.PopUp(data);
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
        //Apply additive percentage
        finalCollectAmount *= 1 + (data.CollectStats[currency].Stats[CollectStat.CollectAdditivePercentage] / 100);
        //Apply multiplicative percentage
        finalCollectAmount += finalCollectAmount * data.CollectStats[currency].Stats[CollectStat.CollectMultiplicativePercentage] / 100;

        //Apply special bonus - Collect per tentacle owned.
        finalCollectAmount += data.CollectStats[currency].Stats[CollectStat.CollectBonus1] * EconomyManager.Instance.TentacleCount;


        //Set current total collect amount for this currency.
        data.CollectStats[currency].Stats[CollectStat.CollectTotalCurrent] = finalCollectAmount;
        return (int)finalCollectAmount;

    }

    //This is only for updating UI values for final collect amounts.
    public void UpdateValues(Currency currency)
    {
        var collectStats = Lood.UpgradeManager.Instance.CurrencySources[PlayerTentacleData.Tag].CollectStats;
        foreach (KeyValuePair<Currency, CollectTypeStatBlock> item in collectStats)
        {
            GetCurrentCollectAmount(item.Key, PlayerTentacleData.Tag);
        }
    }

}
