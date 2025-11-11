using UnityEngine;
using Manager;

public class GustavGameManager : MonoBehaviour
{
    private int currentBiomass;
    void Update()
    {
        currentBiomass++;
        UIManager.Instance.UpdateBiomassLabel(currentBiomass);
    }
}
