
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupTextHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject TextPrefab;
    public int TextPoolSize;
    private List<GameObject> _textPool;

    void OnEnable()
    {
        EconomyManager.onTextPopup += PopUp;
    }
    void OnDisable()
    {
        EconomyManager.onTextPopup -= PopUp;
    }
    void Start()
    {
        _textPool = new List<GameObject>();
        GameObject instance = TextPrefab;
        instance.SetActive(false);

        for (int i = 0; i < TextPoolSize; i++)
        {
            var item = Instantiate(instance, transform);
            //item.SetActive(false);
            _textPool.Add(item);
        }


    }


    public void PopUp(PopupTextData data)
    {

        foreach (GameObject label in _textPool)
        {
            if (!label.activeSelf)
            {
                label.SetActive(true);
                TMP_Text text = label.GetComponent<PopUpText>()._text;
                Image image = label.GetComponent<PopUpText>()._image;
                CurrencySpriteData spriteData = EconomyManager.Instance.GetCurrencySpriteData(data.Currency);

                text.text = data.Text;
                text.color = spriteData.OverlayColor;
                image.sprite = spriteData.Sprite;
                image.color = spriteData.OverlayColor;
                label.transform.position = Camera.main.WorldToScreenPoint(data.Position);
                return;
            }
        }
    }
}

public struct PopupTextData
{
    public Vector2 Position;
    public string Text;
    public Color Color;
    public Currency Currency;


    public PopupTextData(Vector2 position, string text, Color color, Currency currency)
    {
        Position = position;
        Text = text;
        Color = color;
        Currency = currency;
    }
}
