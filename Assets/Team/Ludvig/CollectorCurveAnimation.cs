using UnityEngine;
using UnityEngine.UI;

public class CollectorCurveAnimation : CollectorDrivenAnimation
{
    public float Multiplier = 1f;
    private Animator animator;
    public VictimSystem victimSystem;

    void Start()
    {   
        victimSystem = FindAnyObjectByType<VictimSystem>();
        animator = GetComponent<Animator>();
        
        if(Random.Range(0, 2) == 1)
        {
            transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
        }
    }
    public override void UpdateAnimation()
    {
        animator.SetFloat("Time", collector.NormalizedTime);
    }
}
