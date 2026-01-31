using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class Diver : MonoBehaviour
{
    public float speed = 5f;

    Rigidbody2D rb;
    Vector2 input;
    Animator anim;

    const float moveEpsilon = 0.01f; // threshold

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        anim = GetComponent<Animator>();
    }
s
    void Update()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
        if (input.sqrMagnitude > 1f) input.Normalize();
    }

    void FixedUpdate()
    {
        // Unity versions differ: use linearVelocity if available, otherwise velocity.
        rb.linearVelocity = input * speed;

        bool isMoving = rb.linearVelocity.sqrMagnitude > moveEpsilon;
        anim.SetBool("IsMoving", isMoving);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Spike"))
        {
            Object.FindObjectOfType<GameManager>().TriggerGameOver("spike");
        }

        if (collision.gameObject.CompareTag("Finish"))
        {
            Object.FindObjectOfType<GameManager>().TriggerWin();
        }
    }
}
