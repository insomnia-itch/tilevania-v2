using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] float moveSpeed = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        rb.velocity = new Vector2(moveSpeed, 0f);
    }

    void FlipSprite() {
        float currentSign = Mathf.Sign(rb.velocity.x);
        float currentXScale = Mathf.Abs(transform.localScale.x);
        transform.localScale = new Vector2(-currentSign * currentXScale, transform.localScale.y);
    }

    void OnTriggerExit2D(Collider2D other) {
        moveSpeed = -moveSpeed;
        FlipSprite();
    }
}
