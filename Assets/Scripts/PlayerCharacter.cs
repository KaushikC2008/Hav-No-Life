using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    public Animator animator;
    public PlayerData data;

    public CombatManager combatManager;

    private bool isDefending = false;

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
            combatManager.LogMessage("🔥 Casting Fireball");
        //animator.SetTrigger("CastFireBall");
    }

    public void PlayHit()
    {
        animator.SetTrigger("Hit");
    }

    public void Defend()
    {
        isDefending = true;
        if (combatManager != null)
            combatManager.LogMessage("🛡️ Defense stance activated! Defense tripled for next attack.");
        animator.SetTrigger("Defend");
    }

    public void SetDefending(bool defending)
    {
        isDefending = defending;
    }

    public void TakeDamage(int dmg)
    {
        int effectiveDefense = isDefending ? (data.defense * 3) : data.defense;
        int actualDamage = Mathf.Max(0, dmg - effectiveDefense);

        if(combatManager != null)
            combatManager.LogMessage($"Enemy hit for {dmg} raw damage. Defense absorbed {effectiveDefense}. Took {actualDamage} actual damage!");
        
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
}