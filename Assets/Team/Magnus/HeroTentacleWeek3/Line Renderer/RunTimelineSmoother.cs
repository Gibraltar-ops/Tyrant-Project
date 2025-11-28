using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RuntimeLineSmoother : MonoBehaviour
{
    public Transform[] chainTransforms;
    public float smoothingLength = 0.5f;
    public int smoothingSections = 10;

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

        if (curves == null || curves.Length != chainTransforms.Length - 1)
        {
            InitializeCurves();
        }

        CalculateCurves();

        PopulateLineRenderer();
    }

    private void InitializeCurves()
    {
        curves = new BezierCurve[chainTransforms.Length - 1];
        for (int i = 0; i < curves.Length; i++)
        {
            curves[i] = new BezierCurve();
        }
    }

    private void CalculateCurves()
    {
        for (int i = 0; i < curves.Length; i++)
        {
            Vector3 p0 = chainTransforms[i].position;
            Vector3 p3 = chainTransforms[i + 1].position;

            Vector3 p1,
                p2;

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

            if (i == curves.Length - 1)
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
    }

    private void PopulateLineRenderer()
    {
        positions.Clear();

        positions.AddRange(curves[0].GetSegments(smoothingSections));

        for (int i = 1; i < curves.Length; i++)
        {
            Vector3[] segments = curves[i].GetSegments(smoothingSections);
            for (int j = 1; j < segments.Length; j++)
            {
                positions.Add(segments[j]);
            }
        }

        // Lägg till den absolut sista punkten
        positions.Add(curves[curves.Length - 1].EndPosition);

        lineRenderer.positionCount = positions.Count;
        lineRenderer.SetPositions(positions.ToArray());
    }
}
