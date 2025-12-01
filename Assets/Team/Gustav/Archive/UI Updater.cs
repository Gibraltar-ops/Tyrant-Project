using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIUpdater : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI txtBiomass;
    public TextMeshProUGUI txtBrains;
    public TextMeshProUGUI txtLevels;

    [Header("Buttons")]
    public Button btn_BioIncrease10;
    public Button btn_BioDecrease10;
    public Button btn_BrainIncrease10;
    public Button btn_BrainDecrease10;
    public Button btn_LevelIncrease10;
    public Button btn_LevelDecrease10;

    [Header("Values")]
    private int bioCurrent = 0;
    private int brainCurrent = 0;
    private int levelCurrent = 0;
    private int bioChange = 10;
    private int brainChange = 5;
    private int levelChange = 1;

    private void Start()
    {
        // Add button listeners
        btn_BioIncrease10.onClick.AddListener(UpdateFromBtnIncreaseBio);
        btn_BioDecrease10.onClick.AddListener(UpdateFromBtnDecreaseBio);
        btn_BrainIncrease10.onClick.AddListener(UpdateFromBtnIncreaseBrain);
        btn_BrainDecrease10.onClick.AddListener(UpdateFromBtnDecreaseBrain);
        btn_LevelIncrease10.onClick.AddListener(UpdateFromBtnIncreaseLevel);
        btn_LevelDecrease10.onClick.AddListener(UpdateFromBtnDecreaseLevel);

        UpdateAllText();
    }

    void UpdateFromBtnIncreaseBio() { bioCurrent += bioChange; UpdateAllText(); }
    void UpdateFromBtnDecreaseBio() { bioCurrent -= bioChange; UpdateAllText(); }
    void UpdateFromBtnIncreaseBrain() { brainCurrent += brainChange; UpdateAllText(); }
    void UpdateFromBtnDecreaseBrain() { brainCurrent -= brainChange; UpdateAllText(); }
    void UpdateFromBtnIncreaseLevel() { levelCurrent += levelChange; UpdateAllText(); }
    void UpdateFromBtnDecreaseLevel() { levelCurrent -= levelChange; UpdateAllText(); }

    private void UpdateAllText()
    {
        txtBiomass.text = "BIOMASS: " + bioCurrent;
        txtBrains.text = "BRAIN: " + brainCurrent;
        txtLevels.text = "LEVEL: " + levelCurrent;
    }

    // --- Public access for SaveLoad ---
    public int GetBio() => bioCurrent;
    public int GetBrains() => brainCurrent;
    public int GetLevels() => levelCurrent;

    public void SetValues(int bio, int brain, int level)
    {
        bioCurrent = bio;
        brainCurrent = brain;
        levelCurrent = level;
        UpdateAllText();
    }
}
