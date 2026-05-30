using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 movement;

    [Header("Animation")]
    public Animator playerAnim;

    [Header("Mobile Joystick Support")]
    public Joystick joystick; // Attach joystick in Inspector
    public bool useJoystick = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerAnim = GetComponent<Animator>();
    }

    void Update()
    {
        // MOBILE INPUT
        if (useJoystick && joystick != null)
        {
            movement.x = joystick.Horizontal;
            movement.y = joystick.Vertical;
        }
        // PC INPUT
        else
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");
        }

        // Normalize movement
        movement = movement.normalized;

        // ANIMATION
        HandleAnimation();
    }

    void FixedUpdate()
    {
        // PLAYER MOVEMENT
        rb.linearVelocity = movement.normalized * moveSpeed;
    }

    void HandleAnimation()
    {
        bool isMoving = movement.magnitude > 0.1f;

        playerAnim.SetBool("IsMoving", isMoving);

        if (!isMoving)
            return;

        // Horizontal priority
        if (Mathf.Abs(movement.x) > Mathf.Abs(movement.y))
        {
            // LEFT / RIGHT
            playerAnim.SetFloat("MoveX", movement.x > 0 ? 1 : -1);
            playerAnim.SetFloat("MoveY", 0);
        }
        else
        {
            // UP / DOWN
            playerAnim.SetFloat("MoveX", 0);
            playerAnim.SetFloat("MoveY", movement.y > 0 ? 1 : -1);
        }
    }
}