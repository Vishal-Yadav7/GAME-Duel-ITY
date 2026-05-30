using UnityEngine;
using System.Collections;

public class DoorPuzzle : MonoBehaviour
{
    public GameObject door; // Assign the Door GameObject in the Inspector
    public GameObject[] buttons; // Assign all buttons (Open Door, Turn On Lights, Emergency Exit)

    private bool isDoorLocked = false; // If true, the door is locked
    private bool buttonsDisabled = false; // Track if buttons are temporarily disabled
    private Animator doorAnimator; // Optional for animation

    void Start()
    {
        if (door != null)
        {
            doorAnimator = door.GetComponent<Animator>(); // Get Animator if available
        }
    }

    public void PressButton(string buttonType)
    {
        if (buttonsDisabled) return; // Prevent input if buttons are disabled

        switch (buttonType)
        {
            case "OpenDoor":
                isDoorLocked = true; // Locks the door (wrong choice)
                Debug.Log("The door is now locked!");
                break;

            case "TurnOnLights":
                if (!isDoorLocked)
                {
                    OpenDoor();
                    Debug.Log("The door opens!");
                }
                else
                {
                    Debug.Log("The door is locked! Find another way.");
                }
                break;

            case "EmergencyExit":
                StartCoroutine(DisableButtonsTemporarily(10f));
                Debug.Log("All buttons are disabled for 10 seconds!");
                break;
        }
    }

    void OpenDoor()
    {
        door.SetActive(false); // Hide door when "opened"
        if (doorAnimator) doorAnimator.SetBool("isOpen", true);
    }

    IEnumerator DisableButtonsTemporarily(float duration)
    {
        buttonsDisabled = true;
        foreach (GameObject button in buttons)
        {
            button.SetActive(false); // Hide buttons
        }
        yield return new WaitForSeconds(duration);
        foreach (GameObject button in buttons)
        {
            button.SetActive(true); // Show buttons again
        }
        buttonsDisabled = false;
    }
}
