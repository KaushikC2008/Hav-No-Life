using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerData", menuName = "ScriptableObjects/PlayerData")]
public class PlayerData : ScriptableObject
{
    [Header("Base Info")]
    public string playerName;
    public Sprite combatSprite;
    public EquipmentData equipmentData = new EquipmentData();

    [Header("Leveling")]
    public int currentLevel = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 100;

    [Header("Currency")]
    public int currentGold;

    [Header("Stats")]
    public int maxHealth;
    public int maxManaPoints;
    public int currentHealth;
    public int currentManaPoints;
    public int attack;
    public int defense;

    [Header("Special Attack")]
    public int baseSpecialAttack = 15;
    public int specialAttack => baseSpecialAttack + (currentLevel * 5);

    [Header("Overworld & Checkpoints")]
    public Vector3 overworldReturnPosition;
    public Vector3 latestCheckpointPosition;
    public bool hasCheckpoint = false;

    [Header("Inventory")]
    public List<ConsumableEntry> consumables = new List<ConsumableEntry>();
    public List<Accessaries> ownedAccessories = new List<Accessaries>();

}
[System.Serializable]
public class EquipmentData
{
    public Accessaries equippedAccessory1;
    public Accessaries equippedAccessory2;
    public Accessaries equippedAccessory3;
    public Accessaries equippedAccessory4;
    public Accessaries equippedAccessory5;

    public bool isAccessory3Unlocked = false;
    public bool isAccessory4Unlocked = false;
    public bool isAccessory5Unlocked = false;
}
[System.Serializable]
public class ConsumableEntry
{
    public ConsumableData item;
    public int amount;
}