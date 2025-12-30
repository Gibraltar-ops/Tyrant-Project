using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpText : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TMP_Text _text;
    public Image _image;

    void Start()
    {
        // _text = GetComponentInChildren<TMP_Text>(true);
        // _image = GetComponentInChildren<Image>(true);
    }
    void OnEnable()
    {
        //transform.DOPunchScale(new Vector3(1.1f, 1.1f, 1.1f), 0.2f);
        Invoke("Move", 0.1f);
        Invoke("Fade", 0.5f);
    }

    private void Move()
    {
        transform.DOLocalMoveY(GetComponent<RectTransform>().anchoredPosition.y + Random.Range(0f, 100f), 0.4f);
    }
    private void Fade()
    {

        DOTween.To(() => _text.color, x => _text.color = x, new Color(_text.color.r, _text.color.g, _text.color.b, 0f), 1f).OnComplete(() => Disable());
        DOTween.To(() => _image.color, x => _image.color = x, new Color(_image.color.r, _image.color.g, _image.color.b, 0f), 1f);
    }

    private void Disable()
    {
        gameObject.SetActive(false);
    }
}
