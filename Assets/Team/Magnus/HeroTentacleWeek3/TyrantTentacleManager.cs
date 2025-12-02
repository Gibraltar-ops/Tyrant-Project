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
        IGrabbable target = FindClosestGrabbable(clickPosition, grabRadius);

        if (target != null && !target.IsGrabbed)
        {
            TentacleControllerIK freeTentacle = FindFreeTentacle();
            if (freeTentacle != null)
            {
                target.IsGrabbed = true;
                freeTentacle.StartGrab(target, mouthPoint);
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

        List<int> freeIndices = new List<int>();
        for (int i = 0; i < activeTentacleCount; i++)
        {
            if (!tentacles[i].IsBusy())
                freeIndices.Add(i);
        }

        if (freeIndices.Count == 0)
            return null;

        int randomIndex = freeIndices[Random.Range(0, freeIndices.Count)];
        lastUsedTentacleIndex = randomIndex;
        return tentacles[randomIndex];
    }

    private IGrabbable FindClosestGrabbable(Vector2 position, float maxDistance)
    {
        IGrabbable closest = null;
        float closestDistSqr = maxDistance * maxDistance;

        var grabbables = new List<IGrabbable>();

        if (victimSystem != null)
        {
            grabbables.AddRange(victimSystem.victims);
        }
        if (resistanceSystem != null)
        {
            grabbables.AddRange(resistanceSystem.fighters);
        }

        foreach (var grabbable in grabbables)
        {
            if (grabbable.IsGrabbed)
                continue;

            float distSqr = (grabbable.Position - position).sqrMagnitude;
            if (distSqr < closestDistSqr)
            {
                closestDistSqr = distSqr;
                closest = grabbable;
            }
        }
        return closest;
    }

    public void ActivateNextTentacle()
    {
        if (tentacles == null)
            return;

        if (activeTentacleCount < tentacles.Count)
        {
            activeTentacleCount++;

            // Here we can add visual feedback or stuff!! / Magnus
            Debug.Log($"Tentacle{activeTentacleCount} activated");
        }
        else
        {
            Debug.Log("All tentacles already active");
        }
    }

    public void DestroyGrabbable(IGrabbable grabbable)
    {
        if (grabbable is VictimData victim)
        {
            if (victimSystem != null && victimSystem.victims.Contains(victim))
            {
                victimSystem.victims.Remove(victim);
            }
        }
        else if (grabbable is ResistanceData fighter)
        {
            if (resistanceSystem != null && resistanceSystem.fighters.Contains(fighter))
            {
                resistanceSystem.fighters.Remove(fighter);
            }
        }
    }
}
