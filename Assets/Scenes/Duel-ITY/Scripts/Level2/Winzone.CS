using UnityEngine;

public class WinZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Entered: " + other.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("WIN!");

            MazeGameManager gm =
                FindFirstObjectByType<MazeGameManager>();

            if (gm != null)
            {
                gm.WinGame();
            }
        }
    }
}