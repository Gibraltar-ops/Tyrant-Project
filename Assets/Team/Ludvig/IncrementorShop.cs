using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IncrementorShop : MonoBehaviour
{

    private ValueHolder _valueHolder;
    public GameObject IncrementorPrefab;
    public Transform IncrementorParent;
    public bool CanAfford = false;
    public int nextPrice;

    public TMP_Text PriceTag;
    public TMP_Text CountTag;
    public Button UpgradeButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _valueHolder = FindAnyObjectByType<ValueHolder>();
        nextPrice = _valueHolder.GetPriceOfNextUpgrade();

        PriceTag.text = nextPrice.ToString();
        //CountTag.text = _valueHolder.IncrementorCount.ToString();
    }

    public void Purchase()
    {
        // if (_valueHolder.IncrementorCanAfford)
        // {
        //     SpawnIncrementor();
        //     _valueHolder.Currency1 -= _valueHolder.GetPriceOfNextUpgrade();
        //     _valueHolder.IncrementorCount++;

        //     if (_valueHolder.Currency1! > _valueHolder.GetPriceOfNextUpgrade())
        //     {
        //         UpgradeButton.interactable = false;
        //         _valueHolder.IncrementorCanAfford = false;
        //     }

        //     UpdateTextFields();
        // }

    }

    public void UpdateTextFields()
    {
        PriceTag.text = _valueHolder.GetPriceOfNextUpgrade().ToString();
        //CountTag.text = _valueHolder.IncrementorCount.ToString();
    }

    public void SpawnIncrementor()
    {
        Instantiate(IncrementorPrefab, IncrementorParent);
        //_valueHolder.IncrementorCount++;
    }
    // Update is called once per frame
    void Update()
    {

    }
}
