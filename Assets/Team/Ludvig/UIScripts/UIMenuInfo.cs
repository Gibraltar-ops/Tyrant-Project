
using TMPro;
using UnityEngine;

public class UIMenuInfo : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Organ organ;
    public string Headline;
    public string Description;
    public TMP_Text HeadlineLabel;
    public TMP_Text DescriptionLabel;

    [Header("Object References")]
    public GameObject ShopRoot;
    void Start()
    {
        
    }

    public void SetMenuFields(string headline, string description)
    {
        Headline = headline;
        Description = description;
        UpdateFields();
    }


    public void UpdateFields()
    {
        HeadlineLabel.text = Headline;
        DescriptionLabel.text = Description;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
