using TMPro;
using UnityEngine;

public class InventoryUiScript : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private TextMeshProUGUI inventoryContentText;

    private bool isOpen = false;

    void Start()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        isOpen = !isOpen;
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(isOpen);
        }

        if (isOpen)
        {
            DisplayInventoryData();
        }
    }

    private void DisplayInventoryData()
    {
        if (GameManager.Instance == null || GameManager.Instance.PlayerData == null) return;

        string inventoryText = "=== OWNED ACCESSORIES ===\n";
        
        if (GameManager.Instance.PlayerData.ownedAccessories.Count == 0)
        {
            inventoryText += "None\n";
        }
        else
        {
            foreach (var accessory in GameManager.Instance.PlayerData.ownedAccessories)
            {
                inventoryText += $"- {accessory.accessoryName} (ATK: +{accessory.attackBonus}, DEF: +{accessory.defenseBonus})\n";
            }
        }

        inventoryText +="\n=== CONSUMABLES ===\n";
        if (GameManager.Instance.PlayerData.consumables.Count == 0)
        {
            inventoryText += "None\n";
        }
        else
        {
            foreach (var entry in GameManager.Instance.PlayerData.consumables)
            {
                if (entry.item != null)
                {
                    inventoryText += $"- {entry.item.itemName} x{entry.amount}\n";
                }
            }
        }

        if (inventoryContentText != null)
        {
            inventoryContentText.text = inventoryText;
        }
    }
}
