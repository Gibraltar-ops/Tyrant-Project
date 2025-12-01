using UnityEngine;

[RequireComponent(typeof(Transform))]
public class ParallaxLayerNew : MonoBehaviour
{
    [Range(0f, 1f)]
    public float parallaxStrength = 0.5f;

    private Vector3 initialPosition;

    public void CacheInitialPosition()
    {
        initialPosition = transform.localPosition;
    }

    public void ApplyParallax(Vector2 offset)
    {
        transform.localPosition = initialPosition + new Vector3(offset.x, offset.y, 0f) * parallaxStrength;
    }
}
