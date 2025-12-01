using System.Collections.Generic;
using UnityEngine;

public class TentacleControllerIK : MonoBehaviour
{
    [Header("IK Setup")]
    public TargetJoint2D effectorJoint;
    public Rigidbody2D effectorBody;
    public Rigidbody2D rootBody;

    private Vector3 targetPosition;

    [Header("Visual Tip")]
    public Transform tentacleTip;

    [Header("Stretching Settings")]
    public bool enableStretching = true;
    public float totalUnstretchedLength = 9.5f;
    public Transform rootBone;
    public Transform[] bonesToStretch;

    [Tooltip("Öka denna (t.ex. 10-15) för snabbare respons på stretchen")]
    public float stretchSmoothSpeed = 10f;

    private List<Vector3> initialBoneScales = new List<Vector3>();
    private float currentStretchFactor = 1.0f;

    [Header("IK Dead Zone & Response")]
    public float targetDeadZone = 0.5f;

    [Header("Target Joint Dynamics")]
    public float maxJointFrequency = 80f;
    public float minJointFrequency = 0.5f;

    [Tooltip("Hur nära målet den måste vara för att slappna av")]
    public float relaxationDistance = 2.0f;

    [Tooltip("Extra kraft vid stretching för att motverka hävstångseffekten")]
    public float stretchedForceMultiplier = 1.5f;

    void Start()
    {
        if (bonesToStretch != null)
        {
            foreach (Transform bone in bonesToStretch)
            {
                initialBoneScales.Add(bone.localScale);
            }
        }
        else
        {
            Debug.LogError("BonesToStretch är inte tilldelad i inspectorn!");
        }
    }

    void LateUpdate()
    {
        bool newClick = false;

        // Input hantering
        if (Input.GetMouseButton(0))
        {
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = 0;
            newClick = true;
        }

        // Target Joint Logik
        if (effectorJoint != null)
        {
            float distanceToNewTarget = Vector3.Distance(effectorJoint.target, targetPosition);

            // Uppdatera målet om vi rört musen tillräckligt eller klickat
            if (distanceToNewTarget > targetDeadZone || newClick)
            {
                effectorJoint.target = targetPosition;
            }
        }

        // Hantera Stretch FÖRST, så fysiken vet längden innan vi applicerar krafter
        HandleStretching();

        // Applicera krafter/tension baserat på nya längden
        AdjustTargetJointTension();

        // Flytta debug-transform (själva script-objektet)
        transform.position = targetPosition;

        // Rotera tippen
        if (tentacleTip != null && effectorBody != null)
        {
            Vector3 directionToTarget = targetPosition - (Vector3)effectorBody.position;
            float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
            tentacleTip.rotation = Quaternion.Euler(0f, 0f, angle - -90f);
        }
    }

    private void AdjustTargetJointTension()
    {
        if (effectorJoint == null || effectorBody == null)
            return;

        float distanceToTarget = Vector3.Distance(effectorBody.position, effectorJoint.target);

        // Normalisera spänningen. Clamp01 gör att den går från 0 till 1.
        float tensionFactor = Mathf.Clamp01(distanceToTarget / relaxationDistance);

        // Beräkna frekvensen
        float targetFreq = Mathf.Lerp(minJointFrequency, maxJointFrequency, tensionFactor);

        // BOOST: Om vi är utsträckta, öka frekvensen/kraften för att orka dra den längre armen
        if (currentStretchFactor > 1.1f)
        {
            targetFreq *= stretchedForceMultiplier;
        }

        effectorJoint.frequency = targetFreq;
    }

    void HandleStretching()
    {
        if (
            rootBone == null
            || bonesToStretch == null
            || initialBoneScales.Count != bonesToStretch.Length
        )
            return;

        float targetStretch = 1.0f;

        if (enableStretching)
        {
            float distanceToTarget = Vector3.Distance(rootBone.position, targetPosition);

            if (distanceToTarget > totalUnstretchedLength)
            {
                targetStretch = distanceToTarget / totalUnstretchedLength;
            }
        }

        // Lerp för mjukhet, men snabbare respons
        currentStretchFactor = Mathf.Lerp(
            currentStretchFactor,
            targetStretch,
            Time.deltaTime * stretchSmoothSpeed
        );

        for (int i = 0; i < bonesToStretch.Length; i++)
        {
            Transform bone = bonesToStretch[i];
            Vector3 originalScale = initialBoneScales[i];

            // Applicera skalan
            bone.localScale = new Vector3(
                originalScale.x,
                originalScale.y * currentStretchFactor,
                originalScale.z
            );
        }
    }
}
