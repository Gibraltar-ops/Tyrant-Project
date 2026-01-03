using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Video;
using UnityEngine.EventSystems;


public class ProgressManager : MonoBehaviour
{
    private bool isChangingStage = false;

    [System.Serializable]
    public class Stage
    {
        public string stageName;

        public GameObject playerTyrant;
        public GameObject parallaxBackground;
        public GameObject postProcessing;
        public GameObject resistanceManager;
    }


    [Header("Video Transitions")]
    [SerializeField] private VideoClip stage1To2Video;
    [SerializeField] private VideoClip stage2To3Video;


    [Header("Stages")]
    [SerializeField] private List<Stage> stages = new List<Stage>();

    [Header("Kebabarmar")]
    [SerializeField] private GameObject kebabArmStage1A;
    [SerializeField] private GameObject kebabArmStage1B;
    [SerializeField] private GameObject kebabArmStage2A;
    [SerializeField] private GameObject kebabArmStage2B;


    [Header("Debug")]
    [SerializeField] private int currentStageIndex = 0;

    private void Start()
    {
        SetStage(currentStageIndex);
    }

    // -------- PUBLIC API --------

    void OnEnable()
    { ProgressEventTrigger.onProgressEvent += EventHandler; }
    void OnDisable()
    { ProgressEventTrigger.onProgressEvent -= EventHandler; }

    void EventHandler(ProgressEvent receivedProgressEvent)
    {
        if (receivedProgressEvent == ProgressEvent.SetStage2 || receivedProgressEvent == ProgressEvent.SetStage3)
        {
            NextStage();
        }
        //Filter based on event and call the right function
    }

    public void NextStage()
    {
        if (isChangingStage) return;
        StartCoroutine(NextStageSequence());
    }

    private IEnumerator NextStageSequence()
    {
        isChangingStage = true;



        var fader = TransitionManager.Instance?.fader;

        if (fader != null)
        {
            yield return fader.FadeOut();
        }


        if (stages[currentStageIndex].resistanceManager != null)
        {
            ResistanceSystem rs = stages[currentStageIndex].resistanceManager.GetComponent<ResistanceSystem>();
            if (rs != null)
            {
                rs.ClearAllFighters();
            }
        }

        VideoClip clipToPlay = null;
        if (currentStageIndex == 0 && stage1To2Video != null)
        {
            clipToPlay = stage1To2Video;
        }
        else if (currentStageIndex == 1 && stage2To3Video != null)
        {
            clipToPlay = stage2To3Video;
        }

        if (clipToPlay != null && VideoManager.Instance != null)
        {
            if (fader != null) fader.SetVisible(false);

            yield return VideoManager.Instance.PlayVideo(clipToPlay);

            if (fader != null) fader.SetToBlack();
        }



        int nextIndex = currentStageIndex + 1;
        if (nextIndex < stages.Count)
        {
            SetStage(nextIndex);
        }
        else
        {
            Debug.Log("No more stages");
        }

        yield return new WaitForSeconds(1f);

        if (fader != null)
        {
            yield return fader.FadeIn();
        }



        isChangingStage = false;
    }

    public void SetStage(int stageIndex)
    {
        if (stageIndex < 0 || stageIndex >= stages.Count)
        {
            Debug.LogWarning("Invalid stage index");
            return;
        }

        // Disable all stages
        for (int i = 0; i < stages.Count; i++)
        {
            SetStageActive(stages[i], false);
        }

        // Enable chosen stage
        currentStageIndex = stageIndex;
        SetStageActive(stages[currentStageIndex], true);

        UpdateKebabArmar(stageIndex);

        Debug.Log($"Stage set to: {stages[currentStageIndex].stageName}");
    }

    public int GetCurrentStageIndex()
    {
        return currentStageIndex;
    }

    // -------- INTERNAL --------
    private void UpdateKebabArmar(int index)
    {
        bool isStage2OrHigher = index >= 1;
        if (kebabArmStage1A != null)
            kebabArmStage1A.SetActive(isStage2OrHigher);
        if (kebabArmStage1B != null)
            kebabArmStage1B.SetActive(isStage2OrHigher);

        if (stages[index].resistanceManager != null)
        {
            stages[index].resistanceManager.SetActive(isStage2OrHigher);
        }

        bool isStage3OrHigher = index >= 2;
        if (kebabArmStage2A != null)
            kebabArmStage2A.SetActive(isStage3OrHigher);
        if (kebabArmStage2B != null)
            kebabArmStage2B.SetActive(isStage3OrHigher);

    }

    private IEnumerator EnableResistanceAfterDelay(GameObject resistanceManager, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (resistanceManager != null)
        {
            resistanceManager.SetActive(true);
        }
    }

    private void SetStageActive(Stage stage, bool active)
    {
        if (stage.playerTyrant != null)
            stage.playerTyrant.SetActive(active);

        if (stage.parallaxBackground != null)
            stage.parallaxBackground.SetActive(active);

        if (stage.postProcessing != null)
            stage.postProcessing.SetActive(active);

        if (stage.resistanceManager != null)
        {

            if (active && currentStageIndex == 2)
            {
                stage.resistanceManager.SetActive(false);
                StartCoroutine(EnableResistanceAfterDelay(stage.resistanceManager, 30f));
            }
            else
            {
                stage.resistanceManager.SetActive(active);
            }
        }
    }
}
