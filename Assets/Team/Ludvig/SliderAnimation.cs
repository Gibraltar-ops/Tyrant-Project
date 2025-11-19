using UnityEngine;
using UnityEngine.UI;

public class SliderAnimation : CollectorDrivenAnimation
{
    public Slider Slider;

    public override void UpdateAnimation()
    {
        base.UpdateAnimation();
        Slider.normalizedValue = collector.NormalizedTime;
    }
}
