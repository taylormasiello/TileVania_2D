using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float runSpeed = 10F;

    Vector2 moveInput; // Vector2 is movement on X&Y axis, Vector3 is movement on X,Y,&Z axis
    Rigidbody2D myRigidbody;
    Animator myAnimator;

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
    }


    void Update()
    {
        Run();
        FlipSprite();
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
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
}
