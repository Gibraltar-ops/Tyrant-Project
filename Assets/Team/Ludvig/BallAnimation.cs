using UnityEngine;
using UnityEngine.UI;

public class BallAnimation : CollectorDrivenAnimation
{
    public AnimationCurve curveY;
    public float Multiplier = 1f;

    public override void UpdateAnimation()
    {
        float posY = curveY.Evaluate(collector.NormalizedTime) * Multiplier;
        transform.position = new Vector2(transform.position.x, posY);
    }
}
