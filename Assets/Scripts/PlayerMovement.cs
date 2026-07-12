using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float runSpeed = 10f;
    [SerializeField] float jumpSpeed = 5f;

    CapsuleCollider2D myBodyCollider;
    Vector2 moveInput;
    Rigidbody2D myRigitBody2D;
    Animator myAnimator;

    bool isGrounded;
    bool isTouchingWall;

    void Start()
    {
        myRigitBody2D = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();

        if (GameManager.Instance != null && GameManager.Instance.ShouldRestorePosition)
        {
           if (SceneManager.GetActiveScene().name == "Tutorial Scene") 
           {
               transform.position = GameManager.Instance.LastPlayerPosition;
               GameManager.Instance.ShouldRestorePosition = false;
           }
        }
    }

    void Update()
    {
        CheckGround();
        CheckWall();
        Run();
        FlipSprite();
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        if (!value.isPressed) return;
        if (isGrounded)
        {
            myRigitBody2D.velocity = new Vector2(myRigitBody2D.velocity.x, jumpSpeed);
        }
    }

    void CheckGround()
    {
        isGrounded = myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Platform"));
    }

    void CheckWall()
    {
        Vector2 direction = new Vector2(transform.localScale.x, 0f);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 0.2f, LayerMask.GetMask("Platform"));
        isTouchingWall = hit.collider != null;
    }

    void Run()
    {
        if (isTouchingWall && Mathf.Sign(moveInput.x) == Mathf.Sign(transform.localScale.x))
        {
            myRigitBody2D.velocity = new Vector2(0f, myRigitBody2D.velocity.y);
            myAnimator.SetBool("isRunning", false);
            return;
        }

        Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, myRigitBody2D.velocity.y);
        myRigitBody2D.velocity = playerVelocity;
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigitBody2D.velocity.x) > Mathf.Epsilon;
        myAnimator.SetBool("isRunning", playerHasHorizontalSpeed);
    }

    void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigitBody2D.velocity.x) > Mathf.Epsilon;
        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(myRigitBody2D.velocity.x), 1f);
        }
    }
}