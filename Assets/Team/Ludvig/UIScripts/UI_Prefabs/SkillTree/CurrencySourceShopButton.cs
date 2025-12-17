using System.Collections.Generic;
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
            Counter.SetSource(CurrencySourceData);
            Debug.Log(Counter.currencySource);
            _button = GetComponentInChildren<Button>();

            //Assign Button sprites
            _button.GetComponent<Image>().sprite = CurrencySourceData.ColorStyle.SpriteSet.TreeButtonDefaultSprite;

            SpriteState state = _button.spriteState;
            state.disabledSprite = CurrencySourceData.ColorStyle.SpriteSet.TreeButtonInactiveSprite;
            state.highlightedSprite = CurrencySourceData.ColorStyle.SpriteSet.TreeButtonHoverSprite;
            state.pressedSprite = CurrencySourceData.ColorStyle.SpriteSet.TreeButtonPressedSprite;
            _button.spriteState = state;

            var buttonHierarchy = GetComponent<ButtonHierarchy>();
            buttonHierarchy.ActiveParentLineColor = CurrencySourceData.ColorStyle.MainColor;
            buttonHierarchy.Lock.GetComponent<Image>().color = CurrencySourceData.ColorStyle.MainColor;

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
            _button.onClick.AddListener(Buy);
            //GetComponentInChildren<CurrencySourceShopButton>().CurrencySourceData = CurrencySourceData;
            Invoke("Initialize", 0.01f);

        }

        public void Activate()
        {
            Active = true;
            CheckInteractable(UpgradeManager.Instance.CurrencySources[CurrencySourceData.Tag].Shop.BoughtWithCurrency);
            Color ProgressTrackerColor = EconomyManager.Instance.GetCurrencySpriteData(Lood.UpgradeManager.Instance.CurrencySources[CurrencySourceData.Tag].Shop.BoughtWithCurrency).OverlayColor;
            foreach (GameObject bar in ProgressBars)
            {
                Image image = bar.GetComponent<Image>();
                if (bar.TryGetComponent(out RadialProgressTracker tracker))
                {
                    bar.GetComponent<Image>().color = new Color(ProgressTrackerColor.r, ProgressTrackerColor.g, ProgressTrackerColor.b, 1f);
                }
                else
                {
                    bar.GetComponent<Image>().color = new Color(image.color.r, image.color.g, image.color.b, 1f);
                }
            }
        }

        private void Initialize()
        {

            CheckInteractable(UpgradeManager.Instance.CurrencySources[CurrencySourceData.Tag].Shop.BoughtWithCurrency);
            Color ProgressTrackerColor = EconomyManager.Instance.GetCurrencySpriteData(Lood.UpgradeManager.Instance.CurrencySources[CurrencySourceData.Tag].Shop.BoughtWithCurrency).OverlayColor;
            if (!Active)
            {
                foreach (GameObject bar in ProgressBars)
                {
                    Image image = bar.GetComponent<Image>();
                    if (bar.TryGetComponent(out RadialProgressTracker tracker))
                    {
                        bar.GetComponent<Image>().color = new Color(ProgressTrackerColor.r, ProgressTrackerColor.g, ProgressTrackerColor.b, 0f);
                    }
                    else
                    {
                        bar.GetComponent<Image>().color = new Color(image.color.r, image.color.g, image.color.b, 0f);
                    }
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

