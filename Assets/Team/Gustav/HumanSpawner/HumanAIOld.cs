using UnityEngine;

public class HumanAI : MonoBehaviour
{
    [Header("HumanAI Variables")]
    public Vector2 targetDestination;

    private bool isAtGatherArea = false;
    private float speed = 2.0f;
    private float fadeOutDuration = 2.0f;

    [Header("Component Variables")]
    private Transform humanTransformation;
    private SpriteRenderer spriteRenderer;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initilize human AI behavior
        // Behavior 1 is to walk towards the gather area then idle/roam near its destination.
        // Behavior 2 is to walk towards the gather area then fade out and disappear near center. (This is when the gathercrowd is too many)
        // Depending on each behavior the sprite in use is determined if it's singular or small group walking.
    }

    // Update is called once per frame
    void Update()
    {
        //Code which sets the sprite scale and increase over time as the human gets closer to the gather area.

        // Code which makes the human walk towards the gather area over time.
        // When reached gather area, start idle/roam behavior or fade out and disappear behavior.

        //Transition between one sprite to another depending on if it needs to update itself to singular or group.
    }
    void IdleBehavior()
    {
        // Code for idle/roam behavior near gather area.
    }
    void DisappearBehavior()
    {
        // Code for fade out and disappear behavior near center of gather area.
        // Then return item to pool.
    }
}
