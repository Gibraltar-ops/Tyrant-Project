using System.Collections.Generic;
using UnityEngine;

public class CrabLegIK : MonoBehaviour
{
    [Header("Setup")]
    public Transform target;
    public int chainLength = 5; // Kom ihåg att du ökade denna till 5
    public int iterations = 10;
    public float delta = 0.001f;

    [Header("Constraints")]
    public bool useConstraints = true;
    public List<BoneConstraint> boneConstraints = new List<BoneConstraint>();

    [System.Serializable]
    public struct BoneConstraint
    {
        [Range(-180f, 180f)]
        public float minAngle;

        [Range(-180f, 180f)]
        public float maxAngle;
    }

    private Transform[] bones;
    private float[] boneLengths;
    private float completeLength;
    private Vector3[] positions;
    private Transform root;

    void Awake()
    {
        Init();
    }

    void Init()
    {
        bones = new Transform[chainLength + 1];
        positions = new Vector3[chainLength + 1];
        boneLengths = new float[chainLength];

        Transform current = transform;
        completeLength = 0;

        for (int i = bones.Length - 1; i >= 0; i--)
        {
            bones[i] = current;
            if (i < bones.Length - 1)
            {
                boneLengths[i] = (bones[i + 1].position - current.position).magnitude;
                completeLength += boneLengths[i];
            }

            if (i < chainLength && boneConstraints.Count <= i)
            {
                boneConstraints.Add(new BoneConstraint { minAngle = -45f, maxAngle = 45f });
            }

            if (current.parent != null)
                current = current.parent;
        }
        root = bones[0];

        if (boneConstraints.Count == 0)
        {
            for (int k = 0; k < chainLength; k++)
                boneConstraints.Add(new BoneConstraint { minAngle = -45f, maxAngle = 45f });
        }
    }

    void LateUpdate()
    {
        ResolveIK();
    }

    void ResolveIK()
    {
        if (target == null)
            return;
        if (bones == null || bones.Length != chainLength + 1)
            Init();

        for (int i = 0; i < bones.Length; i++)
            positions[i] = bones[i].position;

        Vector3 rootPos = bones[0].position;

        for (int iter = 0; iter < iterations; iter++)
        {
            for (int i = positions.Length - 1; i > 0; i--)
            {
                if (i == positions.Length - 1)
                    positions[i] = target.position;
                else
                {
                    Vector3 dir = (positions[i + 1] - positions[i]).normalized;
                    positions[i] = positions[i + 1] - dir * boneLengths[i];
                }
            }

            for (int i = 1; i < positions.Length; i++)
            {
                if (i == 1)
                    positions[i - 1] = rootPos;

                Vector3 direction = (positions[i] - positions[i - 1]).normalized;

                if (useConstraints && (i - 1) < boneConstraints.Count)
                {
                    Vector3 parentDir;
                    if (i == 1)
                    {
                        parentDir = (bones[1].position - bones[0].position).normalized;
                    }
                    else
                    {
                        parentDir = (positions[i - 1] - positions[i - 2]).normalized;
                    }

                    // Beräkna vinkel med tecken (Positiv = Vänster/Upp, Negativ = Höger/Ner i 2D)
                    float angle = Vector3.SignedAngle(parentDir, direction, Vector3.forward);

                    // Kläm vinkeln mellan dina inställda gränser
                    float clampedAngle = Mathf.Clamp(
                        angle,
                        boneConstraints[i - 1].minAngle,
                        boneConstraints[i - 1].maxAngle
                    );

                    // Om vi slog i taket/golvet på vinkeln, rotera vektorn
                    if (Mathf.Abs(angle - clampedAngle) > 0.01f)
                    {
                        Quaternion rot = Quaternion.AngleAxis(clampedAngle, Vector3.forward);
                        direction = rot * parentDir;
                    }
                }

                positions[i] = positions[i - 1] + direction * boneLengths[i - 1];
            }

            // Avbryt om nära nog
            if ((positions[positions.Length - 1] - target.position).sqrMagnitude < delta * delta)
                break;
        }

        // 3. Applicera på transforms
        for (int i = 0; i < bones.Length - 1; i++)
        {
            Vector3 dir = positions[i + 1] - positions[i];
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            // Beroende på din sprite-rotation kan du behöva justera offset här (t.ex. -90)
            bones[i].rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            bones[i].position = positions[i];
        }

        // (Valfritt) Rotera sista tippen mot target för "stöt"
        // bones[bones.Length-1].rotation = Quaternion.LookRotation(Vector3.forward, target.position - bones[bones.Length-1].position);
    }

    void OnDrawGizmos()
    {
        if (positions == null)
            return;
        Gizmos.color = Color.green;
        for (int i = 0; i < positions.Length - 1; i++)
            if (positions[i] != null)
                Gizmos.DrawLine(positions[i], positions[i + 1]);
    }
}
