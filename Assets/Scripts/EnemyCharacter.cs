using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : MonoBehaviour
{
    public Animator animator;
    public EnemyData data;

    private int currentHP;
    private int currentMana;

    public CombatManager combatManager;

    public void Setup(EnemyData newData)
    {
        data = newData;
        animator.runtimeAnimatorController = data.animatorController;
        currentHP = data.maxHealth;
        currentMana = data.maxManaPoints;
    }

    public void Focus()
    {
        int manaToAdd = data.maxManaPoints / 2;
        if (currentMana + manaToAdd > data.maxManaPoints)
        {
            currentMana = data.maxManaPoints;
        }
        else
        {
            currentMana += manaToAdd;
        }
        if (combatManager != null)
        {
            combatManager.LogMessage($"🧠 {data.enemyName} focused! Regenerated mana to {currentMana}/{data.maxManaPoints}");
        }
        // animator.SetTrigger("Focus");
    }

    public void SpendMana(int amount)
    {
        currentMana -= amount;
    }

    public int GetCurrentMana()
    {
        return currentMana;
    }

    public void TakeDamage(int dmg)
    {
        int actualDamage = Mathf.Max(0, dmg - data.defense);
        int displayedAbsorption = Mathf.Min(dmg, data.defense);

        if (combatManager != null)
        {
            combatManager.LogMessage($"Hit {data.enemyName} for {dmg} raw damage. Enemy defense absorbed {displayedAbsorption}. Dealt {actualDamage} actual damage!");
        }

        currentHP -= actualDamage;
        if (currentHP <= 0)
        {
            currentHP = 0;
            animator.SetTrigger("Die");
        }
        else
        {
            animator.SetTrigger("Hit");
        }
    }

    public int GetCurrentHP()
    {
        return currentHP;
    }

    public void PlayAttack()
    {
        animator.SetTrigger("Attack");
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