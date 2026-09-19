using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CombinedInventoryEquipmentUI : MonoBehaviour
{
    [Header("Combined UI Panel")]
    [SerializeField] private GameObject menuPanel;

    [Header("Display Text References")]
    [SerializeField] private TextMeshProUGUI equippedSlotsText;
    [SerializeField] private TextMeshProUGUI consumablesListText;

    [Header("Action Containers & Prefabs")]
    [SerializeField] private Transform ownedAccessoriesContainer;
    [SerializeField] private GameObject actionButtonPrefab;

    private bool isOpen = false;

    void Start()
    {
        if (menuPanel != null)
        {
            menuPanel.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleMenu();
        }
    }

    public void ToggleMenu()
    {
        isOpen = !isOpen;
        if (menuPanel != null)
        {
            menuPanel.SetActive(isOpen);
        }

        if (isOpen)
        {
            RefreshUI();
        }
    }

    public void RefreshUI()
    {
        if (GameManager.Instance == null || GameManager.Instance.PlayerData == null) return;

        EquipmentData eq = GameManager.Instance.PlayerData.equipmentData;

        string equippedText = "=== EQUIPPED ACCESSORIES ===\n";
        equippedText += $"1: {(eq.equippedAccessory1 != null ? eq.equippedAccessory1.accessoryName : "Empty")}\n";
        equippedText += $"2: {(eq.equippedAccessory2 != null ? eq.equippedAccessory2.accessoryName : "Empty")}\n";
        equippedText += $"3: {(eq.isAccessory3Unlocked ? (eq.equippedAccessory3 != null ? eq.equippedAccessory3.accessoryName : "Empty") : "Locked")}\n";
        equippedText += $"4: {(eq.isAccessory4Unlocked ? (eq.equippedAccessory4 != null ? eq.equippedAccessory4.accessoryName : "Empty") : "Locked")}\n";
        equippedText += $"5: {(eq.isAccessory5Unlocked ? (eq.equippedAccessory5 != null ? eq.equippedAccessory5.accessoryName : "Empty") : "Locked")}\n";

        if (equippedSlotsText != null)
        {
            equippedSlotsText.text = equippedText;
        }

        string consumableText = "=== CONSUMABLES ===\n";
        if (GameManager.Instance.PlayerData.consumables.Count == 0)
        {
            consumableText += "None\n";
        }
        else
        {
            foreach (var entry in GameManager.Instance.PlayerData.consumables)
            {
                if (entry.item != null && entry.amount > 0)
                {
                    consumableText += $"- {entry.item.itemName} x{entry.amount}\n";
                }
            }
        }

        if (consumablesListText != null)
        {
            consumablesListText.text = consumableText;
        }

        foreach (Transform child in ownedAccessoriesContainer)
        {
            Destroy(child.gameObject);
        }

        CreateUnequipButtons();
        CreateEquipButtons();
    }

    private void CreateUnequipButtons()
    {
        EquipmentData eq = GameManager.Instance.PlayerData.equipmentData;

        if (eq.equippedAccessory1 != null) AddActionBtn("Unequip Slot 1", () => UnequipSlot(1));
        if (eq.equippedAccessory2 != null) AddActionBtn("Unequip Slot 2", () => UnequipSlot(2));
        if (eq.isAccessory3Unlocked && eq.equippedAccessory3 != null) AddActionBtn("Unequip Slot 3", () => UnequipSlot(3));
        if (eq.isAccessory4Unlocked && eq.equippedAccessory4 != null) AddActionBtn("Unequip Slot 4", () => UnequipSlot(4));
        if (eq.isAccessory5Unlocked && eq.equippedAccessory5 != null) AddActionBtn("Unequip Slot 5", () => UnequipSlot(5));
    }

    private void CreateEquipButtons()
    {
        foreach (var accessory in GameManager.Instance.PlayerData.ownedAccessories)
        {
            if (accessory == null) continue;
            
            EquipmentData eq = GameManager.Instance.PlayerData.equipmentData;
            if (eq.equippedAccessory1 == accessory || eq.equippedAccessory2 == accessory ||
                eq.equippedAccessory3 == accessory || eq.equippedAccessory4 == accessory || 
                eq.equippedAccessory5 == accessory)
            {
                continue; 
            }

            GameObject btnObj = Instantiate(actionButtonPrefab, ownedAccessoriesContainer);
            TextMeshProUGUI btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            if (btnText != null)
            {
                btnText.text = $"Equip: {accessory.accessoryName}";
            }

            Button btn = btnObj.GetComponent<Button>();
            Accessaries accToEquip = accessory;
            btn.onClick.AddListener(() => EquipAccessory(accToEquip));
        }
    }

    private void AddActionBtn(string label, UnityEngine.Events.UnityAction action)
    {
        GameObject btnObj = Instantiate(actionButtonPrefab, ownedAccessoriesContainer);
        TextMeshProUGUI btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();
        if (btnText != null)
        {
            btnText.text = label;
        }
        Button btn = btnObj.GetComponent<Button>();
        btn.onClick.AddListener(action);
    }

    public void EquipAccessory(Accessaries accessory)
    {
        EquipmentData eq = GameManager.Instance.PlayerData.equipmentData;

        if (eq.equippedAccessory1 == null) eq.equippedAccessory1 = accessory;
        else if (eq.equippedAccessory2 == null) eq.equippedAccessory2 = accessory;
        else if (eq.isAccessory3Unlocked && eq.equippedAccessory3 == null) eq.equippedAccessory3 = accessory;
        else if (eq.isAccessory4Unlocked && eq.equippedAccessory4 == null) eq.equippedAccessory4 = accessory;
        else if (eq.isAccessory5Unlocked && eq.equippedAccessory5 == null) eq.equippedAccessory5 = accessory;
        else
        {
            Debug.Log("All equipment slots are full!");
            return;
        }

        RefreshUI();
    }

    public void UnequipSlot(int slotIndex)
    {
        EquipmentData eq = GameManager.Instance.PlayerData.equipmentData;

        switch (slotIndex)
        {
            case 1: eq.equippedAccessory1 = null; break;
            case 2: eq.equippedAccessory2 = null; break;
            case 3: eq.equippedAccessory3 = null; break;
            case 4: eq.equippedAccessory4 = null; break;
            case 5: eq.equippedAccessory5 = null; break;
        }

        RefreshUI();
    }
}