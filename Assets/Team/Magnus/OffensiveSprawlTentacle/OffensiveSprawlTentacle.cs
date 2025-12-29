using UnityEngine;

public class OffensiveSprawlTentacle : MonoBehaviour
{
    private LineRenderer lineRenderer;

    [Header("Settings")]
    public int segmentCount = 20;
    public float startSpeed = 2f;
    public float maxSpeed = 40f;
    public float retractSpeed = 10f;
    public float acceleration = 20f;

    [Header("Wiggle Settings")]
    public float wiggleSpeed = 15f;
    public float wiggleAmount = 1.5f;

    [Header("Wave / Noise Settings")]
    public float waveFrequency = 5f;
    public float waveAmplitude = 0.5f;
    public float noiseScale = 2f;
    public float noiseAmount = 0.3f;

    private Vector3 startPos;
    private Vector3 targetPos;
    private float currentProgress = 0f;
    private float currentSpeed = 0f;
    private bool isAttacking = false;

    private float randomSeed;
    private float startDelayTimer = 0f;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = segmentCount;
        lineRenderer.enabled = false;
        randomSeed = Random.Range(0f, 100f);
    }

    public void Attack(Vector3 myStart, Vector3 enemyPos, float delay)
    {
        startPos = myStart;
        targetPos = enemyPos;
        startDelayTimer = delay;
        currentProgress = 0f;
        currentSpeed = startSpeed;
        isAttacking = true;
        lineRenderer.enabled = startDelayTimer <= 0f;
    }

    public void Retract()
    {
        isAttacking = false;
        currentSpeed = retractSpeed;
    }

    void Update()
    {
        if (startDelayTimer > 0f)
        {
            startDelayTimer -= Time.deltaTime;
            if (startDelayTimer <= 0f) lineRenderer.enabled = true;
            else return;
        }

        if (!lineRenderer.enabled) return;
        if (transform.parent != null) startPos = transform.parent.position;

        if (isAttacking && currentSpeed < maxSpeed)
        {
            currentSpeed += acceleration * Time.deltaTime;
        }

        float distance = Vector3.Distance(startPos, targetPos);
        if (distance < 0.1f) distance = 1f;

        if (isAttacking)
        {
            currentProgress += (currentSpeed * Time.deltaTime) / distance;
            if (currentProgress >= 1f)
            {
                currentProgress = 1f;
                Retract();
            }
        }
        else
        {
            currentProgress -= (currentSpeed * Time.deltaTime) / distance;
            if (currentProgress <= 0f)
            {
                currentProgress = 0f;
                lineRenderer.enabled = false;
            }
        }

        DrawTentacle();
    }

    void DrawTentacle()
    {
        Vector3 currentTip = Vector3.Lerp(startPos, targetPos, currentProgress);
        Vector3 midPoint = (startPos + currentTip) / 2f;

        float timeVal = (Time.time * wiggleSpeed) + randomSeed;
        float wiggle = Mathf.Sin(timeVal) * wiggleAmount * currentProgress;

        Vector3 direction = (targetPos - startPos).normalized;
        Vector3 sideVector = Vector3.Cross(direction, Vector3.forward);
        if (sideVector == Vector3.zero) sideVector = Vector3.up; // Fallback
        Vector3 curvePoint = midPoint + (sideVector * wiggle);

        for (int i = 0; i < segmentCount; i++)
        {
            float t = (float)i / (segmentCount - 1);

            Vector3 basePos = CalculateBezierPoint(t, startPos, curvePoint, currentTip);

            float sineWave = Mathf.Sin(t * waveFrequency - (Time.time * wiggleSpeed)) * waveAmplitude;

            float noise = (Mathf.PerlinNoise(t * noiseScale, Time.time * wiggleSpeed * 0.5f + randomSeed) - 0.5f) * noiseAmount;

            float mask = Mathf.Sin(t * Mathf.PI);

            Vector3 offset = sideVector * (sineWave + noise) * mask * currentProgress;

            lineRenderer.SetPosition(i, basePos + offset);
        }
    }


    Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        return (u * u * p0) + (2 * u * t * p1) + (t * t * p2);
    }
}