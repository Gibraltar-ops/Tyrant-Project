using UnityEngine;

public class MenuGroup : MonoBehaviour
{
    public Organ organ;
    public Transform MenuOrigin;
    public GameObject MenuPrefab;
    void Start()
    {
      CreateMenu();
    }

    public void CreateMenu()
    {
        GameObject menu = Instantiate(MenuPrefab, MenuOrigin);
        foreach (Transform child in transform)
        {
            Shop shop = child.gameObject.GetComponent<Shop>();
            shop.ParentMenu = menu;
            shop.CreateStoreItem();
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
