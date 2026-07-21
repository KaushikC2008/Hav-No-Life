using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class CombatManager : MonoBehaviour
{
    [Header("Enemy World Visuals")]
    [SerializeField] private SpriteRenderer enemyWorldSpriteRenderer;

    [Header("Enemy")]
    [SerializeField] private TextMeshProUGUI enemyNameText;
    [SerializeField] private TextMeshProUGUI enemyHealthText;
    [SerializeField] private EnemyCharacter enemy;
    private Vector3 startEnemyPosition;

    [Header("Animation")]
    public Animator playerAnimator;
    private bool isAttacking = false;

    [Header("Player")]
    public PlayerCharacter player;
    [SerializeField] private TextMeshProUGUI playerHealthText;
    [SerializeField] private TextMeshProUGUI playerManaText;
    private Vector3 playerStartPosition;
    private float attackDistance = 1.5f;
    [SerializeField] private float moveSpeed = 5f;

    [Header("UI")]
    [SerializeField] private GameObject attackPanel;
    [SerializeField] private GameObject levelUpPanel;
    [SerializeField] private TextMeshProUGUI levelUpTitleText;
    [SerializeField] private TextMeshProUGUI levelUpStatsText;
    [SerializeField] private GameObject gameOverPanel;

    [Header("Combat Log UI")]
    [SerializeField] private Transform logContentParent;
    [SerializeField] private GameObject logTextPrefab;
    [SerializeField] private ScrollRect combatLogScrollRect;

    [Header("Turn Indicator UI")]
    [SerializeField] private Image turnBannerImage;
    [SerializeField] private Sprite playerTurnSprite;
    [SerializeField] private Sprite enemyTurnSprite;

    private BattleState state;

    void Start()
    {
        player.Setup();
        player.combatManager = this;
        playerStartPosition = player.transform.position;

        if (GameManager.Instance != null && GameManager.Instance.CurrentEnemy != null)
        {
            SetupBattle(GameManager.Instance.CurrentEnemy);
        } else {
            Debug.LogWarning("No enemy data found!");
        }
    }

    void SetupBattle(EnemyData data)
    {
        enemy.Setup(data);
        enemy.combatManager = this;
        enemyNameText.text = data.enemyName;
        enemyHealthText.text = $"HP: {data.maxHealth}/{data.maxHealth}";
        playerHealthText.text = $"HP: {player.GetCurrentHealth()}/{player.data.maxHealth}";
        playerManaText.text = $"MP: {player.GetCurrentMana()}/{player.data.maxManaPoints}";
        startEnemyPosition = data.combatPosition;
        attackDistance = data.playerCombatOffset.x;

        if (combatLogScrollRect != null) combatLogScrollRect.enabled = true;

        if (enemyWorldSpriteRenderer != null && data.combatSprite != null)
        {
            enemyWorldSpriteRenderer.sprite = data.combatSprite;
            enemyWorldSpriteRenderer.flipX = data.flipSprite;
            enemyWorldSpriteRenderer.transform.position = data.combatPosition;
        }
        state = BattleState.PlayerTurn;
        UpdateTurnBanner(true);
    }

    public void BasicAttack()
    {
        if (state != BattleState.PlayerTurn)
            return;
        
        if (isAttacking)
            return;
        StartCoroutine(PlayerAttackRoutine());
    }

    public void FireBall()
    {
        if (state != BattleState.PlayerTurn)
        return;

        if (player.GetCurrentMana() < 5)
        {
            LogMessage("Not enough mana to cast Fireball!");
            return;
        }

        StartCoroutine(FireBallRoutine());
    }

    private IEnumerator FireBallRoutine()
    {
        state = BattleState.Busy;
        SetPlayerControls(false);

        player.data.currentManaPoints -= 5;
        playerManaText.text = $"MP: {player.GetCurrentMana()}/{player.data.maxManaPoints}";

        player.CastFireBall(); 
        
        yield return new WaitForSeconds(0.5f); 

        enemy.TakeDamage(player.data.attack + 2);
        enemyHealthText.text = $"HP: {enemy.GetCurrentHP()}/{enemy.data.maxHealth}";

        yield return new WaitForSeconds(0.5f);

        if (enemy.GetCurrentHP() <= 0)
        {
            yield return StartCoroutine(HandleEnemyDeath());
            yield break;
        }

        StartCoroutine(EnemyTurn());
    }

    public void Focus()
    {
        if (state != BattleState.PlayerTurn)
            return;

        if (player.GetCurrentMana() >= player.data.maxManaPoints)
        {
            LogMessage("Player's mana is already full!");
            return;
        }

        StartCoroutine(FocusRoutine());
    }

    private IEnumerator FocusRoutine()
    {
        state = BattleState.Busy;
        SetPlayerControls(false);

        player.Focus();
        LogMessage($"Player focused! Current Mana: {player.GetCurrentMana()}/{player.data.maxManaPoints}");
        playerManaText.text = $"MP: {player.GetCurrentMana()}/{player.data.maxManaPoints}";

        yield return new WaitForSeconds(0.75f);

        StartCoroutine(EnemyTurn());
    }

    public void Defend()
    {
        if (state != BattleState.PlayerTurn)
            return;

        StartCoroutine(DefendRoutine());
    }

    private IEnumerator DefendRoutine()
    {
        state = BattleState.Busy;
        SetPlayerControls(false);

        player.Defend();

        yield return new WaitForSeconds(0.75f);

        StartCoroutine(EnemyTurn());
    }

    private IEnumerator PlayerAttackRoutine()
    {
        state = BattleState.Busy;
        SetPlayerControls(false);

        Vector3 attackPosition = enemy.transform.position + Vector3.left * attackDistance;
        yield return StartCoroutine(PlayerMoveToPosition(attackPosition,false));
        player.StopRun();
        player.PlayAttack();
        playerManaText.text = $"MP: {player.GetCurrentMana()}/{player.data.maxManaPoints}";
        yield return new WaitForSeconds(0.5f);
        enemy.TakeDamage(player.data.attack);
        enemyHealthText.text = $"HP: {enemy.GetCurrentHP()}/{enemy.data.maxHealth}";
        if (enemy.GetCurrentHP() <= 0)
        {
            StartCoroutine(HandleEnemyDeath());
            yield break;
        }
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(PlayerMoveToPosition(playerStartPosition,true));
        player.StopRun();
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(EnemyTurn());
    }

    private IEnumerator EnemyTurn()
    {
        state = BattleState.EnemyTurn;
        UpdateTurnBanner(false);
        yield return new WaitForSeconds(1f);

        if (enemy.GetCurrentMana() < enemy.data.specialAttackCost && Random.value < 0.6f)
        {
            yield return StartCoroutine(EnemyFocusRoutine());
        }
        else if (enemy.GetCurrentMana() >= enemy.data.specialAttackCost && Random.value < 0.7f)
        {
            yield return StartCoroutine(EnemySpecialRoutine());
        }
        else
        {
            yield return StartCoroutine(EnemyBasicAttackRoutine());
        }

        if (player.GetCurrentHealth() <= 0)
        {
            StartCoroutine(HandlePlayerDeath());
        }
        else
        {
            player.SetDefending(false);
            state = BattleState.PlayerTurn;
            SetPlayerControls(true);
            UpdateTurnBanner(true);
        }
    }

    private IEnumerator EnemyBasicAttackRoutine()
    {
        LogMessage($"⚔️ {enemy.data.enemyName} uses a Basic Attack!");
        Vector3 enemyAttackPosition = player.transform.position + Vector3.right * (attackDistance);
        yield return StartCoroutine(EnemyMoveToPosition(enemyAttackPosition, false));
        
        enemy.StopRun();
        enemy.PlayAttack();
        yield return new WaitForSeconds(0.5f);
        
        player.TakeDamage(enemy.data.attack);
        playerHealthText.text = $"HP: {player.GetCurrentHealth()}/{player.data.maxHealth}";
        
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(EnemyMoveToPosition(startEnemyPosition, true));
        enemy.StopRun();
        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator EnemySpecialRoutine()
    {
        LogMessage($"🔥 {enemy.data.enemyName} casts a Special Spell!");
        
        enemy.SpendMana(enemy.data.specialAttackCost);
        
        enemy.PlayAttack(); // Change this when make new Animation for special attack
        
        yield return new WaitForSeconds(0.5f);
        
        player.TakeDamage(enemy.data.specialAttackDamage);
        playerHealthText.text = $"HP: {player.GetCurrentHealth()}/{player.data.maxHealth}";
        
        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator EnemyFocusRoutine()
    {
        enemy.Focus();
        yield return new WaitForSeconds(0.75f);
    }

    private void SetPlayerControls(bool enabled)
    {
        Button[] buttons = attackPanel.GetComponentsInChildren<Button>();
        foreach (Button btn in buttons)
        {
            btn.interactable = enabled;
        }
    }

    private IEnumerator PlayerMoveToPosition(Vector3 target, bool flipPlayer)
    {
        target.y = player.transform.position.y;

        if (flipPlayer)
        {
            if (target.x > player.transform.position.x)
                player.transform.localScale = new Vector3(1, 1, 1);
            else
                player.transform.localScale = new Vector3(-1, 1, 1);
        }

        while (Mathf.Abs(player.transform.position.x - target.x) > 0.05f)
        {
            Vector3 newPosition = Vector3.MoveTowards(
                player.transform.position,
                new Vector3(target.x, player.transform.position.y, player.transform.position.z),
                moveSpeed * Time.deltaTime
            );

            player.transform.position = newPosition;

            player.PlayRun();

            yield return null;
        }

        player.transform.position = new Vector3(
            target.x,
            player.transform.position.y,
            player.transform.position.z
        );

        if (flipPlayer)
        {
            player.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    private IEnumerator EnemyMoveToPosition(Vector3 target, bool flipEnemy)
    {
        target.y = enemy.transform.position.y;

        if (flipEnemy)
        {
            if (target.x > enemy.transform.position.x)
                enemy.transform.localScale = new Vector3(1, 1, 1);
            else
                enemy.transform.localScale = new Vector3(-1, 1, 1);
        }

        while (Mathf.Abs(enemy.transform.position.x - target.x) > 0.05f)
        {
            Vector3 newPosition = Vector3.MoveTowards(
                enemy.transform.position,
                new Vector3(target.x, enemy.transform.position.y, enemy.transform.position.z),
                moveSpeed * Time.deltaTime
            );

            enemy.transform.position = newPosition;

            enemy.PlayRun();

            yield return null;
        }

        enemy.transform.position = new Vector3(
            target.x,
            enemy.transform.position.y,
            enemy.transform.position.z
        );

        if (flipEnemy)
        {
            enemy.transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private IEnumerator HandleEnemyDeath()
    {
        state = BattleState.Won;
        SetPlayerControls(false);
        if (turnBannerImage != null) turnBannerImage.gameObject.SetActive(false);
        LogMessage("Player wins!");
        GameManager.Instance.DefeatEnemy(GameManager.Instance.CurrentEnemyID);

        bool leveledUp = GameManager.Instance.GainXP(enemy.data.xpReward);
        GameManager.Instance.GainGold(enemy.data.goldReward);

        levelUpPanel.SetActive(true);

        if (leveledUp)
        {
            levelUpTitleText.text = $"Level Up! You are now Level {GameManager.Instance.PlayerData.currentLevel}!";
            levelUpStatsText.text = $"Gained {enemy.data.xpReward} XP & {enemy.data.goldReward} Gold!\n\n" +
                                    $"Max HP: {GameManager.Instance.PlayerData.maxHealth}\n" +
                                    $"Attack: {GameManager.Instance.PlayerData.attack}\n" +
                                    $"Defense: {GameManager.Instance.PlayerData.defense}";
            yield return new WaitForSeconds(3.5f); 
        }
        else
        {
            levelUpTitleText.text = "Victory!";
            levelUpStatsText.text = $"Gained {enemy.data.xpReward} XP\n" +
                                    $"Gained {enemy.data.goldReward} Gold";
            yield return new WaitForSeconds(2.5f);
        }
        SceneTransition.instance.LoadScene("Tutorial Scene");
    }

    private void UpdateTurnBanner(bool isPlayerTurn)
    {
        if (turnBannerImage == null) return;
        
        turnBannerImage.gameObject.SetActive(true);

        if (isPlayerTurn)
        {
            turnBannerImage.sprite = playerTurnSprite;
            turnBannerImage.transform.localScale = new Vector3(3f, 3f, 1f);
        }
        else
        {
            turnBannerImage.sprite = enemyTurnSprite;
            turnBannerImage.transform.localScale = new Vector3(3f, 2f, 1f);
        }
    }

    private IEnumerator HandlePlayerDeath()
    {
        state = BattleState.Lost;
        SetPlayerControls(false);
        if (turnBannerImage != null) turnBannerImage.gameObject.SetActive(false);
        
        LogMessage("💀 Player has fallen! Waiting for animation...");

        if (combatLogScrollRect != null) combatLogScrollRect.enabled = false;

        yield return new WaitForSeconds(2f);

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    public void RetryBattle()
    {
        Debug.Log("🔄 Retrying battle from the start!");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerData.currentHealth = GameManager.Instance.PlayerData.maxHealth;
            GameManager.Instance.PlayerData.currentManaPoints = GameManager.Instance.PlayerData.maxManaPoints;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToCheckpoint()
    {
        Debug.Log("🏃 Retreating to the latest checkpoint!");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerData.currentHealth = GameManager.Instance.PlayerData.maxHealth;
        }

        SceneTransition.instance.LoadScene("Tutorial Scene");
    }

    public void LogMessage(string message)
    {
        Debug.Log(message);

        if (logContentParent != null && logTextPrefab != null)
        {
            GameObject newLog = Instantiate(logTextPrefab, logContentParent);
            TextMeshProUGUI logText = newLog.GetComponent<TextMeshProUGUI>();
            if (logText != null)
            {
                logText.text = message;
            }

            StartCoroutine(ScrollToBottomRoutine());
        }
    }

    private IEnumerator ScrollToBottomRoutine()
    {
        yield return null; 

        if (combatLogScrollRect != null)
        {
            combatLogScrollRect.verticalNormalizedPosition = 0f;
        }
    }
}

public enum BattleState
{
    PlayerTurn,
    EnemyTurn,
    Busy,
    Won,
    Lost
}