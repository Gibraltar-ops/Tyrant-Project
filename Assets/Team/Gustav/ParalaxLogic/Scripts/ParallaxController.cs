using UnityEngine;



public class ParallaxController : MonoBehaviour

{

    [Header("References")]

    public Transform cameraRig;

    public ParallaxLayerNew[] layers;



    [Header("Settings")]

    public float maxOffsetX = 1f;

    public float maxOffsetY = 0.5f;

    public float smoothSpeed = 5f;



    private Vector2 targetOffset;



    void Start()

    {

        foreach (var layer in layers)

            layer.CacheInitialPosition();

    }



    void Update()

    {

        UpdateOffsetBasedOnMouse();

        ApplyParallax();

    }



    void UpdateOffsetBasedOnMouse()

    {

        // Normalize mouse position (0,0 bottom-left -> 1,1 top-right)

        Vector2 mouse = Input.mousePosition;

        Vector2 normalizedMouse = new Vector2(

            Mathf.Clamp01(mouse.x / Screen.width),

            Mathf.Clamp01(mouse.y / Screen.height)

        );



        // Convert to range (-1 to 1)

        Vector2 centered = (normalizedMouse - new Vector2(0.5f, 0.5f)) * 2f;



        // move along mouse direction

        targetOffset = new Vector2(
            centered.x * maxOffsetX,

            centered.y * maxOffsetY
        );

    }
    void ApplyParallax()

    {

        foreach (var layer in layers)

            layer.ApplyParallax(targetOffset);

        // OPTIONAL: Drift Camera Rig slightly

        if (cameraRig)

        {

            Vector3 desired = new Vector3(targetOffset.x, targetOffset.y, cameraRig.localPosition.z);

            cameraRig.localPosition = Vector3.Lerp(

                cameraRig.localPosition,

                desired,

                Time.deltaTime * smoothSpeed

            );

        }

    }

}