using UnityEngine;

public class CollectorDrivenAnimation : MonoBehaviour
{
    public Collector collector;

    void Update()
    {
        UpdateAnimation();
    }

    public virtual void UpdateAnimation()
    {

    }
}
