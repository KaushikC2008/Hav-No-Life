using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    public Animator animator;
    public PlayerData data;

    public CombatManager combatManager;

    private bool isDefending = false;

    public List<ActiveBuff> activeBuffs = new List<ActiveBuff>();

    public void Setup()
    {
        if (GameManager.Instance != null)
        {
            data = GameManager.Instance.PlayerData;
        }
    }

    public void PlayAttack()
    {
        animator.SetTrigger("Attack");
    }

    public void Focus()
    {
        int manaToAdd = data.maxManaPoints / 2;

        if (data.currentManaPoints + manaToAdd > data.maxManaPoints)
        {
            data.currentManaPoints = data.maxManaPoints;
        } else
        {
            data.currentManaPoints += manaToAdd;
        }
        animator.SetTrigger("Focus");
    }

    public void CastFireBall()
    {
        if (combatManager != null) 
            combatManager.LogMessage("[SPELL] Casting Fireball");
        animator.SetTrigger("FireBall");
    }

    public void PlayHit()
    {
        animator.SetTrigger("Hit");
    }

    public void Defend()
    {
        isDefending = true;
        if (combatManager != null)
            combatManager.LogMessage("[DEFENSE] Defense stance activated! Defense tripled for next attack.");
        animator.SetTrigger("Defend");
    }

    public void SetDefending(bool defending)
    {
        isDefending = defending;
    }

    public void TakeDamage(int dmg)
    {
        int effectiveDefense = isDefending ? (GetTotalDefense() * 3) : GetTotalDefense();
        int actualDamage = Mathf.Max(0, dmg - effectiveDefense);

        if(combatManager != null)
            combatManager.LogMessage($"[DAMAGE] Enemy hit for {dmg} raw damage. Defense absorbed {effectiveDefense}. Took {actualDamage} actual damage!");
        
        data.currentHealth -= actualDamage;

        isDefending = false;
        if (data.currentHealth <= 0)
        {
            data.currentHealth = 0;
            animator.SetTrigger("Die");
        }
        else
        {
            PlayHit();
        }
    }

    public int GetCurrentHealth()
    {
        return data.currentHealth;
    }

    public int GetCurrentMana()
    {
        return data.currentManaPoints;
    }

    public void PlayRun()
    {
        animator.SetBool("Run",true);
    }

    public void StopRun()
    {
        animator.SetBool("Run",false);
    }

    public void SetHealth(int health)
    {
        if (data != null)
        {
            data.currentHealth = Mathf.Clamp(health, 0, data.maxHealth);
        }
    }

    public void SetMana(int mana)
    {
        if (data != null)
        {
            data.currentManaPoints = Mathf.Clamp(mana, 0, data.maxManaPoints);
        }
    }
    
    public int GetAttack()
    {
        int totalAttack = data.attack;
        if (data.equipmentData.equippedAccessory1 != null)
            totalAttack += data.equipmentData.equippedAccessory1.attackBonus;
        if (data.equipmentData.equippedAccessory2 != null)
            totalAttack += data.equipmentData.equippedAccessory2.attackBonus;
        if (data.equipmentData.equippedAccessory3 != null)
            totalAttack += data.equipmentData.equippedAccessory3.attackBonus;
        if (data.equipmentData.equippedAccessory4 != null)
            totalAttack += data.equipmentData.equippedAccessory4.attackBonus;
        if (data.equipmentData.equippedAccessory5 != null)
            totalAttack += data.equipmentData.equippedAccessory5.attackBonus;
        return totalAttack;
    }

    public int GetSpecialAttack()
    {
       
       int totalSpecialAttack = data.specialAttack;
       if(data.equipmentData.equippedAccessory1 != null)
           totalSpecialAttack += data.equipmentData.equippedAccessory1.specialAttackBonus;
       if(data.equipmentData.equippedAccessory2 != null)
           totalSpecialAttack += data.equipmentData.equippedAccessory2.specialAttackBonus;
       if(data.equipmentData.equippedAccessory3 != null)
           totalSpecialAttack += data.equipmentData.equippedAccessory3.specialAttackBonus;
       if(data.equipmentData.equippedAccessory4 != null)
           totalSpecialAttack += data.equipmentData.equippedAccessory4.specialAttackBonus;
       if(data.equipmentData.equippedAccessory5 != null)
           totalSpecialAttack += data.equipmentData.equippedAccessory5.specialAttackBonus;
       return totalSpecialAttack;
    }

    public int GetDefense()
    {
        int totalDefense = data.defense;
        if (data.equipmentData.equippedAccessory1 != null)
            totalDefense += data.equipmentData.equippedAccessory1.defenseBonus;
        if (data.equipmentData.equippedAccessory2 != null)
            totalDefense += data.equipmentData.equippedAccessory2.defenseBonus;
        if (data.equipmentData.equippedAccessory3 != null)
            totalDefense += data.equipmentData.equippedAccessory3.defenseBonus;
        if (data.equipmentData.equippedAccessory4 != null)
            totalDefense += data.equipmentData.equippedAccessory4.defenseBonus;
        if (data.equipmentData.equippedAccessory5 != null)
            totalDefense += data.equipmentData.equippedAccessory5.defenseBonus;
        return totalDefense;
    }

    public int GetMaxHealth()
    {
        int totalMaxHealth = data.maxHealth;

        if (data.equipmentData.equippedAccessory1 != null)
            totalMaxHealth += data.equipmentData.equippedAccessory1.maxHealthBonus;
        if (data.equipmentData.equippedAccessory2 != null)
            totalMaxHealth += data.equipmentData.equippedAccessory2.maxHealthBonus;
        if (data.equipmentData.equippedAccessory3 != null)
            totalMaxHealth += data.equipmentData.equippedAccessory3.maxHealthBonus;
        if (data.equipmentData.equippedAccessory4 != null)
            totalMaxHealth += data.equipmentData.equippedAccessory4.maxHealthBonus;
        if (data.equipmentData.equippedAccessory5 != null)
            totalMaxHealth += data.equipmentData.equippedAccessory5.maxHealthBonus;

        return totalMaxHealth;
    }

    public int GetMaxManaPoints()
    {
        int totalMaxMana = data.maxManaPoints;

        if (data.equipmentData.equippedAccessory1 != null)
            totalMaxMana += data.equipmentData.equippedAccessory1.maxManaBonus;
        if (data.equipmentData.equippedAccessory2 != null)
            totalMaxMana += data.equipmentData.equippedAccessory2.maxManaBonus;
        if (data.equipmentData.equippedAccessory3 != null)
            totalMaxMana += data.equipmentData.equippedAccessory3.maxManaBonus;
        if (data.equipmentData.equippedAccessory4 != null)
            totalMaxMana += data.equipmentData.equippedAccessory4.maxManaBonus;
        if (data.equipmentData.equippedAccessory5 != null)
            totalMaxMana += data.equipmentData.equippedAccessory5.maxManaBonus;

        return totalMaxMana;
    }

    public void UseConsumable(ConsumableData item)
    {
        switch (item.effectType)
        {
            case ConsumableEffectType.Heal:
                int newHealth = data.currentHealth + item.amount;
                SetHealth(newHealth);
                break;

            case ConsumableEffectType.RestoreMana:
                int newMana = data.currentManaPoints + item.amount;
                SetMana(newMana);
                break;

            case ConsumableEffectType.BuffAttack:
                AddBuff(item.itemName, item.amount, 0, 0, item.duration); 
                break;

            case ConsumableEffectType.BuffSpecialAttack:
                AddBuff(item.itemName, 0, 0, item.amount, item.duration); 
                break;

            case ConsumableEffectType.BuffDefense:
                AddBuff(item.itemName, 0, item.amount, 0, item.duration);
                break;
        }
    }

    public void AddBuff(string name, int atk, int def, int spAtk, int duration)
    {
        activeBuffs.Add(new ActiveBuff(name, atk, def, spAtk, duration));
        Debug.Log($"Applied buff: {name} for {duration} turns.");
    }

    public void TickBuffs()
    {
        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            activeBuffs[i].turnsRemaining--;
            if (activeBuffs[i].turnsRemaining <= 0)
            {
                Debug.Log($"Buff expired.");
                activeBuffs.RemoveAt(i);
            }
        }
    }

    public int GetTotalAttack()
    {
        int baseAtk = GetAttack(); 
        foreach (var buff in activeBuffs)
        {
            baseAtk += buff.attackBonus;
        }
        return baseAtk;
    }

    public int GetTotalSpecialAttack()
    {
        int baseSpAtk = GetSpecialAttack(); 
        foreach (var buff in activeBuffs)
        {
            baseSpAtk += buff.specialAttackBonus;
        }
        return baseSpAtk;
    }

    public int GetTotalDefense()
    {
        int baseDef = GetDefense(); 
        foreach (var buff in activeBuffs)
        {
            baseDef += buff.defenseBonus;
        }
        return baseDef;
    }
}

[System.Serializable]
public class ActiveBuff
{
    string buffName;
    public int attackBonus;
    public int defenseBonus;
    public int specialAttackBonus;
    public int turnsRemaining;

    public ActiveBuff(string name, int atk, int def, int spAtk, int durationTurns)
    {
        buffName = name;
        attackBonus = atk;
        defenseBonus = def;
        specialAttackBonus = spAtk;
        turnsRemaining = durationTurns;
    }
}