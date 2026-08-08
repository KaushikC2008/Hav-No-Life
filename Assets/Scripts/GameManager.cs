using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player")]
    public PlayerData PlayerData;

    public EnemyData CurrentEnemy { get; private set; }
    public List<string> DefeatedEnemies = new List<string>();
    public string CurrentEnemyID;
    

    [Header("Demo Progress")]
    private List<string> RegisteredEnemies = new List<string>();
    public int TotalNormalEnemies = 0;
    public bool BossDefeated = false;
    public int DefeatedNormalEnemies = 0;
    public bool CurrentEnemyIsBoss;

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

    public void SavePlayerPosition(Vector3 position)
    {
        LastPlayerPosition = position;
        ShouldRestorePosition = true;
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
        if (DefeatedEnemies.Contains(id))
            return;

        DefeatedEnemies.Add(id);

        if (CurrentEnemyIsBoss)
        {
            BossDefeated = true;
            Debug.Log("Boss defeated!");
        }
        else
        {
            DefeatedNormalEnemies++;
        }

        Debug.Log(
            $"Enemy defeated: {id} | " +
            $"Normal enemies: {DefeatedNormalEnemies}/{TotalNormalEnemies} | " +
            $"Boss defeated: {BossDefeated}"
        );
    }

    public void RegisterNormalEnemy()
    { 
        TotalNormalEnemies++; 
    }

    public void RegisterEnemy(string enemyID, bool isBoss)
    {
        if (RegisteredEnemies.Contains(enemyID))
            return;

        RegisteredEnemies.Add(enemyID);

        if (!isBoss)
        {
            TotalNormalEnemies++;
        }

        Debug.Log(
            $"Registered Enemy: {enemyID} | " +
            $"Total Normal Enemies: {TotalNormalEnemies}"
        );
    }

    public bool IsDemoFinished()
    {
        int requiredEnemies = Mathf.CeilToInt(TotalNormalEnemies * 0.9f);
        return DefeatedEnemies.Count >= requiredEnemies && BossDefeated;
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