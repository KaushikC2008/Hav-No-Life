using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    public Animator animator;
    public PlayerData data;

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
        if (data.currentManaPoints + 1/2 * data.maxManaPoints > data.maxManaPoints)
        {
            data.currentManaPoints = data.maxManaPoints;
        } else
        {
            data.currentManaPoints += 1/2 * data.maxManaPoints;
        }
        //animator.SetTrigger("Focus");
    }

    public void CastFireBall()
    {
        Debug.Log("Casting Fireball");
        //animator.SetTrigger("CastFireBall");
    }

    public void PlayHit()
    {
        animator.SetTrigger("Hit");
    }

    public void TakeDamage(int dmg)
    {
        data.currentHealth -= dmg;
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