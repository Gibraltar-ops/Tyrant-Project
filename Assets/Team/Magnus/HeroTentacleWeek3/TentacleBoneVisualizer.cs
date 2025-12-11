using System.Collections.Generic;
using UnityEngine;

public class TentacleBoneVisualizer : MonoBehaviour
{
    public Transform[] bones;
    public Color gizmoColor = Color.green;

    private void OnDrawGizmos()
    {
        if (bones == null || bones.Length < 2)
            return;

        Gizmos.color = gizmoColor;

        for (int i = 0; i < bones.Length - 1; i++)
        {
            if (bones[i] != null && bones[i + 1] != null)
            {
                Gizmos.DrawLine(bones[i].position, bones[i + 1].position);

                Gizmos.DrawSphere(bones[i].position, 0.1f);
            }
        }

        if (bones.Length > 0 && bones[bones.Length - 1] != null)
        {
            Gizmos.DrawSphere(bones[bones.Length - 1].position, 0.1f);
        }
    }
}
