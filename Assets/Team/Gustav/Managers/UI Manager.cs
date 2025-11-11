using UnityEngine;
using TMPro;

namespace Manager
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI currentBiomassLabel;
        [SerializeField] private TextMeshProUGUI currentBrainLabel;
        public static UIManager Instance;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        public void UpdateBiomassLabel(double value)
        {
            string labelValue = FormatNumbers.Format(value);
            currentBiomassLabel.text = labelValue;
        }
        public void UpdateBrainLabel(double value)
        {
            string labelValue = FormatNumbers.Format(value);
            currentBrainLabel.text = labelValue;
        }
    }
}
