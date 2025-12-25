using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.IK;

public class KebabMuscleMemory : MonoBehaviour
{
    [Header("References")]
    public Transform ikTarget;
    public Solver2D ikSolver;

    [Header("Muscles")]
    public List<Transform> allBones;

    [Header("Windup settings")]
    [Tooltip("How high we place the arm before attacking")]
    public float windUpHeight = 3.0f;

    [Tooltip("How much time does the windup take")]
    public float windUpTime = 0.2f;

    [Header("Mutlistrike settings")]
    public float multiStrikeRetractTime = 0.15f;
    public float multiStrikeHeightMultiplier = 1.2f;

    [Tooltip("Attack speed downwards")]
    public float attackSpeed = 0.12f;
    public float returnSpeed = 0.5f;

    public AnimationCurve thrustCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

    public float impactRotationOffset = 0f;

    [Header("Idle Settings")]
    public float idleSpeed = 1.0f;
    public float idleRadius = 0.5f;
    private float _noiseSeed;

    private List<Quaternion> _defaultRotations = new List<Quaternion>();
    private Vector3 _defaultTargetPosLocal;
    private bool _isAttacking = false;
    public bool IsAttacking => _isAttacking;

    void Start()
    {
        _defaultTargetPosLocal = ikTarget.localPosition;
        foreach (var bone in allBones)
            _defaultRotations.Add(bone.localRotation);

        _noiseSeed = UnityEngine.Random.Range(0f, 100f);
        SetIKWeight(1f);
    }

    void Update()
    {
        if (_isAttacking)
            return;

        float time = Time.time * idleSpeed + _noiseSeed;
        float noiseX = Mathf.PerlinNoise(time, 0f) - 0.5f;
        float noiseY = Mathf.PerlinNoise(0f, time) - 0.5f;

        Vector3 offset = new Vector3(noiseX, noiseY, 0f) * idleRadius;

        ikTarget.localPosition = _defaultTargetPosLocal + offset;

        SetIKWeight(1f);
    }

    public void Thrust(Vector3 targetPos, Action onHit = null)
    {
        Thrust(new List<Vector3> { targetPos }, (i) => onHit?.Invoke());
    }
    public void Thrust(List<Vector3> targetPos, Action<int> onHit = null)
    {
        if (_isAttacking)
            return;
        StartCoroutine(PerformKebabStrike(targetPos, onHit));
    }

    IEnumerator PerformKebabStrike(List<Vector3> targetPositions, Action<int> onHit)
    {
        _isAttacking = true;

        if (targetPositions == null || targetPositions.Count == 0)
        {
            _isAttacking = false;
            yield break;
        }

        Vector3 myBasePos = transform.position;

        // Setup for first target
        Vector3 firstTarget = targetPositions[0];
        Vector3 retractedPos = Vector3.Lerp(ikTarget.position, myBasePos, -1f);
        Vector3 windUpPos = retractedPos + (transform.up * windUpHeight);

        Vector3 localDir = transform.InverseTransformPoint(firstTarget);
        float angle = Mathf.Atan2(localDir.y, localDir.x) * Mathf.Rad2Deg;
        Quaternion attackRotation = Quaternion.Euler(0, 0, angle + impactRotationOffset);

        Transform tipBone = null;
        if (allBones.Count > 0)
            tipBone = allBones[allBones.Count - 1];
        Quaternion startTipRot = (tipBone != null) ? tipBone.rotation : Quaternion.identity;

        // 1. Initial Windup
        float timer = 0f;
        Vector3 startIkPos = ikTarget.position;

        while (timer < windUpTime)
        {
            timer += Time.deltaTime;
            float t = timer / windUpTime;
            float smoothT = t * t;

            SetIKWeight(Mathf.Lerp(1f, 1f, smoothT));

            // Move IK Target upwards and backwards
            ikTarget.position = Vector3.Lerp(startIkPos, windUpPos, smoothT);

            // Tip facing click pos
            if (tipBone != null)
            {
                tipBone.rotation = Quaternion.Slerp(
                    startTipRot,
                    transform.rotation * attackRotation,
                    smoothT
                );
            }
            yield return null;
        }

        // 2. Strike Loop
        for (int i = 0; i < targetPositions.Count; i++)
        {
            Vector3 currentTarget = targetPositions[i];

            // Recalculate rotation for current target
            localDir = transform.InverseTransformPoint(currentTarget);
            angle = Mathf.Atan2(localDir.y, localDir.x) * Mathf.Rad2Deg;
            attackRotation = Quaternion.Euler(0, 0, angle + impactRotationOffset);

            if (i > 0)
            {
                // Quick Retract for subsequent strikes
                timer = 0f;
                Vector3 startRetract = ikTarget.position;
                // Retract slightly up and towards base
                Vector3 nextWindUp = Vector3.Lerp(startRetract, myBasePos, 0.2f) + (transform.up * (windUpHeight * multiStrikeHeightMultiplier));

                while (timer < multiStrikeRetractTime)
                {
                    timer += Time.deltaTime;
                    float t = timer / multiStrikeRetractTime;
                    ikTarget.position = Vector3.Lerp(startRetract, nextWindUp, t);
                    yield return null;
                }
                windUpPos = nextWindUp;
            }

            // Strike
            timer = 0f;
            while (timer < attackSpeed)
            {
                timer += Time.deltaTime;
                float t = timer / attackSpeed;

                ikTarget.position = Vector3.Lerp(windUpPos, currentTarget, thrustCurve.Evaluate(t));

                if (tipBone != null)
                    tipBone.localRotation = attackRotation;

                yield return null;
            }

            onHit?.Invoke(i);

            yield return new WaitForSeconds(0.05f); // Impact pause
        }

        // 3. Return
        timer = 0f;
        Vector3 startReturnLocalPos = ikTarget.localPosition;
        while (timer < returnSpeed)
        {
            timer += Time.deltaTime;
            float t = timer / returnSpeed;

            SetIKWeight(1f);
            ikTarget.localPosition = Vector3.Lerp(startReturnLocalPos, _defaultTargetPosLocal, t);
            yield return null;
        }

        SetIKWeight(0f);
        ikTarget.localPosition = _defaultTargetPosLocal;
        for (int i = 0; i < allBones.Count; i++)
            allBones[i].localRotation = _defaultRotations[i];
        _isAttacking = false;
    }

    void SetIKWeight(float weight)
    {
        if (ikSolver != null)
            ikSolver.weight = weight;
    }
}

