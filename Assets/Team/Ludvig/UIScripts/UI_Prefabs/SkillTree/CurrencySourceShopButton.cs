using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Lood
{
    public class CurrencySourceShopButton : MonoBehaviour
    {

        public CurrencySourceSO CurrencySourceData;

        public CurrencySourceValueTracker Counter;

        [HideInInspector]
        public Button _button;
        public bool _maxedOut = false;
        public bool Active = false;
        public List<GameObject> ProgressBars;

        //Event is received on the shop objects in the scene.
        public delegate void OnCollectorPurchased(string tag);
        public static event OnCollectorPurchased onCollectorPurchased;

        void OnEnable()
        {
            EconomyManager.onCurrencyChanged += CheckInteractable;

        }
        void OnDisable()
        {
            EconomyManager.onCurrencyChanged -= CheckInteractable;
        }
        void Awake()
        {
            Counter.currencySource = CurrencySourceData;

            foreach (GameObject bar in ProgressBars)
            {
                if (bar.TryGetComponent(out RadialProgressTracker tracker))
                {
                    tracker.trackedPurchaseable = CurrencySourceData;
                }
            }
        }

        void Start()
        {
            _button = GetComponentInChildren<Button>();
            _button.onClick.AddListener(Buy);
            //GetComponentInChildren<CurrencySourceShopButton>().CurrencySourceData = CurrencySourceData;
            Invoke("Initialize", 0.01f);

        }

        public void Activate()
        {
            Active = true;
            CheckInteractable(UpgradeManager.Instance.CurrencySources[CurrencySourceData.Tag].Shop.BoughtWithCurrency);
            foreach (GameObject bar in ProgressBars)
            {
                Image image = bar.GetComponent<Image>();
                bar.GetComponent<Image>().color = new Color(image.color.r, image.color.g, image.color.b, 1f);
            }
        }

        private void Initialize()
        {

            CheckInteractable(UpgradeManager.Instance.CurrencySources[CurrencySourceData.Tag].Shop.BoughtWithCurrency);
            if (!Active)
            {
                foreach (GameObject bar in ProgressBars)
                {
                    Image image = bar.GetComponent<Image>();
                    bar.GetComponent<Image>().color = new Color(image.color.r, image.color.g, image.color.b, 0f);
                }
            }
        }

        private void CheckInteractable(Currency currency)
        {
            if (!Active)
            {
                _button.interactable = false;
                return;
            }

            if (_maxedOut) return;

            if (UpgradeManager.Instance.CurrencySources[CurrencySourceData.Tag].Shop.MaxedOut)
            {
                _button.interactable = false;
                _maxedOut = true;
                return;
            }

            if (currency != UpgradeManager.Instance.CurrencySources[CurrencySourceData.Tag].Shop.BoughtWithCurrency)
            {
                return;
            }

            if (UpgradeManager.Instance.CurrencySources[CurrencySourceData.Tag].Shop.MaxedOut)
            {
                return;
            }

            if (EconomyManager.Instance.CanAfford(currency, (int)UpgradeManager.Instance.CurrencySources[CurrencySourceData.Tag].Shop.NextPrice))
            {
                _button.interactable = true;
            }
            else
            {
                _button.interactable = false;
            }
        }

        public void Buy()
        {
            onCollectorPurchased?.Invoke(CurrencySourceData.Tag);
        }
    }
}

