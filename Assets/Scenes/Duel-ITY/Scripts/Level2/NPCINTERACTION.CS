
using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public enum NPCType
    {
        Beggar,
        Bully,
        Worker
    }

    public NPCType npcType;

    bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("TRIGGER ENTER");

        if (triggered)
            return;

        if (other.CompareTag("Player"))
        {
            Debug.Log("PLAYER DETECTED");

            triggered = true;

            MazeGameManager gm =
                FindObjectOfType<MazeGameManager>();

            if (npcType == NPCType.Beggar)
            {
                gm.OpenBeggar();
            }

            else if (npcType == NPCType.Bully)
            {
                gm.OpenBully();
            }

            else if (npcType == NPCType.Worker)
            {
                gm.OpenWorker();
            }
        }
    }
}
