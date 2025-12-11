using UnityEngine;

public enum GrabbableType
{
    Victim,
}

public interface IGrabbable
{
    Vector2 Position { get; }
    bool IsGrabbed { get; set; }
    GrabbableType Type { get; }
}
