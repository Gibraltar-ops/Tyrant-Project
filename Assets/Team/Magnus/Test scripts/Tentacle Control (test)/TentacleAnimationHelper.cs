using UnityEngine;

public class TentacleAnimationHelper : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private TentacleGrabber grabber;

    void Awake()
    {
        grabber = GetComponentInParent<TentacleGrabber>();
    }

    public void CallOnGrabAnimationFinished()
    {
        if (grabber != null)
        {
            grabber.ResetGrab();
        }

    }
}
