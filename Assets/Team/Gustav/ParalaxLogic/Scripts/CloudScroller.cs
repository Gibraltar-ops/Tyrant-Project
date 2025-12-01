using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class CloudScroller : MonoBehaviour
{
    public float scrollSpeed = 0.02f;

    private Renderer rend;
    private Vector2 offset;

    void Start()
    {
        rend = GetComponent<Renderer>();
        offset = rend.material.GetTextureOffset("_MainTex");
    }

    void Update()
    {
        offset.x += scrollSpeed * Time.deltaTime;
        rend.material.SetTextureOffset("_MainTex", offset);
    }
}
