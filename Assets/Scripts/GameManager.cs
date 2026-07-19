using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player")]
    public PlayerData PlayerData;
    
    //Enemy
    public EnemyData CurrentEnemy { get; private set; }
    public List<string> DefeatedEnemies = new List<string>();
    public string CurrentEnemyID;

    public Vector3 LastPlayerPosition;
    public bool ShouldRestorePosition = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            PlayerData = Instantiate(PlayerData);
            PlayerData.currentHealth = PlayerData.maxHealth;
            PlayerData.currentManaPoints = PlayerData.maxManaPoints;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetCurrentEnemy(EnemyData enemy)
    {
        CurrentEnemy = enemy;
    }

    public void ClearCurrentEnemy()
    {
        CurrentEnemy = null;
    }

    public void DefeatEnemy(string id)
    {
        if (!DefeatedEnemies.Contains(id))
        {
            DefeatedEnemies.Add(id);
        }
    }

    public bool GainXP(int xpAmount)
    {
        PlayerData.currentXP += xpAmount;
        Debug.Log($"Gained {xpAmount} XP! Total XP: {PlayerData.currentXP}/{PlayerData.xpToNextLevel}");

        bool didLevelUp = false;

        while (PlayerData.currentXP >= PlayerData.xpToNextLevel)
        {
            LevelUp();
            didLevelUp = true;
        }
        return didLevelUp;
    }

   private void LevelUp()
    {
        PlayerData.currentXP -= PlayerData.xpToNextLevel;
        PlayerData.currentLevel++;
        
        PlayerData.xpToNextLevel = 100 * PlayerData.currentLevel;

        PlayerData.maxHealth += 10;
        PlayerData.maxManaPoints += 10;
        PlayerData.currentHealth = PlayerData.maxHealth;
        PlayerData.currentManaPoints = PlayerData.maxManaPoints;
        PlayerData.attack += 5;
        PlayerData.defense += 2;
        PlayerData.speed += 5;

        Debug.Log($"Leveled up to Level {PlayerData.currentLevel}! Attack and Defense increased!");
    }

    public void GainGold(int goldAmount)
    {
        PlayerData.currentGold += goldAmount;
        Debug.Log($"Gained {goldAmount} Gold! Total Gold: {PlayerData.currentGold}");
    }
}