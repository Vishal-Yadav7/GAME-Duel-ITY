using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinPanelManager : MonoBehaviour
{
    [Header("Win Panel")]
    public GameObject winPanel;

    [Header("Popup Box")]
    public GameObject popupBox;

    [Header("Texts")]
    public Text titleText;
    public Text subtitleText;

    [Header("Animation")]
    public float popupTime = 0.3f;

    [Header("UI To Hide")]
    public GameObject zoomOutButton;
    public GameObject inventoryButton;

    void Start()
    {
        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }
    }

    public void ShowWinPanel()
    {
        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }

        if (zoomOutButton != null)
        {
            zoomOutButton.SetActive(false);
        }

        if (inventoryButton != null)
        {
            inventoryButton.SetActive(false);
        }

        if (titleText != null)
        {
            titleText.text =
                "Another Role Awaits...";
        }

        if (subtitleText != null)
        {
            subtitleText.text =
                "After handling home responsibilities,\nshe steps toward her own goals.";
        }

        if (popupBox != null)
        {
            iTween.ScaleFrom(
                popupBox,
                iTween.Hash(
                    "scale",
                    Vector3.zero,
                    "time",
                    popupTime,
                    "easetype",
                    iTween.EaseType.easeOutBack
                )
            );
        }

        if (AudioManager.instance != null)
        {
            AudioManager.instance.Play("Win");
        }

        Time.timeScale = 0f;
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;

        int currentIndex =
            SceneManager.GetActiveScene().buildIndex;

        int nextIndex =
            currentIndex + 1;

        if (nextIndex <
            SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextIndex);
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(0);
    }
}