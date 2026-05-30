using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class MazeLevelManager : MonoBehaviour
{
    public static MazeLevelManager instance;

    // =====================================================
    // SERIALIZABLE NPC ENTRY
    // =====================================================

    [System.Serializable]
    public class NPCButtonEntry
    {
        [Tooltip("Label shown on this button")]
        public string buttonLabel = "Option";

        [Tooltip("Confidence change: positive = add, negative = remove")]
        public int confidenceChange = 5;

        [Tooltip("What the player says when they pick this option (shown in cloud briefly)")]
        [TextArea(1, 2)]
        public string playerReply = "";

        [Tooltip("NPC's reaction after the player picks this option")]
        [TextArea(1, 2)]
        public string npcReaction = "";
    }

    [System.Serializable]
    public class NPCEntry
    {
        [Header("NPC Cloud")]
        public GameObject cloud;
        public TMP_Text cloudText;

        [Header("Player Reply Cloud (assign a separate speech bubble for the player)")]
        public GameObject playerCloud;
        public TMP_Text playerCloudText;

        [TextArea(1, 3)]
        [Tooltip("Opening line NPC says when player approaches")]
        public string dialogue = "";

        [Header("Popup Description")]
        [TextArea(1, 3)]
        [Tooltip("Context text shown in the popup panel below the clouds")]
        public string popupDescription = "";

        [Header("Buttons (add up to 3)")]
        public List<NPCButtonEntry> buttons = new List<NPCButtonEntry>();
    }

    // =====================================================
    // INSPECTOR FIELDS
    // =====================================================

    [Header("Confidence")]
    public Slider confidenceSlider;
    public TMP_Text confidenceText;
    [Range(0, 100)]
    public int confidence = 24;

    [Header("Timer")]
    public TMP_Text timerText;
    public float startTime = 60f;

    [Header("Popup Panel")]
    public GameObject popupPanel;
    public TMP_Text popupText;

    [Header("Beggar NPCs")]
    public List<NPCEntry> beggarNPCs = new List<NPCEntry>();

    [Header("Worker NPCs")]
    public List<NPCEntry> workerNPCs = new List<NPCEntry>();

    [Header("Bullies NPCs")]
    public List<NPCEntry> bulliesNPCs = new List<NPCEntry>();

    [Header("Popup Buttons (shared UI, text set dynamically)")]
    public Button button1;
    public Button button2;
    public Button button3;
    public TMP_Text button1Text;
    public TMP_Text button2Text;
    public TMP_Text button3Text;

    [Header("Win / Fail Panels")]
    public GameObject winPanel;
    public GameObject winPopupBox;
    public GameObject failPanel;
    public GameObject failPopupBox;
    public float popupAnimTime = 0.3f;

    [Header("Intro Character Dialogue")]
    public GameObject girlCharacter;
    public GameObject girlCloud;
    public TMP_Text girlCloudText;
    public GameObject dialogueBackground;
    public float typingSpeed = 0.03f;

    [Header("Scene")]
    public string nextSceneName;
    public string homeSceneName = "Home";

    // =====================================================
    // PRIVATE STATE
    // =====================================================

    private float timer;
    private float negativePenaltyTimer;
    private bool timerActive = false;

    private System.Action action1;
    private System.Action action2;
    private System.Action action3;

    private bool gameOver = false;
    private bool waitingForTap;
    private bool isTyping;
    private bool skipTyping;

    private NPCEntry activeNPCEntry = null;
    private Coroutine npcConversationCoroutine = null;

    // =====================================================
    // UNITY LIFECYCLE
    // =====================================================

    void Awake() { instance = this; }

    void Start()
    {
        timer = startTime;

        HideAllNPCClouds();

        if (girlCharacter != null) girlCharacter.SetActive(false);
        if (girlCloud != null) girlCloud.SetActive(false);
        if (dialogueBackground != null) dialogueBackground.SetActive(false);
        if (popupPanel != null) popupPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
        if (failPanel != null) failPanel.SetActive(false);

        HideAllButtons();
        UpdateConfidence();
        StartCoroutine(PlayIntroDialogue());
    }

    void Update()
    {
        if (!gameOver && timerActive)
            HandleTimer();

        if (Input.GetMouseButtonDown(0))
        {
            if (isTyping) skipTyping = true;
            else waitingForTap = false;
        }
    }

    // =====================================================
    // INTRO DIALOGUE
    // =====================================================

    IEnumerator PlayIntroDialogue()
    {
        if (girlCharacter != null) girlCharacter.SetActive(true);

        yield return SayGirl("Today is the day. I need to reach the office!");
        yield return SayGirl("I must stay confident no matter what comes my way.");
        yield return SayGirl("Let's go. The office won't wait for me.");

        if (girlCharacter != null) girlCharacter.SetActive(false);
        if (girlCloud != null) girlCloud.SetActive(false);
        if (dialogueBackground != null) dialogueBackground.SetActive(false);

        ShowIntro();
    }

    IEnumerator SayGirl(string message)
    {
        yield return StartCoroutine(ShowDialogue(girlCloud, girlCloudText, message));
    }

    IEnumerator ShowDialogue(GameObject cloud, TMP_Text textUI, string message)
    {
        HideAllNPCClouds();

        if (dialogueBackground != null) dialogueBackground.SetActive(true);
        if (cloud != null) cloud.SetActive(true);
        if (textUI != null) textUI.text = "";

        isTyping = true;
        skipTyping = false;

        foreach (char c in message)
        {
            if (skipTyping) { if (textUI != null) textUI.text = message; break; }
            if (textUI != null) textUI.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        waitingForTap = true;
        while (waitingForTap) yield return null;

        if (cloud != null) cloud.SetActive(false);
        if (dialogueBackground != null) dialogueBackground.SetActive(false);
    }

    // =====================================================
    // TIMER
    // =====================================================

    void HandleTimer()
    {
        timer -= Time.deltaTime;
        if (timerText == null) return;

        if (timer >= 0)
        {
            timerText.color = Color.white;
            timerText.text = Mathf.CeilToInt(timer).ToString();
        }
        else
        {
            timerText.color = Color.red;
            timerText.text = "-" + Mathf.Abs(Mathf.FloorToInt(timer));

            negativePenaltyTimer += Time.deltaTime;
            if (negativePenaltyTimer >= 5f)
            {
                negativePenaltyTimer = 0f;
                RemoveConfidence(2);
            }
        }
    }

    // =====================================================
    // CONFIDENCE
    // =====================================================

    public void AddConfidence(int amount)
    {
        confidence += amount;
        if (confidence > 100) confidence = 100;
        UpdateConfidence();
    }

    public void RemoveConfidence(int amount)
    {
        confidence -= amount;
        if (confidence < 0) confidence = 0;
        UpdateConfidence();
        if (confidence <= 0) ShowFailPanel();
    }

    void UpdateConfidence()
    {
        if (confidenceSlider != null) confidenceSlider.value = confidence;
        if (confidenceText != null) confidenceText.text = confidence + "%";
    }

    // =====================================================
    // CLOUD HELPERS
    // =====================================================

    void HideAllNPCClouds()
    {
        HideEntryList(beggarNPCs);
        HideEntryList(workerNPCs);
        HideEntryList(bulliesNPCs);
        if (girlCloud != null) girlCloud.SetActive(false);
    }

    void HideEntryList(List<NPCEntry> list)
    {
        foreach (var e in list)
        {
            if (e.cloud != null) e.cloud.SetActive(false);
            if (e.playerCloud != null) e.playerCloud.SetActive(false);
        }
    }

    void ShowNPCCloud(NPCEntry entry)
    {
        HideAllNPCClouds();
        activeNPCEntry = entry;
        if (entry.cloud != null) entry.cloud.SetActive(true);
        if (entry.cloudText != null) entry.cloudText.text = entry.dialogue;
    }

    public void HideActiveNPCCloud()
    {
        if (activeNPCEntry == null) return;
        if (activeNPCEntry.cloud != null) activeNPCEntry.cloud.SetActive(false);
        if (activeNPCEntry.playerCloud != null) activeNPCEntry.playerCloud.SetActive(false);
        activeNPCEntry = null;
        if (popupPanel != null) popupPanel.SetActive(false);
        HideAllButtons();
    }

    // =====================================================
    // BUTTON HELPERS
    // =====================================================

    void HideAllButtons()
    {
        if (button1 != null) button1.gameObject.SetActive(false);
        if (button2 != null) button2.gameObject.SetActive(false);
        if (button3 != null) button3.gameObject.SetActive(false);
    }

    /// <summary>
    /// Builds up to 3 buttons from an NPCEntry's button list.
    /// Each button: shows player reply cloud → NPC reaction → applies confidence change → closes popup.
    /// </summary>
    void BuildNPCButtons(NPCEntry entry)
    {
        HideAllButtons();

        var btns = new[] { button1, button2, button3 };
        var texts = new[] { button1Text, button2Text, button3Text };
        var acts = new System.Action[3];

        for (int i = 0; i < 3; i++)
        {
            if (i < entry.buttons.Count)
            {
                int capturedIndex = i; // capture loop variable
                NPCButtonEntry btnEntry = entry.buttons[i];

                acts[i] = () =>
                {
                    // Disable buttons immediately so player can't click twice
                    HideAllButtons();
                    // Start the reply → reaction → result conversation
                    if (npcConversationCoroutine != null)
                        StopCoroutine(npcConversationCoroutine);
                    npcConversationCoroutine = StartCoroutine(
                        PlayChoiceConversation(entry, btnEntry));
                };

                btns[i].gameObject.SetActive(true);
                texts[i].text = btnEntry.buttonLabel;
            }
            else
            {
                btns[i].gameObject.SetActive(false);
            }
        }

        action1 = acts[0];
        action2 = acts[1];
        action3 = acts[2];
    }

    public void Choice1() { action1?.Invoke(); }
    public void Choice2() { action2?.Invoke(); }
    public void Choice3() { action3?.Invoke(); }

    // =====================================================
    // CHOICE CONVERSATION FLOW
    // =====================================================

    /// <summary>
    /// After the player picks a button:
    /// 1. Show player's reply in their cloud (typewriter).
    /// 2. Show NPC's reaction in NPC cloud (typewriter).
    /// 3. Apply confidence change.
    /// 4. Close everything.
    /// </summary>
    IEnumerator PlayChoiceConversation(NPCEntry entry, NPCButtonEntry chosen)
    {
        // --- Step 1: Player reply ---
        if (!string.IsNullOrEmpty(chosen.playerReply) && entry.playerCloud != null)
        {
            // Hide NPC cloud while player speaks
            if (entry.cloud != null) entry.cloud.SetActive(false);

            entry.playerCloud.SetActive(true);
            if (entry.playerCloudText != null)
                yield return StartCoroutine(TypeInCloud(entry.playerCloudText, chosen.playerReply));

            yield return new WaitForSeconds(0.3f);
        }

        // --- Step 2: NPC reaction ---
        if (!string.IsNullOrEmpty(chosen.npcReaction))
        {
            // Hide player cloud, show NPC cloud
            if (entry.playerCloud != null) entry.playerCloud.SetActive(false);
            if (entry.cloud != null) entry.cloud.SetActive(true);

            if (entry.cloudText != null)
                yield return StartCoroutine(TypeInCloud(entry.cloudText, chosen.npcReaction));

            yield return new WaitForSeconds(0.5f);
        }

        // --- Step 3: Apply confidence ---
        int change = chosen.confidenceChange;
        if (change >= 0) AddConfidence(change);
        else RemoveConfidence(-change);

        // --- Step 4: Close ---
        HideActiveNPCCloud();
    }

    /// <summary>
    /// Types text into a TMP field with typewriter effect.
    /// Tap / click skips to full text.
    /// Waits for another tap before returning.
    /// </summary>
    IEnumerator TypeInCloud(TMP_Text textUI, string message)
    {
        textUI.text = "";
        isTyping = true;
        skipTyping = false;

        foreach (char c in message)
        {
            if (skipTyping) { textUI.text = message; break; }
            textUI.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        waitingForTap = true;
        while (waitingForTap) yield return null;
    }

    // =====================================================
    // NPC POPUP  (called by NPCProximityTrigger)
    // =====================================================

    public void ShowNPCPopup(string npcType, int index)
    {
        List<NPCEntry> list = GetList(npcType);
        if (list == null || index < 0 || index >= list.Count)
        {
            Debug.LogWarning($"ShowNPCPopup: no entry for {npcType}[{index}]");
            return;
        }

        NPCEntry entry = list[index];

        // Stop any existing conversation
        if (npcConversationCoroutine != null)
            StopCoroutine(npcConversationCoroutine);

        npcConversationCoroutine = StartCoroutine(PlayNPCOpeningThenChoices(entry));
    }

    public void HideNPCPopup(string npcType, int index)
    {
        List<NPCEntry> list = GetList(npcType);
        if (list == null || index < 0 || index >= list.Count) return;

        if (activeNPCEntry == list[index])
        {
            // Stop conversation coroutine if running
            if (npcConversationCoroutine != null)
            {
                StopCoroutine(npcConversationCoroutine);
                npcConversationCoroutine = null;
            }
            HideActiveNPCCloud();
        }
    }

    /// <summary>
    /// Full NPC encounter flow:
    /// 1. Show NPC cloud with opening dialogue (typewriter, wait for tap).
    /// 2. Show popup panel with description + 3 choice buttons.
    /// </summary>
    IEnumerator PlayNPCOpeningThenChoices(NPCEntry entry)
    {
        // Set as active entry right away
        HideAllNPCClouds();
        activeNPCEntry = entry;

        // Show NPC speech bubble
        if (entry.cloud != null) entry.cloud.SetActive(true);
        if (entry.cloudText != null)
            yield return StartCoroutine(TypeInCloud(entry.cloudText, entry.dialogue));

        // After the player taps through the opening line, reveal the choice panel
        if (popupPanel != null) popupPanel.SetActive(true);
        if (popupText != null) popupText.text = entry.popupDescription;

        BuildNPCButtons(entry);
    }

    List<NPCEntry> GetList(string npcType)
    {
        switch (npcType)
        {
            case "Beggar": return beggarNPCs;
            case "Worker": return workerNPCs;
            case "Bullies": return bulliesNPCs;
            default: return null;
        }
    }

    // =====================================================
    // INTRO POPUP
    // =====================================================

    public void ShowIntro()
    {
        if (popupPanel == null) return;

        popupPanel.SetActive(true);
        popupText.text = "Reach the office before time runs out!";

        button1.gameObject.SetActive(true);
        button2.gameObject.SetActive(false);
        button3.gameObject.SetActive(false);
        button1Text.text = "Start";

        action1 = () =>
        {
            popupPanel.SetActive(false);
            HideAllButtons();
            timerActive = true;
        };
    }

    // =====================================================
    // OFFICE TRIGGER
    // =====================================================

    public void ReachedOffice()
    {
        popupPanel.SetActive(true);
        popupText.text = "You made it to the office!";

        button1.gameObject.SetActive(true);
        button2.gameObject.SetActive(false);
        button3.gameObject.SetActive(false);
        button1Text.text = "Continue";

        action1 = () => { popupPanel.SetActive(false); ShowWinPanel(); };
    }

    // =====================================================
    // WIN / FAIL PANELS
    // =====================================================

    public void ShowWinPanel()
    {
        if (gameOver) return;
        gameOver = true;

        if (winPanel != null) winPanel.SetActive(true);
        if (winPopupBox != null)
        {
            winPopupBox.transform.localScale = Vector3.one;
            iTween.ScaleFrom(winPopupBox, iTween.Hash(
                "scale", Vector3.zero, "time", popupAnimTime,
                "easetype", iTween.EaseType.easeOutBack, "ignoretimescale", true));
        }
        StartCoroutine(PauseAfterDelay(popupAnimTime));
    }

    public void ShowFailPanel()
    {
        if (gameOver) return;
        gameOver = true;

        if (failPanel != null) failPanel.SetActive(true);
        if (failPopupBox != null)
        {
            failPopupBox.transform.localScale = Vector3.one;
            iTween.ScaleFrom(failPopupBox, iTween.Hash(
                "scale", Vector3.zero, "time", popupAnimTime,
                "easetype", iTween.EaseType.easeOutBack, "ignoretimescale", true));
        }
        StartCoroutine(PauseAfterDelay(popupAnimTime));
    }

    IEnumerator PauseAfterDelay(float delay)
    {
        float t = 0f;
        while (t < delay) { t += Time.unscaledDeltaTime; yield return null; }
        Time.timeScale = 0f;
    }

    // =====================================================
    // SCENE BUTTONS
    // =====================================================

    public void Retry() { Time.timeScale = 1f; SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }
    public void NextLevel() { Time.timeScale = 1f; SceneManager.LoadScene(nextSceneName); }
    public void MainMenu() { Time.timeScale = 1f; SceneManager.LoadScene(homeSceneName); }
}