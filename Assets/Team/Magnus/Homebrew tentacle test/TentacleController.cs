using UnityEngine;



public class TentacleController : MonoBehaviour {
    
  [Header ("Target")]
public Transform mouthPoint;

 [Header ("2D IK (Unity)")]
 public Transform ikTarget;
 public Transform tipBone;

  [Header ("Movement settings")]
  public float reachSpeed = 20f;
  public float returnToIdleSpeed = 10f;
  public float retractSpeed = 1.5f;
  public float retractArcHeight = 2.0f;

  [Header ("Simulated Physics")]
  public float weightLagTime = 0.2f;
  public float weightAmount = 1.5f;

   [Header ("Tentacle Length")]
   public float totalLength = 5f; 
   private Transform grabTarget;
   private Vector3 animatedTargetPosition;
   private Vector3 currentIkPosition;
   private Vector3 ikVelocity;
   private Vector3 idlePosition;

   private float retractProgress = 0f;
   private Vector3 retractStartPoint;

   public enum TentacleState {Idle, Reaching, Retracting, ReturningToIdle}
   private TentacleState currentState = TentacleState.Idle;
   private bool isBusy = false;

   private void Start()
{
    if (tipBone != null && totalLength <= 0f)
    {
        totalLength = Vector3.Distance(transform.position, tipBone.position);
    }

    if (totalLength <= 0f)
    totalLength = 5f; //Fallback

    idlePosition = transform.position - Vector3.up * totalLength * 1.5f;
    animatedTargetPosition = idlePosition;
    currentIkPosition = idlePosition;
}

private void Update ()
{
    Debug.DrawLine(transform.position, animatedTargetPosition, Color.red);

    switch (currentState)
    {
        case TentacleState.Idle:
        animatedTargetPosition = idlePosition;
        break;

        case TentacleState.Reaching:
        if (grabTarget == null)
            {
                currentState = TentacleState.ReturningToIdle;
                break;
            }

            animatedTargetPosition = Vector3.MoveTowards (
                animatedTargetPosition,
                grabTarget.position,
                Time.deltaTime* reachSpeed
            );

            if (Vector3.Distance(animatedTargetPosition, grabTarget.position) < 0.1f)
            {
                currentState = TentacleState.Retracting;
                retractProgress = 0f;
                retractStartPoint = grabTarget.position;
            }
            break;

            case TentacleState.Retracting:
            if (mouthPoint == null || grabTarget == null)
            {
                if (grabTarget != null) Destroy(grabTarget.gameObject);
                grabTarget = null;
                currentState = TentacleState.ReturningToIdle;
                break;
            }

            retractProgress += Time.deltaTime * retractSpeed;
            retractProgress = Mathf.Clamp01(retractProgress);

 
            Vector3 p0 = retractStartPoint;
            Vector3 p_final = mouthPoint.position;
            
            Vector3 p_intermediate = transform.position - (Vector3.up * retractArcHeight);

            if (retractProgress < 0.5f)
            {
             
                float t = retractProgress * 2f; 
                animatedTargetPosition = Vector3.Lerp(p0, p_intermediate, t);
            }
            else
            {
              
                float t = (retractProgress - 0.5f) * 2f;
                animatedTargetPosition = Vector3.Lerp(p_intermediate, p_final, t);
            }

            if (retractProgress >= 1.0f)
            {
                Destroy(grabTarget.gameObject);
                grabTarget = null;
                currentState = TentacleState.ReturningToIdle;
            }

            break;

            case TentacleState.ReturningToIdle:
            animatedTargetPosition = Vector3.MoveTowards(animatedTargetPosition, idlePosition, Time.deltaTime * returnToIdleSpeed);

            if (Vector3.Distance(animatedTargetPosition, idlePosition) < 0.1f)
            {
                ResetTentacle();
            }
            break;
    }
    if (ikTarget != null)
    {
        ikTarget.position = animatedTargetPosition;
    }
}

private void LateUpdate()
{
    if (currentState == TentacleState.Retracting && grabTarget != null && tipBone != null)
    {
        grabTarget.position = tipBone.position;
    }
}

public void StartGrab(Transform newGrabTarget)
{
    if (newGrabTarget == null)
    return;

    if (!isBusy)
    {
        grabTarget = newGrabTarget;

        if (tipBone != null)
        animatedTargetPosition = tipBone.position;
        else 
        animatedTargetPosition = transform.position;

        currentState = TentacleState.Reaching;
        isBusy = true; 
        Debug.Log ($"Tentacle {name} is reaching for {newGrabTarget.name}");
    }
}

public void ResetTentacle()
{
    isBusy = false;
    grabTarget = null;
    currentState = TentacleState.Idle;
    Debug.Log ($"Tentacle {name} is available");
}

public bool IsBusy()
{
    return isBusy;
}

private Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
{
    float u = 1 - t;
    float tt = t * t;
    float uu = u * u;
    Vector3 p = uu * p0;
    p += 2 * u * t * p1;
    p += tt * p2;
    return p;
}
}