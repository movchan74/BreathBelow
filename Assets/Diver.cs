using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class Diver : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 200f;

    Rigidbody2D rb;
    Vector2 input;
    float rotationInput;
    Animator anim;

    public float moveEpsilon = 0.01f;
    public float rotationEpsilon = 0.0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
        if (input.sqrMagnitude > 1f) input.Normalize();

        rotationInput = 0f;
        if (Input.GetKey(KeyCode.Q)) rotationInput += 1f;
        if (Input.GetKey(KeyCode.E)) rotationInput -= 1f;
    }

    void FixedUpdate()
    {
        // World-space movement: always same directions on screen
        rb.linearVelocity = input * speed;

        // Rotation is independent (aim/look/roll)
        rb.MoveRotation(rb.rotation + rotationInput * rotationSpeed * Time.fixedDeltaTime);

        bool isMoving = rb.linearVelocity.sqrMagnitude > moveEpsilon || Mathf.Abs(rotationInput) > rotationEpsilon;
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

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Finish"))
        {
            Object.FindObjectOfType<GameManager>().TriggerWin();
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("AirPocket"))
        {
            Object.FindObjectOfType<GameManager>().TriggerAirPocket();
        }
    }
}
