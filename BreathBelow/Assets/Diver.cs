using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Diver : MonoBehaviour
{
    public float speed = 5f;

    Rigidbody2D rb;
    Vector2 input;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f; // diver in water (disable gravity)
    }

    void Update()
    {
        // WASD / Arrow keys
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        if (input.sqrMagnitude > 1f) input.Normalize(); // no faster diagonals
    }

    void FixedUpdate()
    {
        rb.linearVelocity = input * speed; // if this errors, use rb.velocity
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
