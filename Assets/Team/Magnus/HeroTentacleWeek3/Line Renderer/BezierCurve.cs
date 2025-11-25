using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurve
{
    public Vector3[] Points;

    public BezierCurve()
    {
        Points = new Vector3[4];
    }

    public BezierCurve(Vector3[] Points)
    {
        this.Points = Points;
    }

    public Vector3 StartPosition
    {
        get { return Points[0]; }
    }

    public Vector3 EndPosition
    {
        get { return Points[3]; }
    }

    // KORREKT KOD
    public Vector3 GetSegment(float Time)
    {
        Time = Mathf.Clamp01(Time);
        float oneMinusTime = 1f - Time;

        return (oneMinusTime * oneMinusTime * oneMinusTime * Points[0])
            + (3f * oneMinusTime * oneMinusTime * Time * Points[1])
            + (3f * oneMinusTime * Time * Time * Points[2])
            + (Time * Time * Time * Points[3]);
    }

    public Vector3[] GetSegments(int Subdivisions)
    {
        Vector3[] segments = new Vector3[Subdivisions];

        float time;
        for (int i = 0; i < Subdivisions; i++)
        {
            time = (float)i / Subdivisions;
            segments[i] = GetSegment(time);
        }

        return segments;
    }
}
