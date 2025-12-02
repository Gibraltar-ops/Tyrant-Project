using UnityEngine;

public enum GrabbableType
{
    Victim,
    Fighter,
}

public interface IGrabbable
{
    Vector2 Position { get; }
    bool IsGrabbed { get; set; }
    GrabbableType Type { get; }
}
