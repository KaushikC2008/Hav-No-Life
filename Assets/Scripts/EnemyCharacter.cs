using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : MonoBehaviour
{
    public Animator animator;
    public EnemyData data;

    private int currentHP;

    public void Setup(EnemyData newData)
    {
        data = newData;
        animator.runtimeAnimatorController = data.animatorController;
        currentHP = data.maxHealth;
    }

    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        if (currentHP <= 0)
        {
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