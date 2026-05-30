using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class LevelSelector : MonoBehaviour
{
    public Button[] levelButtons; // Assign level buttons in Inspector
    //public Button playButton;

    private void Start()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1); //--> At the start check how many levels are unlock

        // Enable only unlocked levels
        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (i + 1 <= unlockedLevel)
                levelButtons[i].interactable = true;
            else
                levelButtons[i].interactable = false;
        }

        //playButton.onClick.AddListener(PlayLatestLevel);
    }

    //public void PlayLatestLevel()
    //{
    //    int latestLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
    //    SceneManager.LoadScene("Level" + latestLevel);
    //}

    public void SelectLevel(int levelIndex)
    {
        SceneManager.LoadScene(levelIndex);
    }

}
