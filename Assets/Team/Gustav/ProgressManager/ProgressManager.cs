using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class ProgressManager : MonoBehaviour
{
    [System.Serializable]
    public class Stage
    {
        public string stageName;

        public GameObject playerTyrant;
        public GameObject parallaxBackground;
        public GameObject postProcessing;
        public GameObject resistanceManager;
    }

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
    { ProgressEventTrigger.onProgressEvent += MyFunction; }
    void OnDisable()
    { ProgressEventTrigger.onProgressEvent -= MyFunction; }

    void MyFunction(ProgressEvent receivedProgressEvent)
    {
        NextStage();
        //Filter based on event and call the right function
    }

    public void NextStage()
    {
        StartCoroutine(NextStageSequence());
    }

    private IEnumerator NextStageSequence()
    {
        if (TransitionManager.Instance != null && TransitionManager.Instance.fader != null)
        {
            yield return TransitionManager.Instance.fader.FadeOut();
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

        if (TransitionManager.Instance != null && TransitionManager.Instance.fader != null)
        {
            yield return TransitionManager.Instance.fader.FadeIn();
        }
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

    private void SetStageActive(Stage stage, bool active)
    {
        if (stage.playerTyrant != null)
            stage.playerTyrant.SetActive(active);

        if (stage.parallaxBackground != null)
            stage.parallaxBackground.SetActive(active);

        if (stage.postProcessing != null)
            stage.postProcessing.SetActive(active);

        if (stage.resistanceManager != null)
            stage.resistanceManager.SetActive(active);
    }
}
