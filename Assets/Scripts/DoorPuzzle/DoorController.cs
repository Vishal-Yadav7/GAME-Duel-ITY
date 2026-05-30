using UnityEngine;

public class DoorController : MonoBehaviour
{
    public ButtonTrigger[] buttons; // Assign all button objects in the Inspector
    public GameObject door; // Assign the door GameObject (or use transform.position to move it)
    public bool isOpen = false;

    private void Update()
    {
        if (AllButtonsPressed())
        {
            OpenDoor();
        }
    }

    private bool AllButtonsPressed()
    {
        foreach (ButtonTrigger button in buttons)
        {
            if (!button.IsPressed())
            {
                return false; // If any button is not pressed correctly, return false
            }
        }
        return true; // All buttons are pressed correctly
    }

    private void OpenDoor()
    {
        door.SetActive(false); // Example: Disabling the door (or move it upwards)
        isOpen = true;
    }

}
