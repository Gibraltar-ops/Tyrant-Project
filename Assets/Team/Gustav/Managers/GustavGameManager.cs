using Manager;
using TMPro;
using UnityEngine;

public class GustavGameManager : MonoBehaviour
{
    private int currentBiomass;
    private int currentBrainpower;
    public TextMeshProUGUI bioMassText;
    public double biomass = 100;
    public double brainpower = 0;
    private double biomassMultiplier = 1;
    
    void Update()
    {
        currentBiomass +=10000;

        UIManager.Instance.UpdateBiomassLabel(currentBiomass);
    }
}
