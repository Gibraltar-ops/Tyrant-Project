using UnityEngine;

public class GadflyData
{
    // Gameobject Attributes
    public GameObject gadflyGameObject;
    public Vector2 position;
    public Vector2 destination;
    public Vector2 wanderDirection;

    // Destination Regions
    public DestinationRegion homeRegion;
    public DestinationRegion offScreenRegion;

    // Timers
    public float wanderTimer;
    public float harvestTimer;

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
    public bool isLeftSide;
    public bool isCarryingVictim = false;
}
