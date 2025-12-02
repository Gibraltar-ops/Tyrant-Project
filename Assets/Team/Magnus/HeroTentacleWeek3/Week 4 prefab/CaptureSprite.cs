using UnityEngine;

public class CaptureSprite : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("MouthZone"))
        {
            gameObject.SetActive(false);
        }
    }
}
