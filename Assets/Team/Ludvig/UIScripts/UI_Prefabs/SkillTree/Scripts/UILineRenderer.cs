using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UILineRenderer : Graphic
{
    public float thickness = 10f;
    public float minimumThickness = 2f;
    public int segments = 20;
    public float animationSpeed = 1f;

    private float time;
    private List<UILineRendererLine> lines = new List<UILineRendererLine>();

    // Procedural Gaussian curve

    private AnimationCurve thicknessCurve;
    public AnimationCurve DefaultCurve;

    void Awake()
    {
        thicknessCurve = GenerateGaussianCurve(0.5f, 0.15f, 32);
    }

    void Update()
    {
        time += Time.deltaTime * animationSpeed;
        SetVerticesDirty();
    }

    public void AddLine(UILineRendererLine line)
    {
        lines.Add(line);
        SetVerticesDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        foreach (var line in lines)
        {
            DrawAnimatedLine(vh, line);
        }
    }

    private void DrawAnimatedLine(VertexHelper vh, UILineRendererLine line)
    {
        int startIndex = vh.currentVertCount;

        Vector2 p1 = line.start.anchoredPosition;
        Vector2 p2 = line.end.anchoredPosition;
        Vector2 dir = (p2 - p1).normalized;
        Vector2 perp = new Vector2(-dir.y, dir.x);

        for (int i = 0; i <= segments; i++)
        {
            float t = i / (float)segments;
            Vector2 pos = Vector2.Lerp(p1, p2, t);


            float curveValue;

            // Gaussian bulge traveling along line
            if (line.animated)
            {
                curveValue = thicknessCurve.Evaluate(Mathf.Repeat(t + time, 1f));
            }
            else
            {
                curveValue = DefaultCurve.Evaluate(t);
            }

            float currentThickness = thickness * curveValue + minimumThickness;

            Vector2 offset = perp * (currentThickness / 2f);
            Vector2 left = pos - offset;
            Vector2 right = pos + offset;

            UIVertex vLeft = UIVertex.simpleVert; vLeft.color = line.color; vLeft.position = left;
            UIVertex vRight = UIVertex.simpleVert; vRight.color = line.color; vRight.position = right;

            vh.AddVert(vLeft);
            vh.AddVert(vRight);

            if (i > 0)
            {
                int baseIndex = startIndex + i * 2;
                vh.AddTriangle(baseIndex - 2, baseIndex - 1, baseIndex);
                vh.AddTriangle(baseIndex, baseIndex - 1, baseIndex + 1);
            }
        }
    }

    // Generate a Gaussian AnimationCurve
    private AnimationCurve GenerateGaussianCurve(float mean, float sigma, int resolution)
    {
        Keyframe[] keys = new Keyframe[resolution];
        for (int i = 0; i < resolution; i++)
        {
            float t = i / (float)(resolution - 1); // 0 → 1
            float value = Mathf.Exp(-Mathf.Pow(t - mean, 2) / (2 * sigma * sigma));
            keys[i] = new Keyframe(t, value);
        }
        return new AnimationCurve(keys);
    }
}

public class UILineRendererLine
{
    public bool animated = false;
    public RectTransform start;
    public RectTransform end;
    public Color color;

    public UILineRendererLine(RectTransform startPoint, RectTransform endPoint, Color lineColor, bool isAnimated)
    {
        animated = isAnimated;
        start = startPoint;
        end = endPoint;
        color = lineColor;
    }
}
