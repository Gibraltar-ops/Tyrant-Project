using System.Collections.Generic;
using UnityEngine;

public class TyrantTentacleManager : MonoBehaviour
{
    [Header("Tentacle Management")]
    [SerializeField]
    private List<TentacleControllerIK> tentacles;

    [SerializeField]
    private int activeTentacleCount = 1;
    private int lastUsedTentacleIndex = -1;

    [Header("Targeting")]
    [SerializeField]
    private float grabRadius = 5.0f;

    [SerializeField]
    private VictimSystem victimSystem;

    [SerializeField]
    private ResistanceSystem resistanceSystem;

    [Header("Dependencies")]
    [SerializeField]
    private Transform mouthPoint; // all tentacles need this variable

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Click");
            TryGrabTargetAtClick(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
    }

    private void TryGrabTargetAtClick(Vector2 clickPosition)
    {
        VictimData targetVictim = FindClosestVictim(clickPosition, grabRadius);
        Debug.Log("Searching for victim... Found: " + (targetVictim != null));

        if (targetVictim != null && !targetVictim.isMarkedForDeath)
        {
            Debug.Log("Victim is valid. Searching for a free tentacle...");
            TentacleControllerIK freeTentacle = FindFreeTentacle();
            if (freeTentacle != null)
            {
                Debug.Log("Found free tentacle! Commanding to grab: " + freeTentacle.name);
                targetVictim.isMarkedForDeath = true;
                freeTentacle.StartGrab(targetVictim, mouthPoint);
            }
            else
            {
                Debug.Log("All tentacles are busy or none are available.");
            }
        }
    }

    private TentacleControllerIK FindFreeTentacle()
    {
        if (tentacles == null || activeTentacleCount == 0)
            return null;

        for (int i = 0; i < activeTentacleCount; i++)
        {
            int indexToTry = (lastUsedTentacleIndex + 1 + i) % activeTentacleCount;
            if (!tentacles[indexToTry].IsBusy())
            {
                lastUsedTentacleIndex = indexToTry;
                return tentacles[indexToTry];
            }
        }
        return null;
    }

    private VictimData FindClosestVictim(Vector2 position, float maxDistance)
    {
        VictimData closest = null;
        float closestDistSqr = maxDistance * maxDistance;

        foreach (var victim in victimSystem.victims)
        {
            if (victim.isMarkedForDeath)
                continue;

            float distSqr = (victim.position - position).sqrMagnitude;
            if (distSqr < closestDistSqr)
            {
                closestDistSqr = distSqr;
                closest = victim;
            }
        }
        return closest;
    }
}
