using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerData", menuName = "ScriptableObjects/PlayerData")]
public class PlayerData : ScriptableObject
{
    [Header("Base Info")]
    public string playerName;
    public Sprite combatSprite;

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
    public float speed;
}