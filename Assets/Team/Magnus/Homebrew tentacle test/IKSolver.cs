using UnityEngine;

public static class IKSolver
{
    public static void Solve(Vector3[] segmentPositions, float[] segmentLengths, Vector3 targetPosition, int iterations = 10, float tolerance = 0.01f)
    {
        int segments = segmentLengths.Length;
        Vector3 basePosition = segmentPositions[0];
        float distanceToTarget = Vector3.Distance(basePosition, targetPosition);

        float totalLength = 0;
        foreach (var length in segmentLengths)
        {
            totalLength += length;
        }

        float stretchFactor = Mathf.Max(1.0f, distanceToTarget / totalLength);

     
        float[] currentSegmentLengths = new float[segments];
        for (int i = 0; i < segments; i++)
        {
            currentSegmentLengths[i] = segmentLengths[i] * stretchFactor;
        }

      
        Vector3 endEffector = segmentPositions[segments];
        for (int iter = 0; iter < iterations && Vector3.Distance(endEffector, targetPosition) > tolerance; iter++)
        {
          
            segmentPositions[segments] = targetPosition;
            for (int i = segments - 1; i >= 0; i--)
            {
                Vector3 direction = (segmentPositions[i + 1] - segmentPositions[i]).normalized;
                segmentPositions[i] = segmentPositions[i + 1] - direction * currentSegmentLengths[i];
            }

          
            segmentPositions[0] = basePosition;
            for (int i = 1; i <= segments; i++)
            {
                Vector3 direction = (segmentPositions[i] - segmentPositions[i - 1]).normalized;
                segmentPositions[i] = segmentPositions[i - 1] + direction * currentSegmentLengths[i - 1];
            }
            endEffector = segmentPositions[segments];
        }
    }
}