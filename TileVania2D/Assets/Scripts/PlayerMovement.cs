using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float runSpeed = 10F;
    [SerializeField] float jumpSpeed = 5F;
    [SerializeField] float climbSpeed = 5F;

    Vector2 moveInput; // Vector2 is movement on X&Y axis, Vector3 is movement on X,Y,&Z axis
    Rigidbody2D myRigidbody;
    Animator myAnimator;
    CapsuleCollider2D myBodyCollider;
    BoxCollider2D myFeetCollider;
    float gravityScaleAtStart;

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        gravityScaleAtStart = myRigidbody.gravityScale;
    }


    void Update()
    {
        Run();
        FlipSprite();
        ClimbLadder();
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value) 
    {
        if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))) { return; } // escape from OnJump() if player collider notTouching ground layerMask collider

        if(value.isPressed)
        {
            myRigidbody.velocity += new Vector2 (0F, jumpSpeed);
        }
    }

    void Run()
    {
        // instead of setting y axis to 0F (fights against gravity, causes bug), use myRigidbody.velocity.y; 
        // "whatever current velocity is on y axis (includes gravity), keep that value"
        Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, myRigidbody.velocity.y);
        myRigidbody.velocity = playerVelocity;

        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        myAnimator.SetBool("isRunning", playerHasHorizontalSpeed);

    }

    void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        // this prevents Mathf.Sign "0 is positive" bug when player has 0 x velocity (see if block below)
        // Mathf.Epsilon is better than setting to 0 as value here is never quite 0
        // Mathf.Abs gives absolute value of param (movement to left is negative movement, need to get abs val before comparing to "0")

        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(myRigidbody.velocity.x), 1f);
            // Mathf.Sign returns sign of F; return value is 1 when F is positive or "0", -1 when F is negative
            // applies this returned 1 or -1 to transform.localScale, x axis
        }
    }

    void ClimbLadder()
    {
        if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"))) // can't climb if not touching a ladder
        {
            myRigidbody.gravityScale = gravityScaleAtStart;
            myAnimator.SetBool("isClimbing", false);
            return; 
        }
        
        Vector2 climbVelocity = new Vector2(myRigidbody.velocity.x, moveInput.y * climbSpeed);
        myRigidbody.velocity = climbVelocity;
        myRigidbody.gravityScale = 0F;

        bool playerHasVerticalSpeed = Mathf.Abs(myRigidbody.velocity.y) > Mathf.Epsilon;
        myAnimator.SetBool("isClimbing", playerHasVerticalSpeed);
    }
}
