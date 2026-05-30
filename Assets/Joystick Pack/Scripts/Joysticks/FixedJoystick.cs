using UnityEngine;
using UnityEngine.EventSystems;
// If your FixedJoystick is inside a namespace like "JoystickPack", uncomment this:
// using JoystickPack;

public class ScreenJoystick : Joystick
{
    private Vector2 inputVector;
    private bool isTouching = false;
    private Vector3 defaultPosition;

    protected override void Start()
    {
        base.Start();
        defaultPosition = background.position; // Store original joystick position
    }

    void Update()
    {
        if (!isTouching && base.Direction == Vector2.zero)
        {
            // Keyboard fallback (WASD / Arrow keys)
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            inputVector = new Vector2(h, v).normalized;
        }
        else
        {
            inputVector = base.Direction;
        }
    }

    // When the finger touches
    public new void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.position.x <= Screen.width / 2) // left half of screen
        {
            isTouching = true;
            background.position = eventData.position; // Move joystick background
            base.OnPointerDown(eventData);
        }
    }

    // While dragging
    public new void OnDrag(PointerEventData eventData)
    {
        if (isTouching)
        {
            base.OnDrag(eventData);
        }
    }

    // When the finger lifts
    public new void OnPointerUp(PointerEventData eventData)
    {
        if (isTouching)
        {
            isTouching = false;
            background.position = defaultPosition; // Reset joystick
            base.OnPointerUp(eventData);
        }
    }

    // Public getters for movement scripts
    public float GetHorizontal() => inputVector.x;
    public float GetVertical() => inputVector.y;
    public Vector2 GetDirection() => inputVector;
}
