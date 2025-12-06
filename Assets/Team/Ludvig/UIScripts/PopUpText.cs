using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpText : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private TMP_Text _text;
    void Start()
    {
        _text = GetComponent<TMP_Text>();
        
    }
    void OnEnable()
    {   
        transform.DOPunchScale(new Vector3(1.1f, 1.1f, 1.1f), 0.2f);
        Invoke("Fade", 0.5f);
    }
    private void Fade()
    {

       DOTween.To(()=> _text.color, x=> _text.color = x, new Color(_text.color.r, _text.color.g, _text.color.b, 0f), 1f).OnComplete(() => Disable());
    }

    private void Disable()
    {
        gameObject.SetActive(false);
    }
}
