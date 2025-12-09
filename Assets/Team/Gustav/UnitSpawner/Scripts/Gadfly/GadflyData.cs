using UnityEngine;

public class GadflyData
{
    // Gameobject Attributes
    public Vector2 position;
    public Vector2 destination;
    public Vector2 wanderDirection;

    // Destination Regions
    public DestinationRegion homeRegion;
    public DestinationRegion harvestRegion;

    // Timers
    public float wanderTimer;

    // Currency Amounts and Multipliers
    public float biomassAmount;
    public float biomassMultiplier;
    public float brainmassAmount;
    public float brainmassMultiplier;

    // Gadfly attributes
    public float wanderSpeed;
    public float harvestSpeed;
    public float scale;

    // Bool States
    public bool isHome = false;
    public bool isHarvesting = false;
    public bool hasReachedHarvest = false;
    public bool isLeftSide;
    public bool isCarryingVictim = false;

    // Other attributes
    public Vector2 velocity;
    public float arcStrength;
    public float jitterStrength;
}
