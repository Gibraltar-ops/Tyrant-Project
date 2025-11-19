using UnityEngine;

public class HumanData
{
    public Vector2 position;
    public Vector2 destination;
    public Vector2 wanderDirection;
    public float wanderTimer;
    public float wanderRadius; // ??? Behövs kanske inte ???


    public float speed;
    public float scale;
    public int value;


    public bool isCenter = false;
    public bool isMarkedForDeath = false;
}
