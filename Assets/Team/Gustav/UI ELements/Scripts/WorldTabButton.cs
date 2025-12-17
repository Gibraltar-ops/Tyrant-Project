using UnityEngine;

public class WorldTabButton : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private UIDrawer drawer;
    [SerializeField] private TabMenu tabMenu;

    [Header("Target Page")]
    [SerializeField] private int pageIndex;

    public void Activate()
    {
        // Drawer closed -> open + jump
        if (!drawer.IsOpen)
        {
            drawer.Open();
            tabMenu.JumpToPage(pageIndex);
            return;
        }

        // Drawer open AND this page is already active -> close
        if (tabMenu.CurrentPageIndex == pageIndex)
        {
            drawer.Close();
            return;
        }

        // Drawer open but different page -> switch page
        tabMenu.JumpToPage(pageIndex);
    }
}

