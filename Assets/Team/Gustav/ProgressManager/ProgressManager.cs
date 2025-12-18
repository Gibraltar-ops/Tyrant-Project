using UnityEngine;
using System.Collections.Generic;

public class ProgressManager : MonoBehaviour
{
    [System.Serializable]
    public class Stage
    {
        public string stageName;

        public GameObject playerTyrant;
        public GameObject parallaxBackground;
        public GameObject postProcessing;
        public GameObject ressistanceManager;
    }

    [Header("Stages")]
    [SerializeField] private List<Stage> stages = new List<Stage>();

    [Header("Debug")]
    [SerializeField] private int currentStageIndex = 0;

    private void Start()
    {
        SetStage(currentStageIndex);
    }

    // -------- PUBLIC API --------

    public void NextStage()
    {
        int nextIndex = currentStageIndex + 1;
        if (nextIndex < stages.Count)
        {
            SetStage(nextIndex);
        }
        else
        {
            Debug.Log("No more stages.");
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

        Debug.Log($"Stage set to: {stages[currentStageIndex].stageName}");
    }

    public int GetCurrentStageIndex()
    {
        return currentStageIndex;
    }

    // -------- INTERNAL --------

    private void SetStageActive(Stage stage, bool active)
    {
        if (stage.playerTyrant != null)
            stage.playerTyrant.SetActive(active);

        if (stage.parallaxBackground != null)
            stage.parallaxBackground.SetActive(active);

        if (stage.postProcessing != null)
            stage.postProcessing.SetActive(active);
    }
}
