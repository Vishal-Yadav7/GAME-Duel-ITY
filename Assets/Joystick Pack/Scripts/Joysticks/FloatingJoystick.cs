using UnityEngine;
using UnityEngine.EventSystems;

public class FloatingJoystick : Joystick
{
    private Vector2 startPos;
    private bool hasRegisteredFirstTouch;

    protected override void Start()
    {
        base.Start();
        startPos = background.anchoredPosition;
        background.gameObject.SetActive(true);
        handle.anchoredPosition = Vector2.zero;

        // Load saved value: 1 = first touch already done before
        hasRegisteredFirstTouch = PlayerPrefs.GetInt("JoystickFirstTouch", 0) == 1;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        // If this is the first touch ever → save it
        if (!hasRegisteredFirstTouch)
        {
            hasRegisteredFirstTouch = true;
            PlayerPrefs.SetInt("JoystickFirstTouch", 1);
            PlayerPrefs.Save();
        }

        // Move base to touch position
        background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);

        // Ensure base visible
        background.gameObject.SetActive(true);

        base.OnPointerDown(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        handle.anchoredPosition = Vector2.zero;
        background.anchoredPosition = startPos;

        base.OnPointerUp(eventData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background,
            eventData.position,
            eventData.pressEventCamera,
            out pos
        );

        float radius = background.sizeDelta.x / 2f;
        pos = Vector2.ClampMagnitude(pos, radius);

        // Move handle
        handle.anchoredPosition = pos;

        // Normalize input
        input = new Vector2(pos.x / radius, pos.y / radius);
    }
}
