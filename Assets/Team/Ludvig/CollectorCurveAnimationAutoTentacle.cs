
using UnityEngine;
using UnityEngine.UI;

public class CollectorCurveAnimationAutoTentacle : CollectorDrivenAnimation
{
    public float Multiplier = 1f;
    private Animator animator;
    public VictimSystem victimSystem;
    private bool _victimDestroyed = false;
    private VictimData closestVictim;
    private Vector2 originPosition;
    public GameObject BloodSplatter;

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
        

        if(collector.NormalizedTime < 0.02f && _victimDestroyed == true)
        {   
            _victimDestroyed = false;
            closestVictim = FindAnyVictim();
            originPosition = new Vector2(transform.position.x, transform.position.y);

            if(closestVictim == null)
            {
                
                collector.Reset();
                _victimDestroyed = true;
            }
            
        }

        if(collector.NormalizedTime > 0.02f && closestVictim != null)
        {
            transform.position = Vector2.Lerp(originPosition, closestVictim.position, collector.NormalizedTime *4f);
            //transform.position = Vector2.Lerp(originPosition, closestVictim.position, collector.NormalizedTime *2f);
        }

        if(collector.NormalizedTime > 0.52f && !_victimDestroyed)
        { 
            
            if(closestVictim != null)
            {
                Instantiate(BloodSplatter, closestVictim.position, Quaternion.identity);
                DestroyVictim(closestVictim);
                closestVictim = null;
            
            }
            
            _victimDestroyed = true;
        }
        
        
    }

     private VictimData FindAnyVictim()
    {
        VictimData closest = null;

        foreach (var victim in victimSystem.victims)
        {
            if (victim.isMarkedForDeath)
            {
                continue;
            }
            if (!victimSystem.destinationRegions[0].Contains(victim.position))
            {
                continue;
            }

                victim.isMarkedForDeath = true;
                closest = victim;
                return closest;
              
        }
        return closest;
    }

    public void DestroyVictim(VictimData victim)
    {
        if (victimSystem != null && victimSystem.victims.Contains(victim))
        {
            victimSystem.victims.Remove(victim);
        }
    }
}

