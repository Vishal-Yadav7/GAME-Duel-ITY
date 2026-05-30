using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MazeGameManager : MonoBehaviour
{
    // =====================================================
    // DIALOGUE BACKGROUND
    // =====================================================

    [Header("Dialogue Background")]
    public GameObject dialogueBackground;

    // =====================================================
    // GIRL
    // =====================================================

    [Header("Girl")]
    public GameObject girlCharacter;
    public GameObject girlCloud;
    public TMP_Text girlText;

    [Header("UI")]
    public GameObject zoomOutButton;

    // =====================================================
    // BEGGAR
    // =====================================================

    [Header("Beggar")]
    public GameObject beggerCharacter;
    public GameObject beggerCloud;
    public TMP_Text beggerText;
    public GameObject beggerMapObject;      // single beggar on map

    public Button beggerBtn1;
    public Button beggerBtn2;
    public Button beggerBtn3;
    public TMP_Text beggerBtn1Text;
    public TMP_Text beggerBtn2Text;
    public TMP_Text beggerBtn3Text;

    // =====================================================
    // BULLY
    // =====================================================

    [Header("Bully")]
    public GameObject bulliCharacter;
    public GameObject bulliCloud;
    public TMP_Text bulliText;
    public GameObject bulliMapObject;       // single bully on map

    public Button bulliBtn1;
    public Button bulliBtn2;
    public Button bulliBtn3;
    public TMP_Text bulliBtn1Text;
    public TMP_Text bulliBtn2Text;
    public TMP_Text bulliBtn3Text;

    // =====================================================
    // WORKER
    // =====================================================

    [Header("Worker")]
    public GameObject workerCharacter;
    public GameObject workerCloud;
    public TMP_Text workerText;
    public GameObject[] workerMapObjects;   // ← array: drag 2-3 worker map objects here

    public Button workerBtn1;
    public Button workerBtn2;
    public Button workerBtn3;
    public TMP_Text workerBtn1Text;
    public TMP_Text workerBtn2Text;
    public TMP_Text workerBtn3Text;

    // =====================================================
    // CONFIDENCE
    // =====================================================

    [Header("Confidence")]
    public Slider confidenceSlider;
    public TMP_Text confidenceText;
    int confidence = 24;

    // =====================================================
    // TIMER
    // =====================================================

    [Header("Timer")]
    public TMP_Text timerText;

    float timer = 60f;
    float penaltyTimer = 0f;

    // =====================================================
    // RESULT
    // =====================================================

    [Header("Result")]
    public GameObject winPopup;
    public GameObject failPopup;

    // =====================================================
    // STATE
    // =====================================================

    bool gameRunning = false;
    bool dialogueRunning = false;
    bool waitingForTap = false;
    bool buttonsVisible = false;   // blocks tap-advance while buttons are shown

    int pendingBeggarChoice = 0;
    int pendingBullyChoice = 0;
    int pendingWorkerChoice = 0;

    int workerMapIndex = 0;         // tracks which worker map object to remove next

    const float TYPE_SPEED = 0.05f;

    // =====================================================
    // START
    // =====================================================

    void Start()
    {
        confidenceSlider.maxValue = 100;
        UpdateConfidenceUI();

        dialogueBackground.SetActive(false);

        girlCharacter.SetActive(false);
        girlCloud.SetActive(false);
        zoomOutButton.SetActive(true);

        beggerCharacter.SetActive(false);
        beggerCloud.SetActive(false);

        bulliCharacter.SetActive(false);
        bulliCloud.SetActive(false);

        workerCharacter.SetActive(false);
        workerCloud.SetActive(false);

        winPopup.SetActive(false);
        failPopup.SetActive(false);

        HideAllButtons();

        StartCoroutine(GirlIntro());

        CameraFollowZoom cameraController =
    FindFirstObjectByType<CameraFollowZoom>();

        if (cameraController != null)
        {
            cameraController.StartGameplayCamera();
        }
    }

    // =====================================================
    // UPDATE
    // =====================================================

    void Update()
    {
        // Only consume tap for dialogue advance — NOT when buttons are visible
        if (waitingForTap && !buttonsVisible && Input.GetMouseButtonDown(0))
        {
            waitingForTap = false;
        }

        if (!gameRunning) return;

        timer -= Time.deltaTime;

        if (timer >= 0f)
        {
            timerText.color = Color.white;
            timerText.text = "Time : " + Mathf.CeilToInt(timer).ToString();
        }
        else
        {
            timerText.color = Color.red;
            timerText.text = "Time : -" + Mathf.Abs(Mathf.FloorToInt(timer)).ToString();

            penaltyTimer += Time.deltaTime;
            if (penaltyTimer >= 5f)
            {
                penaltyTimer = 0f;
                ChangeConfidence(-2);
            }
        }
    }

    // =====================================================
    // WAIT FOR TAP
    // =====================================================

    IEnumerator WaitForTap()
    {
        // Small buffer so the same click that ended typing doesn't immediately advance
        yield return new WaitForSeconds(0.15f);
        waitingForTap = true;
        yield return new WaitUntil(() => !waitingForTap);
    }

    // =====================================================
    // GIRL INTRO
    // =====================================================

    IEnumerator GirlIntro()
    {
        gameRunning = false;
        zoomOutButton.SetActive(false);

        dialogueBackground.SetActive(true);
        girlCharacter.SetActive(true);
        girlCloud.SetActive(true);
        AnimatePopup(girlCloud);

        yield return TypeText(girlText,
            "Today is the day. I need to reach the office!");
        yield return WaitForTap();

        AnimatePopup(girlCloud);
        yield return TypeText(girlText,
            "I must stay confident no matter what comes my way.");
        yield return WaitForTap();

        AnimatePopup(girlCloud);
        yield return TypeText(girlText,
            "Let's go. The office won't wait for me.");
        yield return WaitForTap();

        girlCharacter.SetActive(false);
        girlCloud.SetActive(false);
        dialogueBackground.SetActive(false);

        gameRunning = true;
    }

    // =====================================================
    // TYPEWRITER
    // =====================================================

    IEnumerator TypeText(TMP_Text textUI, string message)
    {
        textUI.text = "";
        foreach (char c in message)
        {
            textUI.text += c;
            yield return new WaitForSeconds(TYPE_SPEED);
        }
    }

    // =====================================================
    // POPUP ANIMATION
    // =====================================================

    void AnimatePopup(GameObject popup)
    {
        Vector3 originalScale = popup.transform.localScale;
        popup.transform.localScale = originalScale * 1.0f;

        iTween.ScaleTo(popup, iTween.Hash(
            "scale", originalScale,
            "time", 0.35f,
            "easetype", iTween.EaseType.easeOutBack
        ));
    }

    // =====================================================
    // HIDE ALL BUTTONS
    // =====================================================

    void HideAllButtons()
    {
        buttonsVisible = false;

        beggerBtn1.gameObject.SetActive(false);
        beggerBtn2.gameObject.SetActive(false);
        beggerBtn3.gameObject.SetActive(false);

        bulliBtn1.gameObject.SetActive(false);
        bulliBtn2.gameObject.SetActive(false);
        bulliBtn3.gameObject.SetActive(false);

        workerBtn1.gameObject.SetActive(false);
        workerBtn2.gameObject.SetActive(false);
        workerBtn3.gameObject.SetActive(false);
    }

    // =====================================================
    // SHOW BEGGAR BUTTONS
    // =====================================================

    void ShowBeggarButtons()
    {
        buttonsVisible = true;

        beggerBtn1.gameObject.SetActive(true);
        beggerBtn2.gameObject.SetActive(true);
        beggerBtn3.gameObject.SetActive(true);
    }

    // =====================================================
    // SHOW BULLY BUTTONS
    // =====================================================

    void ShowBullyButtons()
    {
        buttonsVisible = true;

        bulliBtn1.gameObject.SetActive(true);
        bulliBtn2.gameObject.SetActive(true);
        bulliBtn3.gameObject.SetActive(true);
    }

    // =====================================================
    // SHOW WORKER BUTTONS
    // =====================================================

    void ShowWorkerButtons()
    {
        buttonsVisible = true;

        workerBtn1.gameObject.SetActive(true);
        workerBtn2.gameObject.SetActive(true);
        workerBtn3.gameObject.SetActive(true);
    }

    // =====================================================
    // BEGGAR
    // =====================================================

    public void OpenBeggar()
    {
        if (dialogueRunning) return;
        StartCoroutine(BeggarConversation());
    }

    IEnumerator BeggarConversation()
    {
        dialogueRunning = true;
        gameRunning = false;
        zoomOutButton.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        dialogueBackground.SetActive(true);
        girlCharacter.SetActive(true);
        beggerCharacter.SetActive(true);

        // ── Beggar speaks ──
        girlCloud.SetActive(false);
        beggerCloud.SetActive(true);
        AnimatePopup(beggerCloud);

        yield return TypeText(beggerText,
            "Excuse me miss... I haven't eaten since yesterday. Could you spare something?");
        yield return WaitForTap();

        // ── Girl reacts ──
        beggerCloud.SetActive(false);
        girlCloud.SetActive(true);
        AnimatePopup(girlCloud);

        yield return TypeText(girlText,
            "Hmm... he really looks hungry. What should I do?");

        // Small pause so last typed character doesn't register as a button click
        yield return new WaitForSeconds(0.5f);

        // ── Set up buttons BEFORE showing them ──
        beggerBtn1Text.text = "I don't have time.";
        beggerBtn2Text.text = "Here, take this. Hope it helps.";
        beggerBtn3Text.text = "You can have my lunch.";

        pendingBeggarChoice = 0;

        beggerBtn1.onClick.RemoveAllListeners();
        beggerBtn2.onClick.RemoveAllListeners();
        beggerBtn3.onClick.RemoveAllListeners();

        beggerBtn1.onClick.AddListener(() => { pendingBeggarChoice = 1; });
        beggerBtn2.onClick.AddListener(() => { pendingBeggarChoice = 2; });
        beggerBtn3.onClick.AddListener(() => { pendingBeggarChoice = 3; });

        // ── Show buttons ──
        ShowBeggarButtons();

        // ── Wait for choice ──
        yield return new WaitUntil(() => pendingBeggarChoice != 0);

        // ── Hide buttons immediately ──
        HideAllButtons();

        string reply;
        int confidenceValue;

        switch (pendingBeggarChoice)
        {
            case 1: reply = "I understand... stay safe miss."; confidenceValue = -5; break;
            case 2: reply = "Thank you miss... you are very kind."; confidenceValue = 8; break;
            default: reply = "You gave your own lunch...? Thank you so much."; confidenceValue = 12; break;
        }

        // ── Beggar gives final reply (no tap needed — auto hides) ──
        girlCloud.SetActive(false);
        beggerCloud.SetActive(true);
        AnimatePopup(beggerCloud);

        yield return TypeText(beggerText, reply);

        yield return new WaitForSeconds(1.5f);  // auto-hide after 1.5s

        // ── Hide everything ──
        girlCharacter.SetActive(false);
        girlCloud.SetActive(false);
        beggerCharacter.SetActive(false);
        beggerCloud.SetActive(false);
        dialogueBackground.SetActive(false);

        ChangeConfidence(confidenceValue);

        // Remove beggar from map
        if (beggerMapObject != null) Destroy(beggerMapObject);

        dialogueRunning = false;
        gameRunning = true;
    }

    // =====================================================
    // BULLY
    // =====================================================

    public void OpenBully()
    {
        if (dialogueRunning) return;
        StartCoroutine(BullyConversation());
    }

    IEnumerator BullyConversation()
    {
        dialogueRunning = true;
        gameRunning = false;
        zoomOutButton.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        dialogueBackground.SetActive(true);
        girlCharacter.SetActive(true);
        bulliCharacter.SetActive(true);

        // ── Bully speaks ──
        girlCloud.SetActive(false);
        bulliCloud.SetActive(true);
        AnimatePopup(bulliCloud);

        yield return TypeText(bulliText,
            "Hey! Nice outfit. Trying to impress someone at work?");
        yield return WaitForTap();

        // ── Girl reacts ──
        bulliCloud.SetActive(false);
        girlCloud.SetActive(true);
        AnimatePopup(girlCloud);

        yield return TypeText(girlText,
            "They're trying to shake my confidence... I should stay calm.");

        yield return new WaitForSeconds(0.5f);

        // ── Set up buttons ──
        bulliBtn1Text.text = "I don't need to waste my time.";
        bulliBtn2Text.text = "Your opinion doesn't define me.";
        bulliBtn3Text.text = "This behavior isn't acceptable.";

        pendingBullyChoice = 0;

        bulliBtn1.onClick.RemoveAllListeners();
        bulliBtn2.onClick.RemoveAllListeners();
        bulliBtn3.onClick.RemoveAllListeners();

        bulliBtn1.onClick.AddListener(() => { pendingBullyChoice = 1; });
        bulliBtn2.onClick.AddListener(() => { pendingBullyChoice = 2; });
        bulliBtn3.onClick.AddListener(() => { pendingBullyChoice = 3; });

        ShowBullyButtons();

        yield return new WaitUntil(() => pendingBullyChoice != 0);
        HideAllButtons();

        string reply;
        int confidenceValue;

        switch (pendingBullyChoice)
        {
            case 1: reply = "Whatever... she's ignoring us."; confidenceValue = 5; break;
            case 2: reply = "Tch... attitude."; confidenceValue = 10; break;
            default: reply = "Alright alright... we're leaving."; confidenceValue = 12; break;
        }

        // ── Bully final reply (auto hide) ──
        girlCloud.SetActive(false);
        bulliCloud.SetActive(true);
        AnimatePopup(bulliCloud);

        yield return TypeText(bulliText, reply);

        yield return new WaitForSeconds(1.5f);

        girlCharacter.SetActive(false);
        girlCloud.SetActive(false);
        bulliCharacter.SetActive(false);
        bulliCloud.SetActive(false);
        dialogueBackground.SetActive(false);
        zoomOutButton.SetActive(true);

        ChangeConfidence(confidenceValue);

        // Remove bully from map
        if (bulliMapObject != null) Destroy(bulliMapObject);

        dialogueRunning = false;
        gameRunning = true;
    }

    // =====================================================
    // WORKER
    // =====================================================

    public void OpenWorker()
    {
        if (dialogueRunning) return;
        StartCoroutine(WorkerConversation());
    }

    IEnumerator WorkerConversation()
    {
        dialogueRunning = true;
        gameRunning = false;

        yield return new WaitForSeconds(0.5f);

        dialogueBackground.SetActive(true);
        girlCharacter.SetActive(true);
        workerCharacter.SetActive(true);

        // ── Worker speaks ──
        girlCloud.SetActive(false);
        workerCloud.SetActive(true);
        AnimatePopup(workerCloud);

        yield return TypeText(workerText,
            "Road ahead is blocked. You'll need to find another route.");
        yield return WaitForTap();

        // ── Girl reacts ──
        workerCloud.SetActive(false);
        girlCloud.SetActive(true);
        AnimatePopup(girlCloud);

        yield return TypeText(girlText,
            "Hmm... maybe asking politely is smarter than rushing.");

        yield return new WaitForSeconds(0.5f);

        // ── Set up buttons ──
        workerBtn1Text.text = "I'll find my own way.";
        workerBtn2Text.text = "Could you help me find another route?";
        workerBtn3Text.text = "Let me help you for a minute.";

        pendingWorkerChoice = 0;

        workerBtn1.onClick.RemoveAllListeners();
        workerBtn2.onClick.RemoveAllListeners();
        workerBtn3.onClick.RemoveAllListeners();

        workerBtn1.onClick.AddListener(() => { pendingWorkerChoice = 1; });
        workerBtn2.onClick.AddListener(() => { pendingWorkerChoice = 2; });
        workerBtn3.onClick.AddListener(() => { pendingWorkerChoice = 3; });

        ShowWorkerButtons();

        yield return new WaitUntil(() => pendingWorkerChoice != 0);
        HideAllButtons();

        string reply;
        int confidenceValue;

        switch (pendingWorkerChoice)
        {
            case 1: reply = "Be careful then."; confidenceValue = -5; break;
            case 2: reply = "Sure miss, take the left path."; confidenceValue = 8; break;
            default: reply = "Thanks for helping! You're very kind."; confidenceValue = 12; break;
        }

        // ── Worker final reply (auto hide) ──
        girlCloud.SetActive(false);
        workerCloud.SetActive(true);
        AnimatePopup(workerCloud);

        yield return TypeText(workerText, reply);

        yield return new WaitForSeconds(1.5f);

        girlCharacter.SetActive(false);
        girlCloud.SetActive(false);
        workerCharacter.SetActive(false);
        workerCloud.SetActive(false);
        dialogueBackground.SetActive(false);

        ChangeConfidence(confidenceValue);

        // ── Remove CURRENT worker from map, advance index for next encounter ──
        if (workerMapObjects != null && workerMapIndex < workerMapObjects.Length)
        {
            if (workerMapObjects[workerMapIndex] != null)
                Destroy(workerMapObjects[workerMapIndex]);
            workerMapIndex++;
        }

        dialogueRunning = false;
        gameRunning = true;
    }

    // =====================================================
    // CONFIDENCE
    // =====================================================

    void ChangeConfidence(int value)
    {
        confidence += value;
        confidence = Mathf.Clamp(confidence, 0, 100);
        UpdateConfidenceUI();

        if (confidence <= 0)
            LoseGame();
    }

    void UpdateConfidenceUI()
    {
        confidenceSlider.value = confidence;
        confidenceText.text = confidence + "%";
    }

    // =====================================================
    // WIN / FAIL
    // =====================================================

    public void WinGame()
    {
        gameRunning = false;
        winPopup.SetActive(true);
        AnimatePopup(winPopup);
    }

    public void LoseGame()
    {
        gameRunning = false;
        failPopup.SetActive(true);
    }

    // =====================================================
    // RESTART
    // =====================================================

    public void RestartGame()
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex);
    }
}