using UnityEngine;

public class ReplaceShader : MonoBehaviour
{
    public Shader overrideShader;
    public RenderTexture output;
    void Update()
    {
        // Check if there is a current camera rendering
        if (Camera.main != null)
        {
            // Assign the output render texture to the current camera
            Camera.main.targetTexture = output;
            // 
            // Render the scene by replacing all the "RenderType" shaders with the overrideShader
            Camera.main.RenderWithShader(overrideShader, "RenderType");
            // 
            // Set the output render texture back to null to avoid side effects.
            Camera.main.targetTexture = null;
        }
        else
        {
            Debug.LogWarning("No current camera available for rendering.");
        }
    }
}
