using System.Collections.Generic;
using UnityEngine;

public class VictimSystem : MonoBehaviour
{
    public List<VictimData> victims = new List<VictimData>();

    void Start()
    {
        // Initialize savefile data here.
        // Load victims from savefile if any exist.

        // Example starter victims
        for (int i = 0; i < 1000; i++)
        {
            victims.Add(new VictimData
            {
                position = Random.insideUnitCircle * 5f,
                destination = Vector2.zero,
                speed = Random.Range(1.5f, 3f),
                scale = Random.Range(0.75f, 1.25f),
                wanderTimer = Random.Range(0.5f, 1.5f),
                wanderDirection = Random.insideUnitCircle.normalized
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var victim in victims)
        {
            if (!victim.isCenter)
            {
                Vector2 dir = (victim.destination - victim.position).normalized; // Calculate the movement direction toward the destination

                victim.position += dir * victim.speed * Time.deltaTime; // Move the victim

                // Update scale based on distance to center (smaller when further up and bigger when further down on screen)

                if (Vector2.Distance(victim.position, victim.destination) < 0.1f) { victim.isCenter = true; } // Check if victim reached destination
            }
            else
            {
                victim.wanderTimer -= Time.deltaTime;
                if (victim.wanderTimer <= 0f)
                {
                    victim.wanderDirection = Random.insideUnitCircle.normalized;
                    victim.wanderTimer = Random.Range(0.5f, 1.5f);
                }

                victim.position += victim.wanderDirection * (victim.speed * 0.5f) * Time.deltaTime;

                // Check if victim is out of bounds, if so set new destination toward center and change isCenter to false.
                // Check if victim is marked for death, start timer/wait for feedback then move to itempool if so.
            }
        }
    }
}
