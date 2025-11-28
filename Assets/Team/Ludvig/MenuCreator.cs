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
      CreateMenu();
    }

    public void CreateMenu()
    {
        GameObject menu = Instantiate(MenuPrefab, MenuOrigin);
        MenuInstance = menu;

        UIMenuInfo menuInfo = menu.GetComponent<UIMenuInfo>();
        menuInfo.organ = organ;
        menuInfo.SetMenuFields(Headline, Description);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
