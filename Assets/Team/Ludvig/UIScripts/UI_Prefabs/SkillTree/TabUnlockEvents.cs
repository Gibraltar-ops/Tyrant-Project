using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TabUnlockEvents : MonoBehaviour
{
    public bool DebugMode = false;
    public GameObject BroodTab;
    public GameObject MawTab;
    public GameObject BrainTab;
    public GameObject SurvivalTab;
    void OnEnable()
    {
        ProgressEventTrigger.onProgressEvent += EventHandler;
    }
    void OnDisable()
    {
        ProgressEventTrigger.onProgressEvent -= EventHandler;
    }

    void Awake()
    {
        if (DebugMode)
        {
            SetVisible(BrainTab);
            SetVisible(SurvivalTab);
            SetVisible(BroodTab);
            SetVisible(MawTab);
            // BrainTab.SetActive(true);
            // SurvivalTab.SetActive(true);
            // BroodTab.SetActive(true);
            // MawTab.SetActive(true);
            return;
        }
        SetInvisible(BrainTab);
        SetInvisible(SurvivalTab);
        SetInvisible(BroodTab);
        SetVisible(MawTab);
        // BrainTab.SetActive(false);
        // SurvivalTab.SetActive(false);
        // BroodTab.SetActive(false);
        // MawTab.SetActive(true);

    }

    private void SetInvisible(GameObject tab)
    {
        tab.GetComponent<Toggle>().interactable = false;
        tab.GetComponent<Image>().enabled = false;
        tab.GetComponentInChildren<TMP_Text>(true).enabled = false;
    }

    private void SetVisible(GameObject tab)
    {
        tab.GetComponent<Toggle>().interactable = true;
        tab.GetComponent<Image>().enabled = true;
        tab.GetComponentInChildren<TMP_Text>(true).enabled = true;
    }
    private void EventHandler(ProgressEvent progressEvent)
    {
        switch (progressEvent)
        {
            case ProgressEvent.UnlockBrain:
                SetVisible(BrainTab);
                break;
            case ProgressEvent.UnlockBrood:
                SetVisible(BroodTab);
                break;
            case ProgressEvent.UnlockSurvival:
                SetVisible(SurvivalTab);
                break;
            default: break;
        }
    }

}
