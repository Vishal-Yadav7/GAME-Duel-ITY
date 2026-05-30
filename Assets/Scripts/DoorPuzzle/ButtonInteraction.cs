using UnityEngine;

public class ButtonInteraction : MonoBehaviour
{
    public DoorPuzzle puzzleManager;
    public string buttonType; // Set this in Inspector: "OpenDoor", "TurnOnLights", "EmergencyExit"

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            puzzleManager.PressButton(buttonType);
        }
    }
}
