using System.Collections.Generic;
using UnityEngine;

public class OffensiveTentacleController : MonoBehaviour
{
    [Header("Settings")]
    public GameObject tentaclePrefab;
    public int numberOfTentacles = 6;
    public float spread = 2f;

    [Header("Timing")]
    public float minDelay = 0.0f;
    public float maxDelay = 0.2f;

    private List<OffensiveSprawlTentacle> myTentacles = new List<OffensiveSprawlTentacle>();

    void Start()
    {
        SpawnTentacles();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            StartAttack(mousePos);
        }
    }

    void SpawnTentacles()
    {
        foreach (var t in myTentacles)
        {
            if (t != null) Destroy(t.gameObject);
        }
        myTentacles.Clear();

        for (int i = 0; i < numberOfTentacles; i++)
        {
            GameObject newObj = Instantiate(tentaclePrefab, transform.position, Quaternion.identity);
            newObj.transform.parent = transform;

            OffensiveSprawlTentacle script = newObj.GetComponent<OffensiveSprawlTentacle>();
            myTentacles.Add(script);
        }
    }

    public void StartAttack(Vector3 targetPosition)
    {
        foreach (OffensiveSprawlTentacle tentacle in myTentacles)
        {
            Vector3 randomOffset = Random.insideUnitCircle * spread;

            float randomDelay = Random.Range(minDelay, maxDelay);


            tentacle.Attack(transform.position, targetPosition + randomOffset, randomDelay);
        }
    }
}