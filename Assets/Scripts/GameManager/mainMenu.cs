using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenu : MonoBehaviour
{

    public void play()
    {
        SceneManager.LoadScene(1);
    }
    public void quitGame()
    {
        Application.Quit();
    }
    public void resetGame()
    {
        // 🗑️ Clear all saved PlayerPrefs data
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        Debug.Log("🔄 Game reset: All saved data cleared.");

        // Reload back to main menu (scene index 0)
        SceneManager.LoadScene(0);
    }
}
