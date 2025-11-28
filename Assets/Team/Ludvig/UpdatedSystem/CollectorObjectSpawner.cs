using System;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public struct Line
{
    public Vector2 Start;
    public Vector2 End;

}

public class CollectorObjectSpawner : MonoBehaviour
{
    public GameObject AnimatedObject;
    public Transform InstanceParent;

    public bool SpawnOnLine;
    public Line line;

    public Transform LineStart;
    public Transform LineEnd;

    public Vector2 ScaleMultiplier = new Vector2(1f, 1f);




    public void SpawnObject(Collector collector)
    {
        if (SpawnOnLine)
        {
            Vector3 spawnPos = Vector3.Lerp(LineStart.position, LineEnd.position, UnityEngine.Random.Range(0f, 1f));
            SpawnAtPoint(collector, spawnPos);
            return;
        }

        GameObject newInstance = Instantiate(AnimatedObject, InstanceParent);
        newInstance.GetComponent<CollectorDrivenAnimation>().collector = collector;
    }

    private void SpawnAtPoint(Collector collector, Vector3 spawnPosition)
    {
        GameObject newInstance = Instantiate(AnimatedObject, spawnPosition, Quaternion.identity, InstanceParent);
        newInstance.GetComponent<CollectorDrivenAnimation>().collector = collector;
        newInstance.transform.localScale *= ScaleMultiplier;
    }
}
