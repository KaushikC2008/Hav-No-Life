using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
   Rigidbody2D myRigidBody;
   [SerializeField] float moveSpeed;
   [SerializeField] private EnemyData enemyData;
   
   public string enemyID;
   private bool triggered = false;

   void Start()
   {
       if (GameManager.Instance.DefeatedEnemies.Contains(enemyID))
        {
            Destroy(gameObject);
            return;
        }

        myRigidBody = GetComponent<Rigidbody2D>();
   }

   void FixedUpdate()
   {
       myRigidBody.velocity = new Vector2(moveSpeed, 0);
   }

   private void OnTriggerExit2D(Collider2D other)
   {
       moveSpeed = -moveSpeed;
       FlipEnemy();
   }

   private void FlipEnemy()
   {
       transform.localScale = new Vector2(Mathf.Sign(myRigidBody.velocity.x), 1f);
   }

   private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        
        if (other.CompareTag("Player"))
        {
            triggered = true;

            GameManager.Instance.LastPlayerPosition = other.transform.position;
            GameManager.Instance.ShouldRestorePosition = true;

            GameManager.Instance.SetCurrentEnemy(enemyData);
            GameManager.Instance.CurrentEnemyID = enemyID;
            SceneTransition.instance.LoadScene("Combat Scene");
        }
    }
}