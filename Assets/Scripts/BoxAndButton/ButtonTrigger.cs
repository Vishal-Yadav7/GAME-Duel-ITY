using UnityEngine;

public class ButtonTrigger : MonoBehaviour
{
    public string requiredName;  // The correct box name that should be placed here
    private bool isPressed = false; // To check if this button is correctly activated

    private void OnCollisionEnter2D(Collision2D collision)
    {

        SetName box = collision.gameObject.GetComponent<SetName>();

        if (box != null)
        {
            Debug.Log("Found SetName component on: " + box.objectName);
        }

        if (box != null && box.objectName == requiredName)
        {
            isPressed = true;
            Debug.Log("Correct box placed on " + gameObject.name);
        }
    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        SetName box = collision.gameObject.GetComponent<SetName>();

        if (box != null && box.objectName == requiredName)
        {
            isPressed = false;
            Debug.Log("Box removed from " + gameObject.name);
        }
    }

    public bool IsPressed()
    {
        return isPressed;
    }
}
