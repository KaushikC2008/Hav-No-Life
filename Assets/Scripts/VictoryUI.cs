using UnityEngine;
using TMPro;

public class VictoryUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI statsText;
    [SerializeField] private TextMeshProUGUI rewardsText;

    public void DisplayVictoryData(string title, string stats, string loot)
    {
        if (titleText != null) titleText.text = title;
        if (statsText != null) statsText.text = stats;
        if (rewardsText != null) rewardsText.text = loot;

        gameObject.SetActive(true);
    }
}