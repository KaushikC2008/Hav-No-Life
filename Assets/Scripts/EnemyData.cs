using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "ScriptableObjects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Base Info")]
    public string enemyName;
    public Sprite combatSprite;

    [Header("Stats")]
    public int maxHealth;
    public int attack;
    public int defense;
    public float speed;

    [Header("Rewards")]
    public int xpReward;
    public int goldReward;

    [Header("Combat Display")]
    public bool flipSprite;
    public RuntimeAnimatorController animatorController;

    [Header("Position")]
    public Vector2 combatPosition;
    public Vector2 playerCombatOffset = new Vector2(-2f, 0f);

    [Header("Mana & Spells")]
    public int maxManaPoints;
    public int specialAttackCost;
    public int specialAttackDamage => Mathf.RoundToInt(attack * 1.5f); // Example: Special attack deals 1.5x base attack damage
}