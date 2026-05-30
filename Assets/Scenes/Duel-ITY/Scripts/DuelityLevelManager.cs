using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DuelityLevelManager : MonoBehaviour
{
    [Header("Dialogue Manager")]
    public DialogueManager dialogueManager;



    [Header("Gameplay UI")]
    public TMP_Text timerText;

    public TMP_Text taskText;

    [Header("Confidence")]
    public Slider confidenceSlider;

    public TMP_Text confidenceText;

    [Header("Inventory")]
    public GameObject inventoryPanel;

    public GameObject inventoryBackground;

    [Header("Inventory Slots")]
    public Image[] inventorySlots;

    [Header("Player")]
    public GameObject player;

    [Header("Joystick")]
    public GameObject joystickUI;

    [Header("Map Button")]
    public GameObject zoomOutButton;

    [Header("Camera")]
    public CameraFollowZoom cameraController;

    [Header("Popups")]
    public GameObject failPopup;

    public GameObject winPopup;

    public GameObject correctPopup;

    public GameObject wrongPopup;

    [Header("Next Level")]
    public string nextSceneName;

    [Header("Correct Objects")]
    public List<GameObject> correctObjects;

    [Header("Wrong Objects")]
    public List<GameObject> wrongObjects;

    private List<GameObject> selectedObjects =
        new List<GameObject>();

    [Header("Timer")]
    public float levelTime = 60f;

    private float currentTime;

    private bool timerRunning = false;

    private bool levelCompleted = false;

    // ONLY CORRECT OBJECTS COUNT
    private int completedTasks = 0;

    // START FROM 0
    private int confidence = 0;

    void Start()
    {
        currentTime = levelTime;

        // SLIDER SETTINGS
        confidenceSlider.minValue = 0;

        confidenceSlider.maxValue = 100;

        confidenceSlider.value = 0;

        // HIDE POPUPS
        failPopup.SetActive(false);

        winPopup.SetActive(false);

        if (correctPopup != null)
        {
            correctPopup.SetActive(false);
        }

        if (wrongPopup != null)
        {
            wrongPopup.SetActive(false);
        }

        if (winPopup == null)
            Debug.LogError("winPopup not assigned in Inspector!");
        if (dialogueManager == null)
            Debug.LogError("DialogueManager not assigned in Inspector!");

        // HIDE MAP BUTTON
        if (zoomOutButton != null)
        {
            zoomOutButton.SetActive(false);
        }

        // RESET INVENTORY
        foreach (Image slot in inventorySlots)
        {
            slot.sprite = null;

            slot.color =
                new Color(1, 1, 1, 0);
        }

        // UPDATE UI
        UpdateConfidenceUI();

        // HIDE GAMEPLAY
        HideGameplayUI();

        // RESET CORRECT OBJECTS
        foreach (GameObject obj
            in correctObjects)
        {
            if (obj != null)
            {
                ClickableObject click =
                    obj.GetComponent
                    <ClickableObject>();

                if (click != null)
                {
                    click.ResetObject();
                }
            }
        }

        // RESET WRONG OBJECTS
        foreach (GameObject obj
            in wrongObjects)
        {
            if (obj != null)
            {
                ClickableObject click =
                    obj.GetComponent
                    <ClickableObject>();

                if (click != null)
                {
                    click.ResetObject();
                }
            }
        }

        taskText.text =
            "Task : <color=#e3affd>0/5</color>";

        StartCoroutine(GameFlow());
    }

    void Update()
    {
        if (timerRunning)
        {
            currentTime -= Time.deltaTime;

            if (currentTime < 0)
            {
                currentTime = 0;
            }

            timerText.text =
                "Timer : <color=#e3affd>" +
                Mathf.Ceil(currentTime) +
                "</color>";

            // FAIL
            if (currentTime <= 0)
            {
                timerRunning = false;

                HideGameplayUI();

                failPopup.SetActive(true);
            }
        }
    }

    IEnumerator GameFlow()
    {
        // HIDE GAMEPLAY
        HideGameplayUI();

        yield return dialogueManager.ShowBoy(
            "Good morning..."
        );

        yield return dialogueManager.ShowGirl(
            "Morning. I already prepared breakfast."
        );

        yield return dialogueManager.ShowBoy(
            "I'm getting late for office."
        );

        yield return dialogueManager.ShowGirl(
            "What do you need now?"
        );

        // RANDOM TASKS
        SelectRandomObjects();

        // SHOW CORRECT OBJECTS
        foreach (GameObject obj
            in selectedObjects)
        {
            ClickableObject click =
                obj.GetComponent
                <ClickableObject>();

            if (click != null)
            {
                click.ShowObject();
            }
        }

        // SHOW WRONG OBJECTS
        foreach (GameObject obj
            in wrongObjects)
        {
            ClickableObject click =
                obj.GetComponent
                <ClickableObject>();

            if (click != null)
            {
                click.ShowObject();
            }
        }

        // SET TASK OBJECTS
        for (int i = 0;
             i < selectedObjects.Count;
             i++)
        {
            ClickableObject click =
                selectedObjects[i]
                .GetComponent
                <ClickableObject>();

            if (click != null)
            {
                click.isTaskObject = true;
            }
        }

        // TASK LIST
        string allTaskText =
            "Can you help me to Find:\n";

        for (int i = 0;
             i < selectedObjects.Count;
             i++)
        {
            allTaskText +=
                selectedObjects[i].name;

            // COMMA
            if (i < selectedObjects.Count - 1)
            {
                allTaskText += ", ";
            }

            // NEW LINE AFTER 2 ITEMS
            if ((i + 1) % 2 == 0)
            {
                allTaskText += "\n";
            }
        }

        // SHOW TASK DIALOGUE
        yield return dialogueManager.ShowBoy(
            allTaskText
        );

        // HIDE CHARACTERS
        dialogueManager.EndConversation();

        // SHOW GAMEPLAY
        ShowGameplayUI();

        // START CAMERA
        if (cameraController != null)
        {
            cameraController.StartGameplayCamera();
        }

        // SHOW MAP BUTTON
        if (zoomOutButton != null)
        {
            zoomOutButton.SetActive(true);
        }

        // START TIMER
        timerRunning = true;
    }

    // RANDOM TASKS
    void SelectRandomObjects()
    {
        selectedObjects.Clear();

        List<int> usedIndexes =
            new List<int>();

        int taskAmount =
            Mathf.Min(
                5,
                correctObjects.Count
            );

        while (selectedObjects.Count <
               taskAmount)
        {
            int randomIndex =
                Random.Range(
                    0,
                    correctObjects.Count
                );

            if (!usedIndexes.Contains(
                randomIndex))
            {
                usedIndexes.Add(
                    randomIndex
                );

                selectedObjects.Add(
                    correctObjects[randomIndex]
                );
            }
        }
    }

    // CORRECT OBJECT
    public void CorrectObjectFound(
        ClickableObject obj
    )
    {
        // ALREADY COMPLETE
        if (levelCompleted)
            return;

        // ONLY CORRECT OBJECTS COUNT
        if (!obj.isTaskObject)
            return;

        // CONFIDENCE +
        confidence += Random.Range(1, 5);

        if (confidence > 100)
        {
            confidence = 100;
        }

        UpdateConfidenceUI();

        // TASK COUNT
        completedTasks++;

        taskText.text =
            "Task : <color=#e3affd>" +
            completedTasks +
            "/" +
            selectedObjects.Count +
            "</color>";

        // INVENTORY
        int slotIndex =
            obj.inventorySlotIndex;

        if (slotIndex <
            inventorySlots.Length)
        {
            Image slot =
                inventorySlots[slotIndex];

            Image icon =
                obj.inventoryIcon
                .GetComponent<Image>();

            slot.sprite =
                icon.sprite;

            slot.color =
                Color.white;
        }

        // CORRECT POPUP
        if (correctPopup != null)
        {
            StartCoroutine(
                ShowPopup(correctPopup)
            );
        }

        // COMPLETE LEVEL
        if (completedTasks >=
            selectedObjects.Count)
        {
            levelCompleted = true;

            StartCoroutine(
                CompleteLevel()
            );
        }
    }

    // WRONG OBJECT
    public void WrongObjectClicked(
        ClickableObject obj
    )
    {
        // IMPORTANT:
        // DO NOT HIDE OBJECT HERE

        // CONFIDENCE -
        confidence -= Random.Range(1, 5);

        if (confidence < 0)
        {
            confidence = 0;
        }

        UpdateConfidenceUI();

        // WRONG POPUP
        if (wrongPopup != null)
        {
            StartCoroutine(
                ShowPopup(wrongPopup)
            );
        }

        // NO TASK COUNT
        // NO INVENTORY
        // NO LEVEL COMPLETE
    }

    // COMPLETE LEVEL
    IEnumerator CompleteLevel()
    {
        Debug.Log("CompleteLevel START");

        yield return new WaitForSeconds(1f);

        Debug.Log("After 1s wait");

        timerRunning = false;

        HideGameplayUI();

        Debug.Log("HideGameplayUI done");

        if (dialogueManager != null)
        {
            Debug.Log("Starting ShowBoyAuto");

            yield return StartCoroutine(
                dialogueManager.ShowBoyAuto(
                    "Thank you. You found everything.", 2f
                )
            );

            Debug.Log("ShowBoyAuto finished");

            dialogueManager.EndConversation();
        }

        yield return new WaitForSeconds(0.3f);

        Debug.Log("About to show winPopup");

        if (winPopup != null)
        {
            // MOVE TO CANVAS ROOT SO NOTHING CAN HIDE IT
            Canvas rootCanvas = FindObjectOfType<Canvas>();
            if (rootCanvas != null)
            {
                winPopup.transform.SetParent(
                    rootCanvas.transform, false
                );
            }

            winPopup.transform.SetAsLastSibling();
            winPopup.SetActive(true);
            Debug.Log("WIN POPUP SET ACTIVE TRUE");
        }
    }

    // UPDATE CONFIDENCE
    void UpdateConfidenceUI()
    {
        if (confidenceSlider != null)
        {
            confidenceSlider.value =
                confidence;
        }

        if (confidenceText != null)
        {
            confidenceText.text =
                confidence + "%";
        }
    }

    // POPUP EFFECT
    IEnumerator ShowPopup(
        GameObject popup
    )
    {
        popup.SetActive(true);

        yield return new WaitForSeconds(1f);

        popup.SetActive(false);
    }

    // HIDE UI
    void HideGameplayUI()
    {
        timerText.gameObject
            .SetActive(false);

        taskText.gameObject
            .SetActive(false);

        confidenceSlider.gameObject
            .SetActive(false);

        confidenceText.gameObject
            .SetActive(false);

        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }

        if (inventoryBackground != null)
        {
            inventoryBackground.SetActive(false);
        }

        if (player != null)
        {
            player.SetActive(false);
        }

        if (joystickUI != null)
        {
            joystickUI.SetActive(false);
        }

        if (zoomOutButton != null)
        {
            zoomOutButton.SetActive(false);
        }
    }

    // SHOW UI
    void ShowGameplayUI()
    {
        timerText.gameObject
            .SetActive(true);

        taskText.gameObject
            .SetActive(true);

        confidenceSlider.gameObject
            .SetActive(true);

        confidenceText.gameObject
            .SetActive(true);

        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(true);
        }

        if (inventoryBackground != null)
        {
            inventoryBackground.SetActive(true);
        }

        if (player != null)
        {
            player.SetActive(true);
        }

        if (joystickUI != null)
        {
            joystickUI.SetActive(true);
        }

        if (zoomOutButton != null)
        {
            zoomOutButton.SetActive(true);
        }
    }

    // RESTART
    public void RestartLevel()
    {
        SceneManager.LoadScene(
            SceneManager
            .GetActiveScene()
            .buildIndex
        );
    }

    // NEXT LEVEL
    public void LoadNextLevel()
    {
        SceneManager.LoadScene(
            nextSceneName
        );
    }
}