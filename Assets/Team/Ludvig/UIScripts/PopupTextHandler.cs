using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
                label.GetComponent<TMP_Text>().text = data.Text;
                label.GetComponent<TMP_Text>().color = data.Color;
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

    public PopupTextData(Vector2 position, string text, Color color)
    {
        Position = position;
        Text = text;
        Color = color;
    }
}
