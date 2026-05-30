using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowManager : MonoBehaviour
{
    [Header("Scene Names")]

    // MAIN MENU SCENE
    public string mainMenuScene = "Duel-ITY";

    // CUTSCENE SCENE
    public string cutsceneScene = "D-CutScene";

    // FIRST LEVEL SCENE
    public string level1Scene = "LVL1";

    // START GAME BUTTON
    public void StartGame()
    {
        // CHECK IF CUTSCENE ALREADY PLAYED
        if (PlayerPrefs.GetInt("CutscenePlayed", 0) == 0)
        {
            // FIRST TIME → PLAY CUTSCENE
            SceneManager.LoadScene(cutsceneScene);
        }
        else
        {
            // CUTSCENE ALREADY PLAYED
            SceneManager.LoadScene(level1Scene);
        }
    }

    // CALLED AT CUTSCENE END
    public void CutsceneFinished()
    {
        // SAVE CUTSCENE STATE
        PlayerPrefs.SetInt("CutscenePlayed", 1);

        PlayerPrefs.Save();

        // LOAD LEVEL 1
        SceneManager.LoadScene(level1Scene);
    }

    // LOAD NEXT LEVEL
    public void LoadNextLevel(string nextScene)
    {
        SceneManager.LoadScene(nextScene);
    }

    // RESTART CURRENT LEVEL
    public void RestartLevel()
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
        );
    }

    // BACK TO MENU
    public void BackToMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }

    // RESET SAVE DATA
    public void ResetGameData()
    {
        PlayerPrefs.DeleteAll();

        Debug.Log("Game Data Cleared");
    }

    // QUIT GAME
    public void QuitGame()
    {
        Application.Quit();
    }
}