using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RuntimeLineSmoother : MonoBehaviour
{
    public Transform[] chainTransforms;
    public float smoothingLength = 0.5f;
    public int smoothingSections = 10;

    public Vector3? overrideEndPosition = null;

    private LineRenderer lineRenderer;
    private List<Vector3> positions = new List<Vector3>();
    private BezierCurve[] curves;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void LateUpdate()
    {
        if (chainTransforms == null || chainTransforms.Length < 2)
        {
            lineRenderer.positionCount = 0;
            return;
        }

        int requiredCurves = chainTransforms.Length - 1;
        if (overrideEndPosition.HasValue)
        {
            requiredCurves = chainTransforms.Length; //Extra stretch for curve
        }

        if (curves == null || curves.Length != requiredCurves)
        {
            InitializeCurves(requiredCurves);
        }

        CalculateCurves();
        PopulateLineRenderer();
    }

    private void InitializeCurves(int count)
    {
        curves = new BezierCurve[count];
        for (int i = 0; i < curves.Length; i++)
        {
            curves[i] = new BezierCurve();
        }
    }

    private void CalculateCurves()
    {
        // Loop physical legs first
        int physicalSegments = chainTransforms.Length - 1;

        for (int i = 0; i < physicalSegments; i++)
        {
            Vector3 p0 = chainTransforms[i].position;
            Vector3 p3 = chainTransforms[i + 1].position;

            Vector3 p1,
                p2;

            // calculate tangent p1
            if (i == 0)
            {
                Vector3 tangent = (p3 - p0).normalized;
                p1 = p0 + tangent * smoothingLength;
            }
            else
            {
                Vector3 prevPoint = chainTransforms[i - 1].position;
                Vector3 tangent = (p3 - prevPoint).normalized;
                p1 = p0 + tangent * smoothingLength;
            }

            // Calculate tanget p2
            // last physical leg check, calculate override
            if (i == physicalSegments - 1 && overrideEndPosition.HasValue)
            {
                // point tangent towards click pos
                Vector3 nextPoint = overrideEndPosition.Value;
                Vector3 tangent = (p0 - nextPoint).normalized;
                p2 = p3 + tangent * smoothingLength;
            }
            else if (i == physicalSegments - 1)
            {
                Vector3 tangent = (p0 - p3).normalized;
                p2 = p3 + tangent * smoothingLength;
            }
            else
            {
                Vector3 nextPoint = chainTransforms[i + 2].position;
                Vector3 tangent = (p0 - nextPoint).normalized;
                p2 = p3 + tangent * smoothingLength;
            }

            curves[i].Points[0] = p0;
            curves[i].Points[1] = p1;
            curves[i].Points[2] = p2;
            curves[i].Points[3] = p3;
        }

        if (overrideEndPosition.HasValue)
        {
            int lastIndex = curves.Length - 1;
            Vector3 p0 = chainTransforms[chainTransforms.Length - 1].position;
            Vector3 p3 = overrideEndPosition.Value;

            // Tangent out from last bone
            Vector3 prevPoint = chainTransforms[chainTransforms.Length - 2].position;
            Vector3 tangentP1 = (p3 - prevPoint).normalized; //point it forward
            Vector3 p1 = p0 + tangentP1 * smoothingLength;

            // Tangent towards target
            Vector3 tangentP2 = (p0 - p3).normalized;
            Vector3 p2 = p3 + tangentP2 * smoothingLength;

            curves[lastIndex].Points[0] = p0;
            curves[lastIndex].Points[1] = p1;
            curves[lastIndex].Points[2] = p2;
            curves[lastIndex].Points[3] = p3;
        }
    }

    private void PopulateLineRenderer()
    {
        positions.Clear();

        if (curves.Length > 0)
            positions.Add(curves[0].StartPosition);

        for (int i = 0; i < curves.Length; i++)
        {
            Vector3[] segments = curves[i].GetSegments(smoothingSections);
            for (int j = 1; j < segments.Length; j++)
            {
                positions.Add(segments[j]);
            }
        }

        lineRenderer.positionCount = positions.Count;
        lineRenderer.SetPositions(positions.ToArray());
    }
}
