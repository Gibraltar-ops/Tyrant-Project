using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class IconPulse : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.5f).SetLoops(-1, LoopType.Yoyo);
        transform.DOLocalRotate(new Vector3(0f, 0f, 60f), 0.5f).SetLoops(-1, LoopType.Incremental);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
