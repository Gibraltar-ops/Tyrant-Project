using UnityEngine;

public class TentacleSpriteStretch : MonoBehaviour
{
    [Header("Bones (visual chain)")]
    public Transform rootBone;    
    public Transform[] bones;      

    [Header("Target")]
    public Transform target;      

    [Header("IK settings")]
    public int iterations = 10;
    public float tolerance = 0.01f;

    private Vector3[] segmentPositions;
    private float[] segmentLengths;
    private float totalLength;

    private void Awake()
    {
        if (rootBone == null || bones == null || bones.Length == 0)
        {
            Debug.LogError($"{name}: TentacleSpriteStretch saknar rootBone eller bones!");
            enabled = false;
            return;
        }

        int segments = bones.Length;
        segmentPositions = new Vector3[segments + 1];
        segmentLengths = new float[segments];

        
        segmentPositions[0] = rootBone.position;
        totalLength = 0f;

        for (int i = 0; i < segments; i++)
        {
            segmentPositions[i + 1] = bones[i].position;
            float dist = Vector3.Distance(segmentPositions[i], segmentPositions[i + 1]);
            segmentLengths[i] = dist;
            totalLength += dist;
        }
    }

    private void LateUpdate()
    {
        if (target == null || rootBone == null) return;

        int segments = bones.Length;

     
        segmentPositions[0] = rootBone.position;
        for (int i = 1; i <= segments; i++)
        {
            segmentPositions[i] = segmentPositions[i - 1] + Vector3.down * segmentLengths[i - 1];
        }

  
        IKSolver.Solve(segmentPositions, segmentLengths, target.position, iterations, tolerance);

    
        for (int i = 0; i < segments; i++)
        {
            Vector3 p0 = segmentPositions[i];
            Vector3 p1 = segmentPositions[i + 1];
            Vector3 dir = p1 - p0;

            bones[i].position = p0;
            

            if (dir.sqrMagnitude > 0.0001f)
            {
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

              
                bones[i].rotation = Quaternion.AngleAxis(angle , Vector3.forward);
            }
        }
    }
}
