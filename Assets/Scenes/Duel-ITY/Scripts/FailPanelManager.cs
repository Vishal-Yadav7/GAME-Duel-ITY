using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FailPanelManager : MonoBehaviour
{
    [Header("Fail Panel")]
    public GameObject failPanel;

    [Header("Popup Box")]
    public GameObject popupBox;

    [Header("Texts")]
    public Text titleText;

    public Text subtitleText;

    [Header("Animation")]
    public float popupTime = 0.3f;

    void Start()
    {
        // HIDE PANEL
        if (failPanel != null)
        {
            failPanel.SetActive(false);
        }
    }

    // SHOW FAIL PANEL
    public void ShowFailPanel()
    {
        // SHOW PANEL
        if (failPanel != null)
        {
            failPanel.SetActive(true);
        }

        // TITLE
        if (titleText != null)
        {
            titleText.text =
                "You Lost Confidence...";
        }

        // SUBTITLE
        if (subtitleText != null)
        {
            subtitleText.text =
                "The pressure became too much.\nTry again.";
        }

        // POPUP EFFECT
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

        // FAIL SOUND
        if (AudioManager.instance != null)
        {
            AudioManager.instance.Play(
                "Fail"
            );
        }

        // STOP GAME
        Time.timeScale = 0f;
    }

    // RETRY BUTTON
    public void RetryLevel()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager
            .GetActiveScene()
            .buildIndex
        );
    }

    // MAIN MENU BUTTON
    public void MainMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            "Duel-ITY"
        );
    }
}