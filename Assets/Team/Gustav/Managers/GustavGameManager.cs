using UnityEngine;
using Manager;

public class GustavGameManager : MonoBehaviour
{
    private int currentBiomass;
    void Update()
    {
        currentBiomass+=10000;
        UIManager.Instance.UpdateBiomassLabel(currentBiomass);
    }
}
