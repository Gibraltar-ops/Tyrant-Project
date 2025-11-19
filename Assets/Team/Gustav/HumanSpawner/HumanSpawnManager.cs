using UnityEngine;

public class HumanSpawnManager : MonoBehaviour
{
    public Transform grabPoint;
    public Transform gatherArea;
    public float gatherAreaRadius = 10f;

    [Header("Spawn Settings")]
    public Transform spawnLocation1;
    public Transform spawnLocation2;
    public Transform spawnLocation3;
    public Transform spawnLocation4;

    [Header("Values")]
    public float spawnInterval = 5f;
    public float capsuleWidth = 1f;
    public float capsuleLength = 2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialization code here
        // Spawn humans inside gather area/zone based on load data from previous game if any.
    }

    // Update is called once per frame
    void Update()
    {
        // Code which spawns humans over time in intervals based on feedback from Gamemanager which tells the spawn multiplier. 
        // Code which updates the current amout of humans in the gather area to the UI Text inside it's hiarchy.
    }
    public void SpawnHuman()
    {
        // Spawn a human at one random spawn location and make it walk towards the gather area over time.
        // Pass instructions to humanAI about which behavior to use. How many humans to spawn.
        // if spawn instruction is given when more than 200 humans are present in gather area, spawn group sprite instead of singular.
        // if less then spawn the amount of humans singularly.
    }
    public void AbductHuman()
    {
        //Code here which will remove the saved human from the pool and delete the sprite from the gather area but only
        //if a certain amount of humans are left. If more than 500 himans are present instead shift them around around eachother.
    }
    public void AddHumanToCrowd()
    {
        // Code here which updates the current amount of humans in the gather area when a human reaches it.
    }
}
