using UnityEngine;

public class MenuCreator : MonoBehaviour
{
    public Organ organ;
    public Transform MenuOrigin;
    public GameObject MenuPrefab;

    public GameObject MenuInstance;

    public string Headline;
    [TextArea(2, 3)]public string Description;
    void Start()
    {
      Invoke("CreateMenu", 0.01f);
    }

    public void CreateMenu()
    {
        GameObject menu = Instantiate(MenuPrefab, MenuOrigin.gameObject.GetComponent<RectTransform>());
        MenuInstance = menu;

        UIMenuInfo menuInfo = menu.GetComponent<UIMenuInfo>();
        menuInfo.organ = organ;
        menuInfo.SetMenuFields(Headline, Description);
        transform.parent.gameObject.GetComponent<UpgradeManager>().AddOrgan(menuInfo);

        MenuInstance.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
