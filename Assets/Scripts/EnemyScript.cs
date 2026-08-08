using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    Rigidbody2D myRigidBody;

    [SerializeField] float moveSpeed;
    [SerializeField] private EnemyData enemyData;

    [Header("Enemy Identity")]
    [SerializeField] private string enemyID;
    [SerializeField] private bool isBoss;

    private bool triggered = false;

    void Start()
    {
        if (GameManager.Instance.DefeatedEnemies.Contains(enemyID))
        {
            Destroy(gameObject);
            return;
        }

        myRigidBody = GetComponent<Rigidbody2D>();
        GameManager.Instance.RegisterEnemy(enemyID, isBoss);
    }

    void FixedUpdate()
    {
        myRigidBody.linearVelocity = new Vector2(moveSpeed, 0);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        moveSpeed = -moveSpeed;
        FlipEnemy();
    }

    private void FlipEnemy()
    {
        transform.localScale =
            new Vector2(Mathf.Sign(myRigidBody.linearVelocity.x), 1f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;

            GameManager.Instance.SavePlayerPosition(other.transform.position);

            GameManager.Instance.SetCurrentEnemy(enemyData);
            GameManager.Instance.CurrentEnemyID = enemyID;
            GameManager.Instance.CurrentEnemyIsBoss = isBoss;

            SceneTransition.instance.LoadScene("Combat Scene");
        }
    }

    public bool IsBoss()
    {
        return isBoss;
    }

    public string GetEnemyID()
    {
        return enemyID;
    }
}