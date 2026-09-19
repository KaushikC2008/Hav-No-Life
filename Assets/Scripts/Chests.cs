using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Chest : MonoBehaviour
{
    [Header("Chest Data Reference")]
    [SerializeField] private ChestData chestData;

    [Header("Visuals")]
    [SerializeField] private Animator animator;

    [Header("Reward UI Panel")]
    [SerializeField] private GameObject rewardPanel;
    [SerializeField] private TextMeshProUGUI rewardText;

    [Header("Display Settings")]
    [SerializeField] private float displayDuration = 3f;

    private bool isOpen = false;

    private void Start()
    {
        // Automatically assign the animator controller from ChestData if available
        if (chestData != null && chestData.animatorController != null)
        {
            if (animator == null)
            {
                animator = GetComponent<Animator>();
                if (animator == null)
                {
                    animator = gameObject.AddComponent<Animator>();
                }
            }
            animator.runtimeAnimatorController = chestData.animatorController;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || isOpen)
        {
            return;
        }
        StartCoroutine(OpenChestRoutine(other.gameObject));
    }

    private IEnumerator OpenChestRoutine(GameObject player)
    {
        isOpen = true;

        if (animator != null)
        {
            animator.SetTrigger("Open");
            yield return new WaitForSeconds(1f);
        }

        // Stop player movement and reset run animation
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        Animator playerAnimator = player.GetComponent<Animator>();
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("isRunning", false);
        }

        PlayerMovement movementScript = player.GetComponent<PlayerMovement>();
        if (movementScript != null)
        {
            movementScript.enabled = false;
        }

        // Get drops using the separate ChestData asset
        string lootResults = "";
        if (chestData != null)
        {
            lootResults = DropManager.Instance.GiveChestDropsAndGetText(chestData.drops);
        }
        else
        {
            Debug.LogWarning("ChestData is missing on " + gameObject.name);
            lootResults = "The chest was empty!";
        }

        if (rewardPanel != null && rewardText != null)
        {
            rewardText.text = lootResults;
            rewardPanel.SetActive(true);
        }

        // Wait for the display duration
        yield return new WaitForSeconds(displayDuration);

        // Hide panel and re-enable player movement
        if (rewardPanel != null)
        {
            rewardPanel.SetActive(false);
        }

        if (movementScript != null)
        {
            movementScript.enabled = true;
        }

        // Destroy the chest
        Destroy(gameObject);
    }
}