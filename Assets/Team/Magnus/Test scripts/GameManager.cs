using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI bioMassText;
    public double biomass = 0;
    public double brainpower = 0;
    private double biomassMultiplier = 1;

    private void Update()
    {
        if (bioMassText != null)
            bioMassText.text = $"Biomass: {biomass:N0}";
    }

    public void AddBiomass(double amount)
    {
        double scaledAmount = amount * System.Math.Pow(biomassMultiplier, biomass / 100);
        biomass += scaledAmount;
    }

    public void AddBrainpower(double amount)
    {
        brainpower += amount;
    }
}
