using UnityEngine;

public class GameSceneCleanup : MonoBehaviour
{
    void Awake()
    {

        if (AudioManager.Instance != null)
        {
            Destroy(AudioManager.Instance.gameObject);
        }

        if (Lood.UpgradeManager.Instance != null)
        {
            Destroy(Lood.UpgradeManager.Instance.gameObject);
        }

        if (EconomyManager.Instance != null)
        {
            Destroy(EconomyManager.Instance.gameObject);
        }
    }
}