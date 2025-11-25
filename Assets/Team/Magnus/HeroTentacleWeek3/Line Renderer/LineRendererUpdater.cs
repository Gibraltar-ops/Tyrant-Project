using UnityEngine;

public class LineRendererUpdater : MonoBehaviour
{
    public Transform[] chainTransforms;
    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null || chainTransforms == null || chainTransforms.Length == 0)
        {
            Debug.LogError("LineRenderer missing!");
            enabled = false;
            return;
        }

        lineRenderer.positionCount = chainTransforms.Length;
    }

    void LateUpdate()
    {
        if (chainTransforms == null || lineRenderer == null)
            return;

        for (int i = 0; i < chainTransforms.Length; i++)
        {
            if (chainTransforms[i] != null)
            {
                lineRenderer.SetPosition(i, chainTransforms[i].position);
            }
        }
    }
}
