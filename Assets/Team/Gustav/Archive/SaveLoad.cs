using System;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem; // New Input System

public class SaveLoad : MonoBehaviour
{
    private string saveFilePath;
    private UIUpdater uiUpdater;

    private void Awake()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "playerdata.json");
    }

    private void Start()
    {
        // Find the UIUpdater in the scene
        uiUpdater = FindFirstObjectByType<UIUpdater>();

        // Load saved data automatically at startup
        if (uiUpdater != null && File.Exists(saveFilePath))
        {
            PlayerData data = Load();
            uiUpdater.SetValues(data.curBiomass, data.curBrains, data.curLevels);
        }
    }

    private void Update()
    {
        // Manual save
        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            ManualSave();
        }

        // Manual load
        if (Keyboard.current.lKey.wasPressedThisFrame)
        {
            ManualLoad();
        }
    }

    public void Save(PlayerData data)
    {
        try
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(saveFilePath, json);
            Debug.Log($"Data saved to {saveFilePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save data: {e.Message}");
        }
    }

    public PlayerData Load()
    {
        try
        {
            if (File.Exists(saveFilePath))
            {
                string json = File.ReadAllText(saveFilePath);
                PlayerData data = JsonUtility.FromJson<PlayerData>(json);
                Debug.Log("Data loaded successfully.");
                return data;
            }
            else
            {
                Debug.LogWarning("Save file not found. Returning default data.");
                return new PlayerData();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load data: {e.Message}");
            return new PlayerData();
        }
    }

    private void ManualSave()
    {
        if (uiUpdater == null)
        {
            Debug.LogWarning("UIUpdater not found, cannot save manually.");
            return;
        }

        PlayerData data = new PlayerData
        {
            curBiomass = uiUpdater.GetBio(),
            curBrains = uiUpdater.GetBrains(),
            curLevels = uiUpdater.GetLevels()
        };

        Save(data);
    }

    private void ManualLoad()
    {
        if (uiUpdater == null)
        {
            Debug.LogWarning("UIUpdater not found, cannot load manually.");
            return;
        }

        PlayerData data = Load();
        uiUpdater.SetValues(data.curBiomass, data.curBrains, data.curLevels);
    }

    [Serializable]
    public class PlayerData
    {
        public int curBiomass;
        public int curBrains;
        public int curLevels;
    }
}
