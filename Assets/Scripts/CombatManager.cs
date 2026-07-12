using UnityEngine;
using TMPro;
using System.Collections;
using Unity.VisualScripting;

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

    private BattleState state;

    void Start()
    {
        player.Setup();
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
        enemyNameText.text = data.enemyName;
        enemyHealthText.text = $"HP: {data.maxHealth}/{data.maxHealth}";
        playerHealthText.text = $"HP: {player.GetCurrentHealth()}/{player.data.maxHealth}";
        playerManaText.text = $"MP: {player.GetCurrentMana()}/{player.data.maxManaPoints}";
        startEnemyPosition = data.combatPosition;
        attackDistance = data.playerCombatOffset.x;


        if (enemyWorldSpriteRenderer != null && data.combatSprite != null)
        {
            enemyWorldSpriteRenderer.sprite = data.combatSprite;
            enemyWorldSpriteRenderer.flipX = data.flipSprite;
            enemyWorldSpriteRenderer.transform.position = data.combatPosition;
        }
        state = BattleState.PlayerTurn;
    }

    public void BasicAttack()
    {
        if (state != BattleState.PlayerTurn)
            return;
        
        if (isAttacking)
            return;
        StartCoroutine(PlayerAttackRoutine());
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
            state = BattleState.Won;
            SetPlayerControls(false);
            Debug.Log("Player wins!");
            GameManager.Instance.DefeatEnemy(GameManager.Instance.CurrentEnemyID);

            bool leveledUp = GameManager.Instance.GainXP(enemy.data.xpReward);
            GameManager.Instance.GainGold(enemy.data.goldReward);

            levelUpPanel.SetActive(true);

            if (leveledUp)
            {
                levelUpPanel.SetActive(true);
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
        yield return new WaitForSeconds(1f);
        Vector3 enemyAttackPosition = player.transform.position + Vector3.right * (attackDistance);
        yield return StartCoroutine(EnemyMoveToPosition(enemyAttackPosition,false));
        enemy.StopRun();
        enemy.PlayAttack();
        yield return new WaitForSeconds(0.5f);
        player.TakeDamage(enemy.data.attack);
        playerHealthText.text = $"HP: {player.GetCurrentHealth()}/{player.data.maxHealth}";
        if (player.GetCurrentHealth() <= 0)
        {
            state = BattleState.Lost;
            SetPlayerControls(false);
            Debug.Log("Player lost!");
            yield break;
        }
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(EnemyMoveToPosition(startEnemyPosition,true));
        enemy.StopRun();
        yield return new WaitForSeconds(0.5f);
        state = BattleState.PlayerTurn;
        SetPlayerControls(true);
    }

    private void SetPlayerControls(bool enabled)
    {
        attackPanel.SetActive(enabled);
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

}

public enum BattleState
{
    PlayerTurn,
    EnemyTurn,
    Busy,
    Won,
    Lost
}