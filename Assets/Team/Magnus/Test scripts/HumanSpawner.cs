using UnityEngine;

public class HumanSpawner : MonoBehaviour
{
    public GameObject humanPrefab;
    public Transform grabPoint;
    public float spawnInterval = 5f;
    public float spawnDistance = 20f;
    public float spawnHeight = -8f;
    public int maxCount = 50;
    public int victimCount = 0;

    private float timer;

    void OnEnable()
    {
        DieInTrigger.onHumanEaten += DecreaseCount;
    }
    void OnDisable()
    {
        DieInTrigger.onHumanEaten -= DecreaseCount;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            if (victimCount < maxCount)
            {
                SpawnHuman();
                victimCount++;
            }

            timer = 0f;
        }
    }

    public void DecreaseCount()
    {
        victimCount--;

    }

    public void SpawnHuman()
    {
        int side = Random.Range(0, 2) == 0 ? -1 : 1;

        Vector3 spawnPos = new Vector3(side * spawnDistance, spawnHeight, 0);
        GameObject human = Instantiate(humanPrefab, spawnPos, Quaternion.identity);

        HumanMovement move = human.GetComponent<HumanMovement>();
        move.target = grabPoint;
    }

}
