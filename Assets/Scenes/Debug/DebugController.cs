using UnityEngine;

public class DebugController : MonoBehaviour
{
    [Header("System References")]
    public GadflySystem gadflySystem;
    public ResistanceSystem resistanceSystem;
    public GameObject debugPanel;

    void Start()
    {
        // Auto-find references if not assigned
        if (gadflySystem == null)
            gadflySystem = FindFirstObjectByType<GadflySystem>();
        if (resistanceSystem == null)
            resistanceSystem = FindFirstObjectByType<ResistanceSystem>();
    }

    void Update()
    {
        // Tryck på TAB för att visa/dölja debug-menyn
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (debugPanel != null)
                debugPanel.SetActive(!debugPanel.activeSelf);
        }
    }

    // ---------------------------------------------------------
    // GADFLY DEBUGGING
    // ---------------------------------------------------------

    [ContextMenu("Spawn Gadfly")]
    public void SpawnGadfly()
    {
        if (gadflySystem != null)
        {
            gadflySystem.SpawnGatfly();
            Debug.Log($"Debug: Spawned Gadfly. Total: {gadflySystem.gadflies.Count}");
        }
    }

    [ContextMenu("Upgrade Gadfly Biomass")]
    public void UpgradeGadflyBiomass()
    {
        if (gadflySystem != null)
        {
            gadflySystem.biomassMultiplier += 0.5f;
            Debug.Log(
                $"Debug: Gadfly Biomass Multiplier increased to {gadflySystem.biomassMultiplier}"
            );
        }
    }

    [ContextMenu("Upgrade Gadfly Brainmass")]
    public void UpgradeGadflyBrainmass()
    {
        if (gadflySystem != null)
        {
            gadflySystem.brainmassMultiplier += 0.5f;
            Debug.Log(
                $"Debug: Gadfly Brainmass Multiplier increased to {gadflySystem.brainmassMultiplier}"
            );
        }
    }

    [ContextMenu("Upgrade Gadfly Speed")]
    public void UpgradeGadflySpeed()
    {
        if (gadflySystem != null)
        {
            gadflySystem.harvestMovementSpeed += 1.0f;
            Debug.Log(
                $"Debug: Gadfly Harvest Speed increased to {gadflySystem.harvestMovementSpeed}"
            );
        }
    }

    [ContextMenu("Decrease Harvest Cooldown")]
    public void DecreaseHarvestCooldown()
    {
        if (gadflySystem != null)
        {
            gadflySystem.harvestCooldown = Mathf.Max(1f, gadflySystem.harvestCooldown - 2f);
            Debug.Log($"Debug: Harvest Cooldown decreased to {gadflySystem.harvestCooldown}");
        }
    }

    // ---------------------------------------------------------
    // RESISTANCE DEBUGGING
    // ---------------------------------------------------------

    [ContextMenu("Spawn Resistance Fighter")]
    public void SpawnResistance()
    {
        if (resistanceSystem != null)
        {
            resistanceSystem.SpawnResistance();
            Debug.Log(
                $"Debug: Spawned Resistance Fighter. Total: {resistanceSystem.fighters.Count}"
            );
        }
    }
}
