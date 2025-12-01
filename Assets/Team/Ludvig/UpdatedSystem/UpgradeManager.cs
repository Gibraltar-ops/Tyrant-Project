using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{

    private UIState State;
    private List<UIMenuInfo> UpgradeMenusInScene;
    private UIMenuInfo ActiveMenu;
    private GameObject[] OrganButtons;

    void OnEnable()
    {
        OrganButton.onButtonClick += ReceiveButtonEvent;
        UI_MenuCloseButton.onButtonClick += CloseMenu;
        
    }
     void OnDisable()
    {
        OrganButton.onButtonClick -= ReceiveButtonEvent;
        UI_MenuCloseButton.onButtonClick -= CloseMenu;
    }
    void Start()
    {   
        UpgradeMenusInScene = new List<UIMenuInfo>();
        State = UIState.Null;
        ActiveMenu = null;
        OrganButtons = GameObject.FindGameObjectsWithTag("UI_OrganButton");

    }
    public void AddOrgan(UIMenuInfo organ)
    {
        UpgradeMenusInScene.Add(organ);
    }

    private void ReceiveButtonEvent(Organ organ)
    {
        if(ActiveMenu == null)
        {

            foreach (UIMenuInfo menu in UpgradeMenusInScene)
            {
                if(menu.organ == organ)
                {
                    ActiveMenu = menu;
                }
            }

            ActiveMenu.gameObject.SetActive(true);
            foreach (var button in OrganButtons)
            {
                button.SetActive(false);
            }

            switch (organ)
            {
                case Organ.Maw:
                    State = UIState.Maw;
                    break;
                case Organ.Larynx:
                    State = UIState.Larynx;
                    break;
                case Organ.BrainCluster:
                    State = UIState.BrainCluster;
                    break;
                default: break;
            }
            return;
        }

        if(ActiveMenu?.organ == organ && State != UIState.Null)
        {
            ActiveMenu.gameObject.SetActive(false);
            ActiveMenu = null;
            State = UIState.Null;
            foreach (var button in OrganButtons)
            {
                button.SetActive(true);
            }
            return;
        }

         if(ActiveMenu?.organ == organ && State == UIState.Null)
        {
            ActiveMenu.gameObject.SetActive(false);
            State = UIState.Null;
             foreach (var button in OrganButtons)
            {
                button.SetActive(false);
            }
            return;
        }

        if (ActiveMenu?.organ != organ)
        {
            ActiveMenu.gameObject.SetActive(false);

            foreach (UIMenuInfo menu in UpgradeMenusInScene)
            {
                if(menu.organ == organ)
                {
                    ActiveMenu = menu;
                }
            }

            ActiveMenu.gameObject.SetActive(true);

            switch (organ)
            {
                case Organ.Maw:
                    State = UIState.Maw;
                    break;
                case Organ.Larynx:
                    State = UIState.Larynx;
                    break;
                case Organ.BrainCluster:
                    State = UIState.BrainCluster;
                    break;
                default: break;
            }
        }
    }

public void CloseMenu()
    {
        ActiveMenu.gameObject.SetActive(false);
            ActiveMenu = null;
            State = UIState.Null;
            foreach (var button in OrganButtons)
            {
                button.SetActive(true);
            }
            return;
    }

}


public enum UIState
{
    Null,
    Maw,
    Larynx,
    BrainCluster
}

