using UnityEngine;

public class Pmove : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb;

    private Vector2 movement;

    [Header("Animation")]
    public Animator playerAnim;

    [Header("Joystick")]
    public Joystick joystick;

    public bool useJoystick = true;

    void Start()
    {
        rb =
            GetComponent<Rigidbody2D>();

        playerAnim =
            GetComponent<Animator>();
    }

    void Update()
    {
        // MOBILE INPUT
        if (useJoystick &&
            joystick != null)
        {
            movement.x =
                joystick.Horizontal;

            movement.y =
                joystick.Vertical;
        }
        // PC INPUT
        else
        {
            movement.x =
                Input.GetAxisRaw(
                    "Horizontal"
                );

            movement.y =
                Input.GetAxisRaw(
                    "Vertical"
                );
        }

        // SMOOTH NORMALIZE
        movement =
            movement.normalized;

        // ANIMATIONS
        HandleAnimation();
    }

    void FixedUpdate()
    {
        rb.linearVelocity =
            movement * moveSpeed;
    }

    void HandleAnimation()
    {
        // SPEED
        playerAnim.SetFloat(
            "Speed",
            movement.sqrMagnitude
        );

        // DIRECTION
        playerAnim.SetFloat(
            "MoveX",
            movement.x
        );

        playerAnim.SetFloat(
            "MoveY",
            movement.y
        );
    }
}