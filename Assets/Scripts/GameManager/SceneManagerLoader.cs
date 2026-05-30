using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneManagerLoader : MonoBehaviour
{
    public static SceneManagerLoader instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void LoadGameOnPlay(int n)
    {
        SceneManager.LoadScene(n);
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public int getCurrentSceneIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }

    // ✅ NEW: Load cutscene by name
    public void LoadCutscene(string cutsceneName)
    {
        SceneManager.LoadScene(cutsceneName);
    }
}
