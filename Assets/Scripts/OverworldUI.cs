using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OverworldUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI goldText;

    void Start()
    {
        UpdateGoldDisplay();
    }

    public void UpdateGoldDisplay()
    {
        // Check if the GameManager and PlayerData exist before trying to read them
        if (GameManager.Instance != null && GameManager.Instance.PlayerData != null)
        {
            goldText.text = $"Gold: {GameManager.Instance.PlayerData.currentGold}";
        }
    }
}
