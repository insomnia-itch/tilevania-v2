using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Vector2 moveInput;
    Rigidbody2D rb;
    Animator anim;
    float startingGravity;
    bool isAlive = true;
    CapsuleCollider2D bodyCollider;
    BoxCollider2D feetCollider;

    [SerializeField] float runSpeed = 10f;
    [SerializeField] float jumpSpeed = 17.5f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] Vector2 deathKick = new Vector2(20f, 20f);
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        bodyCollider = GetComponent<CapsuleCollider2D>();
        feetCollider = GetComponent<BoxCollider2D>();
        startingGravity = rb.gravityScale;
    }

    void Update()
    {
        if (!isAlive)
            return;
        Run();
        FlipSprite();
        ClimbLadder();
        Die();
    }

    void OnMove(InputValue value) {
        if (!isAlive)
            return;
        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value) {
        LayerMask groundLayer = LayerMask.GetMask("Ground");
        if (!feetCollider.IsTouchingLayers(groundLayer))
            return;
        if (value.isPressed)
        {
            rb.velocity += new Vector2(0f, jumpSpeed);   
        }
    }

    void Run() {
        Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, rb.velocity.y);
        rb.velocity = playerVelocity;

        bool moving = Mathf.Abs(rb.velocity.x) > Mathf.Epsilon;
        anim.SetBool("isRunning", moving);
    }

    void FlipSprite() {
        bool moving = Mathf.Abs(rb.velocity.x) > Mathf.Epsilon;
        if (moving)
            transform.localScale = new Vector2(Mathf.Sign(rb.velocity.x), 1f);
    }

    void ClimbLadder() {
       LayerMask climbingLayer = LayerMask.GetMask("Climbing");
        if (!feetCollider.IsTouchingLayers(climbingLayer)) {
            rb.gravityScale = startingGravity;
            anim.SetBool("isClimbing", false);
            return;
        }

        Vector2 climbVelocity = new Vector2(rb.velocity.x , moveInput.y * climbSpeed);
        rb.velocity = climbVelocity;
        rb.gravityScale = 0f;
        // TODO make one frame of climb if idle on ladder
        //  decide if you want horizontal movement on ladder to be possible
        bool climbing = Mathf.Abs(rb.velocity.y) > Mathf.Epsilon;
        anim.SetBool("isClimbing", climbing);
    }

    void Die() {
        LayerMask enemyLayer = LayerMask.GetMask("Enemy", "Hazards");
        if (bodyCollider.IsTouchingLayers(enemyLayer)) {
            isAlive = false;
            anim.SetTrigger("Dying");
            rb.velocity = deathKick;
        }
    }
}
