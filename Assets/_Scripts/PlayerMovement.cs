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

    // for dashing 
    Vector2 savedVelocity;
    bool canDash = true;

    [SerializeField] float runSpeed = 10f;
    [SerializeField] float jumpSpeed = 17.5f;
    [SerializeField] float dashSpeed = 2f;
    [SerializeField] float dashCoolDown = 0f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] Vector2 deathKick = new Vector2(20f, 20f);
    [SerializeField] GameObject bullet;
    [SerializeField] Transform gun;

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

    void OnFire(InputValue value) {
        if (!isAlive)
            return;
        Instantiate(bullet, gun.position, bullet.transform.rotation);
    }

    void OnJump(InputValue value) {
        LayerMask groundLayer = LayerMask.GetMask("Ground");
        if (!feetCollider.IsTouchingLayers(groundLayer))
            return;
        if (value.isPressed && !anim.GetBool("isDashing"))
        {
            rb.velocity += new Vector2(0f, jumpSpeed);
            anim.SetBool("isJumping", true);
            StartCoroutine(Jump());
        }
    }

    void OnDash(InputValue value) {
        bool s = anim.GetBool("isDashing");
        if (dashCoolDown > 0)
        {
            canDash = false;
            // dashCoolDown -= Time.deltaTime;
        }
        else
        {
            anim.SetBool("isDashing", false);
            canDash = true;
        }
        // Debug.Log(value.isPressed);
        // Debug.Log(dashCoolDown);
        // Debug.Log(canDash);
        if (canDash && value.isPressed)
        {
            Debug.Log("Should trigger DASh");
            anim.SetBool("isDashing", true);
            // dashCoolDown = 1.5f;
            savedVelocity = rb.velocity;
            Debug.Log(savedVelocity);
            float sign = 0;
            if (transform.localScale.x > 0)
            {
                sign = 1;
            } else
            {
                sign = -1;
            }
            moveInput = new Vector2(sign * 1.5f, rb.velocity.y);
            // Debug.Log(rb.velocity);
            dashCoolDown = 0f;
            StartCoroutine(Dash());
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }
        // if (value.isPressed) {
        //     Debug.Log(rb.velocity);
        //     float min = moveInput.x;
        //     if (min == 0)
        //         min = 1f;
        //     Vector2 playerVelocity = new Vector2(min * dashSpeed, rb.velocity.y);
        //     rb.velocity = playerVelocity;
        //     Debug.Log(rb.velocity);
        //     anim.SetBool("isDashing", true);
        //     // Dash();
        //     StartCoroutine(Dash());
        // }
        
        // rb.velocity =  new Vector2(rb.velocity.x*3f, rb.velocity.y);

    }

    void Run() {
        Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, rb.velocity.y);
        rb.velocity = playerVelocity;
        Debug.Log(moveInput.x);
        Debug.Log(anim.GetBool("isRunning"));
        Debug.Log("_____2");
        bool moving = Mathf.Abs(rb.velocity.x) > Mathf.Epsilon && !anim.GetBool("isDashing");
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
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
    }

    IEnumerator Jump() {
        yield return new WaitForSecondsRealtime(0.5f);
        anim.SetBool("isJumping", false);
    }

    IEnumerator Dash() {
        yield return new WaitForSecondsRealtime(0.25f);
        anim.SetBool("isDashing", false);
        rb.velocity = new Vector2(0f, rb.velocity.y);
        Debug.Log("AFTER DASH v:");
        Debug.Log(rb.velocity);
        Debug.Log(anim.GetBool("isRunning"));
        Debug.Log("_____");
        moveInput.x = 0f;
    }
}
