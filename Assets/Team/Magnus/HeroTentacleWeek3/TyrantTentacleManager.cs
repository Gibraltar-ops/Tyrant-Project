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

    void Start()
    {
        if (victimSystem != null)
        {
            Debug.Log(
                $"TYRANT MANAGER START. Using VictimSystem with ID: {victimSystem.GetInstanceID()}",
                victimSystem.gameObject
            );
        }
        else
        {
            Debug.LogError("Tyrant Manager has NO VictimSystem assigned!");
        }
    }

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

        if (targetVictim != null && !targetVictim.isMarkedForDeath)
        {
            TentacleControllerIK freeTentacle = FindFreeTentacle();
            if (freeTentacle != null)
            {
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

    public void DestroyVictim(VictimData victim)
    {
        if (victimSystem != null && victimSystem.victims.Contains(victim))
        {
            victimSystem.victims.Remove(victim);
        }
    }
}
