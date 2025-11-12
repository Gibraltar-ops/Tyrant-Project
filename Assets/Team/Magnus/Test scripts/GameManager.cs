using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI bioMassText;
    public double biomass = 100;
    public double brainpower = 0;
    private double biomassMultiplier = 1;

    private void Update()
    {
        if (bioMassText != null)
            bioMassText.text = $"Biomass: {biomass:N0}";
    }

    public void AddBiomass(float amount)
    {
        double scaledAmount = amount * System.Math.Pow(biomassMultiplier, biomass / 100);
        biomass += amount;
    }

    public void AddBrainpower(double amount)
    {
        brainpower += amount;
    }
}
