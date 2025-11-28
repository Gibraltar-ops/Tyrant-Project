using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{

    private UIState State;
    private List<UIMenuInfo> UpgradeMenusInScene;
    private UIMenuInfo ActiveMenu;

    void OnEnable()
    {
        OrganButton.onButtonClick += ReceiveButtonEvent;
    }
     void OnDisable()
    {
        OrganButton.onButtonClick -= ReceiveButtonEvent;
    }
    void Start()
    {   
        UpgradeMenusInScene = new List<UIMenuInfo>();
        State = UIState.Null;
        ActiveMenu = null;
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
            return;
        }

         if(ActiveMenu?.organ == organ && State == UIState.Null)
        {
            ActiveMenu.gameObject.SetActive(false);
            State = UIState.Null;
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

}


public enum UIState
{
    Null,
    Maw,
    Larynx,
    BrainCluster
}

