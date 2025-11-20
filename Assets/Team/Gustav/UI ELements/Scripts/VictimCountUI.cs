using TMPro;
using UnityEngine;

public class VictimCountUI : MonoBehaviour
{
    public TMP_Text countText;
    private VictimSystem system;  // Reference to VictimSystem

    void Start()
    {
        system = FindAnyObjectByType<VictimSystem>();

        if (system != null)
        {
            // Subscribe to events
            system.OnVictimCountChanged += UpdateCount;

            // Update once on start
            UpdateCount(system.victims.Count);
        }
    }

    void OnDestroy()
    {
        // Unsubscribe on destroy to avoid memory leaks
        if (system != null)
            system.OnVictimCountChanged -= UpdateCount;
    }

    private void UpdateCount(int newCount)
    {
        countText.text = $"Humans: {newCount}";
    }
}
