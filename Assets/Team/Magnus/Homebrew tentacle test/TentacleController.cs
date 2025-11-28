using UnityEngine;

public class TentacleController : MonoBehaviour
{
    private class Grabbable
    {
        private VictimData victim;
        private ResistanceData resistance;

        public bool IsVictim => victim != null;
        public bool IsResistance => resistance != null;

        public Vector3 Position
        {
            get
            {
                if (IsVictim)
                    return victim.position;
                if (IsResistance)
                    return resistance.position;
                return Vector3.zero;
            }
            set
            {
                if (IsVictim)
                    victim.position = value;
                if (IsResistance)
                    resistance.position = value;
            }
        }

        public Grabbable(VictimData data)
        {
            victim = data;
        }

        public Grabbable(ResistanceData data)
        {
            resistance = data;
        }

        public void RemoveFromSystem(VictimSystem vSystem, ResistanceSystem rSystem)
        {
            if (IsVictim && vSystem != null && vSystem.victims.Contains(victim))
            {
                vSystem.victims.Remove(victim);
            }
            else if (IsResistance && rSystem != null && rSystem.fighters.Contains(resistance))
            {
                rSystem.fighters.Remove(resistance);
            }
        }
    }

    [Header("Target")]
    public Transform mouthPoint;

    [Header("2D IK (Unity)")]
    public Transform ikTarget;
    public Transform tipBone;

    [Header("Movement settings")]
    public float reachSpeed = 20f;
    public float returnToIdleSpeed = 10f;
    public float retractSpeed = 1.5f;
    public float retractArcHeight = 2.0f;

    [Header("Simulated Physics")]
    public float weightLagTime = 0.2f;
    public float weightAmount = 1.5f;

    [Header("Tentacle Length")]
    public float totalLength = 5f;

    private Grabbable currentGrabTarget;

    /*   [SerializeField]
      private VictimData grabTarget; */

    [SerializeField]
    private VictimSystem victimSystem;

    [SerializeField]
    private ResistanceSystem resistanceSystem;
    private TyrantController tyrantController;
    private Vector3 animatedTargetPosition;
    private Vector3 currentIkPosition;
    private Vector3 ikVelocity;
    private Vector3 idlePosition;

    private float retractProgress = 0f;
    private Vector3 retractStartPoint;

    public enum TentacleState
    {
        Idle,
        Reaching,
        Retracting,
        ReturningToIdle,
    }

    private TentacleState currentState = TentacleState.Idle;
    private bool isBusy = false;

    private void Start()
    {
        tyrantController = FindFirstObjectByType<TyrantController>();
        if (mouthPoint == null && tyrantController != null)
        {
            mouthPoint = tyrantController.mouthPoint;
        }

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

    private void Update()
    {
        Debug.DrawLine(transform.position, animatedTargetPosition, Color.red);

        switch (currentState)
        {
            case TentacleState.Idle:
                animatedTargetPosition = idlePosition;
                break;

            case TentacleState.Reaching:
                if (currentGrabTarget == null)
                {
                    currentState = TentacleState.ReturningToIdle;
                    break;
                }

                animatedTargetPosition = Vector3.MoveTowards(
                    animatedTargetPosition,
                    currentGrabTarget.Position,
                    Time.deltaTime * reachSpeed
                );

                if (Vector3.Distance(animatedTargetPosition, currentGrabTarget.Position) < 0.1f) // ÄNDRAD
                {
                    currentState = TentacleState.Retracting;
                    retractProgress = 0f;
                    retractStartPoint = currentGrabTarget.Position;
                }
                break;

            case TentacleState.Retracting:
                if (mouthPoint == null || currentGrabTarget == null)
                {
                    currentGrabTarget = null;
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
                    currentGrabTarget.RemoveFromSystem(victimSystem, resistanceSystem);

                    if (tyrantController != null)
                    {
                        tyrantController.gameManager.biomass += tyrantController.biomassPerHuman;
                        tyrantController.UpdateTextFields();
                        EconomyManager.Instance.IncrementCurrency(
                            Currency.Biomass,
                            (long)tyrantController.biomassPerHuman
                        );
                    }

                    currentGrabTarget = null;
                    currentState = TentacleState.ReturningToIdle;
                }

                break;

            case TentacleState.ReturningToIdle:
                animatedTargetPosition = Vector3.MoveTowards(
                    animatedTargetPosition,
                    idlePosition,
                    Time.deltaTime * returnToIdleSpeed
                );

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
        if (
            currentState == TentacleState.Retracting
            && currentGrabTarget != null
            && tipBone != null
        )
        {
            currentGrabTarget.Position = tipBone.position;
        }
    }

    public void StartGrab(VictimData newGrabTarget, VictimSystem system)
    {
        if (newGrabTarget == null || isBusy)
            return;

        currentGrabTarget = new Grabbable(newGrabTarget);
        victimSystem = system;
        resistanceSystem = null; // Nollställ den andra

        StartGrabbing();
    }

    public void StartGrab(ResistanceData newGrabTarget, ResistanceSystem system)
    {
        if (newGrabTarget == null || isBusy)
            return;

        currentGrabTarget = new Grabbable(newGrabTarget);
        resistanceSystem = system;
        victimSystem = null; // Nollställ den andra

        StartGrabbing();
    }

    private void StartGrabbing()
    {
        if (tipBone != null)
            animatedTargetPosition = tipBone.position;
        else
            animatedTargetPosition = transform.position;

        currentState = TentacleState.Reaching;
        isBusy = true;
    }

    public void ResetTentacle()
    {
        isBusy = false;
        currentGrabTarget = null;
        victimSystem = null;
        resistanceSystem = null;
        currentState = TentacleState.Idle;
        //Debug.Log ($"Tentacle {name} is available");
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
