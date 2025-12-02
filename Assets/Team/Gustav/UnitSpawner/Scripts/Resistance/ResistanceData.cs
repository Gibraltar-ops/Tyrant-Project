using UnityEngine;

public class ResistanceData : IGrabbable
{
    public GameObject resistanceGameObject;
    public Vector2 position;
    public Vector2 destination;
    public Vector2 wanderDirection;

    public DestinationRegion homeRegion;

    public float wanderTimer;
    public float attackTimer;
    public int attackDamage;

    public float speed;
    public float scale;
    public int value;

    public bool isCenter = false;
    public bool isMarkedForDeath = false;
    public bool isLeftSide;

    public Vector2 Position => position;
    public bool IsGrabbed
    {
        get => isMarkedForDeath;
        set => isMarkedForDeath = value;
    }
    public GrabbableType Type => GrabbableType.Fighter;
    public GameObject gameObject => resistanceGameObject;

    public void DestroySelf()
    {
        if (resistanceGameObject != null)
        {
            Object.Destroy(resistanceGameObject);
        }
    }
}
