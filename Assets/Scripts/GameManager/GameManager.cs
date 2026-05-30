using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    public GameObject gameComplete;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    private void Start()
    {
        gameComplete.SetActive(false);
    }

    public void homeLoad()
    {
        SceneManagerLoader.instance.LoadGameOnPlay(0);
    }
    
    public void LoadNewLevel()
    {
        SceneManagerLoader.instance.LoadNextLevel();
    }

    // --> This funtion call in the levelPass.cs
    public void CompleteLevel(int levelIndex)
    {
        int currentUnlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        if (levelIndex >= currentUnlockedLevel)
        {
            PlayerPrefs.SetInt("UnlockedLevel", levelIndex);
            PlayerPrefs.Save();
        }

        Debug.Log("Unlocked Level: " + PlayerPrefs.GetInt("UnlockedLevel"));
    }
}
