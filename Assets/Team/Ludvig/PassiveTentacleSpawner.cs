using System;
using UnityEngine;
using UnityEngine.UIElements;


public class PassiveTentacleSpawner : MonoBehaviour
{
    public GameObject AnimatedObject;
    public Transform InstanceParent;

    public bool SpawnOnLine;
    public Line line;




    public void SpawnObject(Collector collector)
    {
        if (SpawnOnLine)
        {
            Vector2 spawnPos = Vector2.Lerp(line.Start, line.End, UnityEngine.Random.Range(0f, 1f));
            SpawnAtPoint(collector, spawnPos);
            return;
        }

        GameObject newInstance = Instantiate(AnimatedObject, InstanceParent);
        newInstance.GetComponent<CollectorDrivenAnimation>().collector = collector;
    }

    private void SpawnAtPoint(Collector collector, Vector2 spawnPosition)
    {
        GameObject newInstance = Instantiate(AnimatedObject, spawnPosition, Quaternion.identity, InstanceParent);
        newInstance.GetComponent<CollectorDrivenAnimation>().collector = collector;
    }
}
