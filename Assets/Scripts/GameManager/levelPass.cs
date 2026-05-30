using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levelPass : MonoBehaviour
{
    public DoorController doorController;
    // Start is called before the first frame update
    void Start()
    {
        doorController = GetComponentInParent<DoorController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && doorController.isOpen)
        {
            Debug.Log("Level Completed");
            GameManager.instance.gameComplete.SetActive(true);
            GameManager.instance.CompleteLevel(SceneManagerLoader.instance.getCurrentSceneIndex());
            AudioManager.instance.Play("levelPass");
        }
        else
        {
            Debug.Log("Pls Unlock The door");
        }
    }
}
