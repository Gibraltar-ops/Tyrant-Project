
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lood
{
    public class Harvester : MonoBehaviour
    {
        public bool SpawnObject;
        public bool IsActive = true;

        private CurrencySourceTag _tag;
        private CurrencySourceSO currencySourceData;

        public Vector2 PopUpNumberOffset;

        private CollectorObjectSpawner _objectSpawner;
        private List<Collector> _collectors;

        public delegate void OnCollectionPopup(PopupTextData data);
        public static event OnCollectionPopup onCollectionPopup;

        void Start()
        {
            _objectSpawner = GetComponent<CollectorObjectSpawner>();
            _collectors = new List<Collector>();
            currencySourceData = GetComponent<CurrencySourceShop>().CurrencySourceData;

        }

        public void SpawnCollector()
        {
            Collector newCollector = new Collector(0f);
            _collectors.Add(newCollector);

            if (SpawnObject)
            {
                _objectSpawner.SpawnObject(newCollector);
            }

        }

        public void UpdateValues()
        {
            var collectStats = UpgradeManager.Instance.CurrencySources[currencySourceData.Tag].CollectStats;
            foreach (KeyValuePair<Currency, CollectTypeStatBlock> item in collectStats)
            {
                GetCurrentCollectAmount(item.Key, currencySourceData.Tag);
            }
        }

        public float GetCurrentTicksPerSecond()
        {
            CurrencySourceStatBlock data = UpgradeManager.Instance.CurrencySources[currencySourceData.Tag];

            //Apply base speed (in ticks per second)
            float _ticksPerSecondCurrent = data.MainStats[MainStat.BaseSpeed];
            //Apply simple add
            _ticksPerSecondCurrent += data.MainStats[MainStat.SpeedAdd];
            //Apply simple multiply
            _ticksPerSecondCurrent *= data.MainStats[MainStat.SpeedMultiply];
            //Apply additive percentage
            _ticksPerSecondCurrent = _ticksPerSecondCurrent * (1 + (data.MainStats[MainStat.SpeedAdditivePercentage] / 100));
            //Apply multiplicative percentage
            _ticksPerSecondCurrent += _ticksPerSecondCurrent * data.MainStats[MainStat.SpeedMultiplicativePercentage] / 100;

            return _ticksPerSecondCurrent;
        }

        public int GetCurrentCollectAmount(Currency currency, string tag)
        {
            //
            CurrencySourceStatBlock data = UpgradeManager.Instance.CurrencySources[tag];
            //Apply base speed (in ticks per second)
            float finalCollectAmount = data.CollectStats[currency].Stats[CollectStat.BaseCollect];
            //Apply simple add
            finalCollectAmount += data.CollectStats[currency].Stats[CollectStat.CollectAdd];
            //Apply simple multiply
            finalCollectAmount *= data.CollectStats[currency].Stats[CollectStat.CollectMultiplier];
            //Apply additive percentage
            finalCollectAmount = finalCollectAmount * (1 + (data.CollectStats[currency].Stats[CollectStat.CollectAdditivePercentage] / 100));
            //Apply multiplicative percentage
            finalCollectAmount += finalCollectAmount * data.CollectStats[currency].Stats[CollectStat.CollectMultiplicativePercentage] / 100;

            //Apply special bonus - Per Tentacle Count.
            finalCollectAmount += data.CollectStats[currency].Stats[CollectStat.CollectBonus1] * EconomyManager.Instance.TentacleCount;
            //Apply special bonus - Per Hunter Tentacle Count.
            finalCollectAmount += data.CollectStats[currency].Stats[CollectStat.CollectBonus2] * EconomyManager.Instance.HunterTentacleCount;

            //Set current total collect amount for this currency.
            data.CollectStats[currency].Stats[CollectStat.CollectTotalCurrent] = finalCollectAmount;
            return (int)finalCollectAmount;

        }

        private void CollectCurrency(Currency collectedCurrency, int amount)
        {
            EconomyManager.Instance.IncrementCurrency(collectedCurrency, amount);
        }

        private void DoPopup(Currency collectedCurrency, int amount, Vector2 position)
        {
            Vector2 offsetPosition = position + Random.insideUnitCircle + PopUpNumberOffset;
            PopupTextData data = new PopupTextData(offsetPosition, FormatNumbers.FormatValueString(amount, ValueFormatType.IntegerAdditive), Color.white, collectedCurrency);
            StartCoroutine(DelayedPopup(data));

        }
        private IEnumerator DelayedPopup(PopupTextData data)
        {
            float delay = Random.Range(0f, 0.5f);
            yield return new WaitForSeconds(delay);
            EconomyManager.Instance.PopUp(data);
        }

        // Update is called once per frame
        void Update()
        {
            if (IsActive)
            {
                foreach (var collector in _collectors)
                {
                    if (collector.Collect(Time.deltaTime, GetCurrentTicksPerSecond()))
                    {
                        var collectStats = UpgradeManager.Instance.CurrencySources[currencySourceData.Tag].CollectStats;

                        //The currencies collected are held in a dictionary "CollectStats". 
                        //Key: Currency, Value: CollectTypeStatBlock (Holds stuff like BaseCollect and multipliers.)
                        foreach (KeyValuePair<Currency, CollectTypeStatBlock> item in collectStats)
                        {
                            int collectAmount = GetCurrentCollectAmount(item.Key, currencySourceData.Tag);
                            if (collectAmount != 0)
                            {
                                CollectCurrency(item.Key, collectAmount);
                                DoPopup(item.Key, collectAmount, collector.position);
                            }
                        }

                    }

                }
            }
        }



    }
}

