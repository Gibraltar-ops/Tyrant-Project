using Manager;
using UnityEngine;

public class GustavGameManager : MonoBehaviour
{
    private int currentBiomass;
    private int currentBrainpower;
    
    void Update()
    {
        currentBiomass +=10000;

        UIManager.Instance.UpdateBiomassLabel(currentBiomass);
    }
}
