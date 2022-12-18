using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Vector2 moveInput;
    Rigidbody2D rb;
    Animator anim;
    [SerializeField] float runSpeed = 10f;
    [SerializeField] float jumpSpeed = 17.5f;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        Run();
        FlipSprite();
    }

    void OnMove(InputValue value) {
        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value) {
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
}
