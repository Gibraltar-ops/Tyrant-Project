using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HumanSystem : MonoBehaviour
{
    public List<HumanData> humans = new List<HumanData>();

    void Start()
    {
        // Initialize savefile data here.
        // Load humans from savefile if any exist.

        // Example starter humans
        for (int i = 0; i < 100; i++)
        {
            humans.Add(new HumanData
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
        foreach (var human in humans)
        {
            if (!human.isCenter)
            {
                Vector2 dir = (human.destination - human.position).normalized; // Calculate the movement direction toward the destination

                human.position += dir * human.speed * Time.deltaTime; // Move the human

                // Update scale based on distance to center (smaller when further up and bigger when further down on screen)

                if (Vector2.Distance(human.position, human.destination) < 0.1f) { human.isCenter = true; } // Check if human reached destination
            }
            else
            {
                human.wanderTimer -= Time.deltaTime;
                if (human.wanderTimer <= 0f)
                {
                    human.wanderDirection = Random.insideUnitCircle.normalized;
                    human.wanderTimer = Random.Range(0.5f, 1.5f);
                }

                human.position += human.wanderDirection * (human.speed * 0.5f) * Time.deltaTime;

                // Check if human is out of bounds, if so set new destination toward center and change isCenter to false.
                // Check if human is marked for death, start timer/wait for feedback then move to itempool if so.
            }
        }
    }
}
