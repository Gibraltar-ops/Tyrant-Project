using UnityEngine;
using UnityEngine.UI;

public class ProgressBarAnimation : CollectorDrivenAnimation
{
    public RectTransform RectTransform;

    public override void UpdateAnimation()
    {
        RectTransform.localScale = new Vector2(1, collector.NormalizedTime);
    }
}
