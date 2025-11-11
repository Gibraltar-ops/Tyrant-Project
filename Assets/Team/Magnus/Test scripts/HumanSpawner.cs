using UnityEngine;

public class HumanSpawner : MonoBehaviour
{
    public GameObject humanPrefab;
    public Transform absorbPoint;
    public float spawnInterval = 2f;
    public float spawnDistance = 20f;
    public float spawnHeight = -8f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnHuman();
            timer = 0f;
        }
    }

    void SpawnHuman()
    {
        int side = Random.Range(0, 2) == 0 ? -1 : 1;

        Vector3 spawnPos = new Vector3(side * spawnDistance, spawnHeight, 0);
        GameObject human = Instantiate(humanPrefab, spawnPos, Quaternion.identity);

        HumanMovement move = human.GetComponent<HumanMovement>();
        move.target = absorbPoint;
    }

}
