using UnityEngine;

[RequireComponent(typeof(LineRenderer))]

public class TentacleController : MonoBehaviour
{
 [Header ("IK Settings")]
 public int segments = 5; 
 public float [] segmentLengths;
 public float totalLength;

 [Header("Target")]

public Transform mouthPoint;

[Header ("Movement settings")]
public float reachSpeed = 20f;

public float returnToIdleSpeed = 10f;
public float retractSpeed = 1.5f; 

public float retractArcHeight = 2.0f;

private Transform grabTarget;
private Vector3 animatedTargetPosition;
private Vector3 idlePosition;

private Vector3[] segmentPositions;
private LineRenderer lineRenderer;

private float retractProgress = 0f;
private Vector3 retractStartPoint;
private Vector3 retractControlPoint;

public enum TentacleState {Idle, Reaching, Retracting, ReturningToIdle}
private TentacleState currentState = TentacleState.Idle;
private bool isBusy = false;

void Awake ()
    {
        lineRenderer = GetComponent<LineRenderer>();
        segmentPositions = new Vector3[segments + 1];
        segmentLengths = new float[segments];

        for (int i = 0; i < segments; i++)
        {
            segmentLengths[i] = 1f; 
        }

        ResetPositions();
    }
void Start()
    {
        idlePosition = transform.position - Vector3.up * totalLength * 1.5f;
        animatedTargetPosition = idlePosition;
    }
    void Update()
    {
        segmentPositions[0] = transform.position;

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
                animatedTargetPosition = Vector3.MoveTowards(animatedTargetPosition, grabTarget.position, Time.deltaTime * reachSpeed);
                if (Vector3.Distance(animatedTargetPosition, grabTarget.position) < 0.1f)
                {
                    currentState = TentacleState.Retracting;
                    retractProgress = 0f;
                    retractStartPoint = grabTarget.position;
                    float horizontalDirection = Mathf.Sign(retractStartPoint.x - mouthPoint.position.x);
                    Vector3 arcDirection = Vector3.left * horizontalDirection;
                    retractControlPoint = (retractStartPoint + mouthPoint.position) / 2 + arcDirection * retractArcHeight;
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

          
            Vector3 posOnCurve = CalculateQuadraticBezierPoint(retractProgress, retractStartPoint, retractControlPoint, mouthPoint.position);
            animatedTargetPosition = posOnCurve;

           
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

        IKSolver.Solve(segmentPositions, segmentLengths, animatedTargetPosition);
        UpdateLineRenderer();
    }

    void LateUpdate()
    {
        if (currentState == TentacleState.Retracting && grabTarget != null)
    {
        grabTarget.position = segmentPositions[segments];
    }
    }

     Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector3 p = uu * p0; 
        p += 2 * u * t * p1; 
        p += tt * p2; 
        return p;
    }

        void UpdateLineRenderer()
        {lineRenderer.positionCount = segments +1;
        lineRenderer.SetPositions (segmentPositions);
        }
    void ResetPositions()
    {
        totalLength = 0;
        for (int i = 0; i < segments; i++)
        {
            totalLength += segmentLengths[i];
        }

        segmentPositions[0] = transform.position;
        for (int i = 1; i <= segments; i++)
        {
            segmentPositions[i] = segmentPositions[i - 1] - Vector3.up * segmentLengths[i - 1];
        }


    }

    public void StartGrab(Transform newGrabTarget)
    {
        if (!isBusy)
        {
            this.grabTarget = newGrabTarget;
            animatedTargetPosition = segmentPositions [segments];
            currentState = TentacleState.Reaching;
            isBusy = true; 
            Debug.Log ($"Tentakel {name} sträcer sig efter {newGrabTarget.name}");

        }
    }

    public void ResetTentacle()
    {
        isBusy = false; 
        grabTarget = null;
        currentState = TentacleState.Idle;
        Debug.Log ($"Tentakel {name} är ledig");
    }

    public bool IsBusy()
    {
        return isBusy;
    }
}
