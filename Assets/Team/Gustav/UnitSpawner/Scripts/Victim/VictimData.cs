using UnityEngine;

public class VictimData : IGrabbable
{
    public GameObject victimGameObject;
    public Vector2 position;
    public Vector2 destination;
    public Vector2 wanderDirection;
    public DestinationRegion homeRegion;

    public float wanderTimer;

    public float speed;
    public float scale;
    public int value;

    public bool isCenter = false;
    public bool isMarkedForDeath = false;

    public Vector2 Position => position;
    public bool IsGrabbed
    {
        get => isMarkedForDeath;
        set => isMarkedForDeath = value;
    }
    public GrabbableType Type => GrabbableType.Victim;
    public GameObject gameObject => victimGameObject;

    public void DestroySelf()
    {
        if (victimGameObject != null)
        {
            Object.Destroy(victimGameObject);
        }
    }
}
