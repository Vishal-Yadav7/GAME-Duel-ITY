using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CookingGameManager : MonoBehaviour
{
    // =====================================================
    // INSPECTOR FIELDS
    // =====================================================

    [Header("Panels")]
    public GameObject winPanel;
    public GameObject failPanel;
    public GameObject winPopupBox;
    public GameObject failPopupBox;
    public string nextSceneName;
    public string homeSceneName = "Home";
    public float popupTime = 0.3f;

    [Header("Dialogue Background")]
    public GameObject dialogueBackground;

    [Header("Characters")]
    public GameObject husband;
    public GameObject wife;
    public GameObject mother;
    public GameObject child;

    // ─────────────────────────────────────────────────────
    // GIRL WITH FOOD  (drag-drop sprite GameObjects here)
    // ─────────────────────────────────────────────────────
    [Header("Girl Holding Food — Sprites")]
    public GameObject girlWithOmelette;
    public GameObject girlWithTea;
    public GameObject girlWithMaggie;

    // ─────────────────────────────────────────────────────
    // GIRL SPEECH CLOUDS — one per dish
    // ─────────────────────────────────────────────────────
    [Header("Girl Speech Clouds")]
    public GameObject girlOmeletteCloud;
    public GameObject girlTeaCloud;
    public GameObject girlMaggieCloud;

    // ─────────────────────────────────────────────────────
    // GIRL SPEECH TEXTS — TMP_Text inside each cloud
    // ─────────────────────────────────────────────────────
    [Header("Girl Speech Texts (inside clouds)")]
    public TMP_Text girlOmeletteText;
    public TMP_Text girlTeaText;
    public TMP_Text girlMaggieText;

    [Header("Clouds")]
    public GameObject husbandCloud;
    public GameObject wifeCloud;
    public GameObject motherCloud;
    public GameObject childCloud;

    [Header("Dialogue Texts")]
    public TMP_Text husbandText;
    public TMP_Text wifeText;
    public TMP_Text motherText;
    public TMP_Text childText;

    [Header("Task UI")]
    public TMP_Text taskText;
    public TMP_Text hintText;

    [Header("Timer")]
    public TMP_Text timerText;
    public float dishTimeLimit = 30f;

    [Header("Recipe Manager")]
    public CookingRecipeManager recipeManager;

    [Header("Typing Speed")]
    public float typingSpeed = 0.03f;

    [Header("Wrong Cross Popup")]
    public GameObject crossPopup;
    public float crossPopupDuration = 0.7f;

    [Header("Cook Button (Hold)")]
    public GameObject cookButtonObject;
    public Slider cookHoldSlider;
    public float holdDuration = 2f;

    // =====================================================
    // PRIVATE STATE
    // =====================================================

    private bool waitingForTap;
    private bool isTyping;
    private bool skipTyping;

    private bool cookReady = false;
    private bool isHoldingCook = false;
    private float holdProgress = 0f;

    private Coroutine timerRoutine;
    private bool timerRunning = false;

    // =====================================================
    // UNITY LIFECYCLE
    // =====================================================

    void Start()
    {
        HideAllClouds();
        HideAllGirlFood();
        HideAllGirlClouds();

        if (dialogueBackground != null) dialogueBackground.SetActive(false);
        if (crossPopup != null) crossPopup.SetActive(false);
        if (cookButtonObject != null) cookButtonObject.SetActive(false);
        if (cookHoldSlider != null) cookHoldSlider.gameObject.SetActive(false);
        if (timerText != null) timerText.gameObject.SetActive(false);

        taskText.gameObject.SetActive(false);
        hintText.gameObject.SetActive(false);

        husband.SetActive(true);
        wife.SetActive(true);
        mother.SetActive(false);
        child.SetActive(false);

        StartCoroutine(HusbandScene());
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isTyping) skipTyping = true;
            else waitingForTap = false;
        }

        HandleCookHold();
    }

    // =====================================================
    // HELPERS
    // =====================================================

    void HideAllGirlFood()
    {
        if (girlWithOmelette != null) girlWithOmelette.SetActive(false);
        if (girlWithTea != null) girlWithTea.SetActive(false);
        if (girlWithMaggie != null) girlWithMaggie.SetActive(false);
    }

    void HideAllGirlClouds()
    {
        if (girlOmeletteCloud != null) girlOmeletteCloud.SetActive(false);
        if (girlTeaCloud != null) girlTeaCloud.SetActive(false);
        if (girlMaggieCloud != null) girlMaggieCloud.SetActive(false);
    }

    void HideAllClouds()
    {
        if (husbandCloud != null) husbandCloud.SetActive(false);
        if (wifeCloud != null) wifeCloud.SetActive(false);
        if (motherCloud != null) motherCloud.SetActive(false);
        if (childCloud != null) childCloud.SetActive(false);
    }

    // =====================================================
    // TIMER
    // =====================================================

    void StartDishTimer()
    {
        StopDishTimer();
        if (timerText != null) timerText.gameObject.SetActive(true);
        timerRoutine = StartCoroutine(CountdownRoutine());
        timerRunning = true;
    }

    void StopDishTimer()
    {
        if (timerRoutine != null) { StopCoroutine(timerRoutine); timerRoutine = null; }
        timerRunning = false;
        if (timerText != null) timerText.gameObject.SetActive(false);
    }

    IEnumerator CountdownRoutine()
    {
        float remaining = dishTimeLimit;
        while (remaining > 0f)
        {
            remaining -= Time.deltaTime;
            if (remaining < 0f) remaining = 0f;
            if (timerText != null) timerText.text = Mathf.CeilToInt(remaining).ToString();
            yield return null;
        }
        timerRunning = false;
        if (timerText != null) timerText.gameObject.SetActive(false);
        ShowFailPanel();
    }

    // =====================================================
    // COOK HOLD BUTTON
    // =====================================================

    public void OnCookButtonDown()
    {
        if (!cookReady) { StartCoroutine(ShowCrossPopup()); return; }
        isHoldingCook = true;
    }

    public void OnCookButtonUp()
    {
        isHoldingCook = false;
        if (holdProgress < 1f)
        {
            holdProgress = 0f;
            if (cookHoldSlider != null) cookHoldSlider.value = 0f;
        }
    }

    void HandleCookHold()
    {
        if (!isHoldingCook) return;
        holdProgress += Time.deltaTime / holdDuration;
        holdProgress = Mathf.Clamp01(holdProgress);
        if (cookHoldSlider != null) cookHoldSlider.value = holdProgress;
        if (holdProgress >= 1f) { isHoldingCook = false; OnCookComplete(); }
    }

    void OnCookComplete()
    {
        holdProgress = 0f;
        cookReady = false;
        if (cookHoldSlider != null) { cookHoldSlider.value = 0f; cookHoldSlider.gameObject.SetActive(false); }
        if (cookButtonObject != null) cookButtonObject.SetActive(false);
        StopDishTimer();
        recipeManager.CookingFinished();
    }

    public void SetCookReady(bool ready)
    {
        cookReady = ready;
        holdProgress = 0f;
        isHoldingCook = false;
        if (cookButtonObject != null) cookButtonObject.SetActive(ready);
        if (cookHoldSlider != null) { cookHoldSlider.gameObject.SetActive(ready); cookHoldSlider.value = 0f; }
    }

    // =====================================================
    // CROSS POPUP
    // =====================================================

    IEnumerator ShowCrossPopup()
    {
        if (crossPopup == null) yield break;
        crossPopup.SetActive(true);
        iTween.ScaleFrom(crossPopup, iTween.Hash("scale", Vector3.zero, "time", 0.2f, "easetype", iTween.EaseType.easeOutBack));
        yield return new WaitForSeconds(crossPopupDuration);
        crossPopup.SetActive(false);
    }

    // =====================================================
    // SCENE 1 — HUSBAND (Omelette)
    // =====================================================

    IEnumerator HusbandScene()
    {
        yield return ShowHusband("Welcome home.");
        yield return ShowWife("It's been a long day.");
        yield return ShowHusband("You look exhausted.");
        yield return ShowWife("Just a little tired.");
        yield return ShowHusband("If you're comfortable, could you make an omelette for me?");
        yield return ShowWife("Of course. I'll make one.");
        StartOmeletteTask();
    }

    public void StartOmeletteTask()
    {
        husband.SetActive(false);
        wife.SetActive(false);
        taskText.gameObject.SetActive(true);
        hintText.gameObject.SetActive(true);
        recipeManager.StartOmelette();
        StartDishTimer();
    }

    public void HusbandThankYou()
    {
        StartCoroutine(ShowFoodThenThankyou_Omelette());
    }

    IEnumerator ShowFoodThenThankyou_Omelette()
    {
        taskText.gameObject.SetActive(false);
        hintText.gameObject.SetActive(false);

        // Background opens, girl with food appears on it — she STAYS the whole time
        HideAllGirlFood();
        HideAllGirlClouds();
        if (dialogueBackground != null) dialogueBackground.SetActive(true);
        if (girlWithOmelette != null) girlWithOmelette.SetActive(true);

        // Her cloud handles the entire thank-you conversation
        yield return ShowGirlOmelette("Omelette is ready!");
        yield return ShowGirlOmelette("This looks delicious, thank you!");
        yield return ShowGirlOmelette("You're welcome. That's what family is for.");

        // Clean up, move to next scene
        if (girlWithOmelette != null) girlWithOmelette.SetActive(false);
        if (girlOmeletteCloud != null) girlOmeletteCloud.SetActive(false);
        if (dialogueBackground != null) dialogueBackground.SetActive(false);

        mother.SetActive(true);
        wife.SetActive(true);
        StartCoroutine(MotherScene());
    }

    // =====================================================
    // SCENE 2 — MOTHER (Tea)
    // =====================================================

    IEnumerator MotherScene()
    {
        yield return ShowMother("Beta, you've worked all day...");
        yield return ShowMother("But if you have time, could you make me some tea?");
        yield return ShowWife("Of course, Ma.");
        StartTeaTask();
    }

    public void StartTeaTask()
    {
        mother.SetActive(false);
        wife.SetActive(false);
        recipeManager.StartTea();
        StartDishTimer();
    }

    public void MotherThankYou()
    {
        StartCoroutine(ShowFoodThenThankyou_Tea());
    }

    IEnumerator ShowFoodThenThankyou_Tea()
    {
        taskText.gameObject.SetActive(false);
        hintText.gameObject.SetActive(false);

        // Background opens, girl with tea appears on it — she STAYS
        HideAllGirlFood();
        HideAllGirlClouds();
        if (dialogueBackground != null) dialogueBackground.SetActive(true);
        if (girlWithTea != null) girlWithTea.SetActive(true);

        // Her cloud handles the entire thank-you conversation
        yield return ShowGirlTea("Tea is ready!");
        yield return ShowGirlTea("The tea is wonderful, thank you beta!");
        yield return ShowGirlTea("I'm happy you liked it.");

        // Clean up, move to next scene
        if (girlWithTea != null) girlWithTea.SetActive(false);
        if (girlTeaCloud != null) girlTeaCloud.SetActive(false);
        if (dialogueBackground != null) dialogueBackground.SetActive(false);

        child.SetActive(true);
        wife.SetActive(true);
        StartCoroutine(ChildScene());
    }

    // =====================================================
    // SCENE 3 — CHILD (Maggie)
    // =====================================================

    IEnumerator ChildScene()
    {
        yield return ShowChild("Mom! I'm hungry!");
        yield return ShowChild("Can I have noodles?");
        yield return ShowWife("Of course, sweetheart.");
        StartNoodlesTask();
    }

    public void StartNoodlesTask()
    {
        child.SetActive(false);
        wife.SetActive(false);
        recipeManager.StartMaggie();
        StartDishTimer();
    }

    public void ChildThankYou()
    {
        StartCoroutine(ShowFoodThenThankyou_Maggie());
    }

    IEnumerator ShowFoodThenThankyou_Maggie()
    {
        taskText.gameObject.SetActive(false);
        hintText.gameObject.SetActive(false);

        // Background opens, girl with maggie appears on it — she STAYS
        HideAllGirlFood();
        HideAllGirlClouds();
        if (dialogueBackground != null) dialogueBackground.SetActive(true);
        if (girlWithMaggie != null) girlWithMaggie.SetActive(true);

        // Her cloud handles the entire thank-you conversation
        yield return ShowGirlMaggie("Noodles are ready!");
        yield return ShowGirlMaggie("Yay! Thank you Mom!");
        yield return ShowGirlMaggie("Enjoy your meal, Betaa!");

        // Clean up
        if (girlWithMaggie != null) girlWithMaggie.SetActive(false);
        if (girlMaggieCloud != null) girlMaggieCloud.SetActive(false);
        if (cookButtonObject != null) cookButtonObject.SetActive(false);
        if (cookHoldSlider != null) cookHoldSlider.gameObject.SetActive(false);
        cookReady = false;

        HideAllClouds();
        HideAllGirlClouds();
        if (dialogueBackground != null) dialogueBackground.SetActive(false);
        if (crossPopup != null) crossPopup.SetActive(false);

        ShowWinPanel();
    }

    // =====================================================
    // GIRL DIALOGUE HELPERS
    // Each one shows that girl's cloud, types the message,
    // waits for tap, then hides the cloud.
    // =====================================================

    IEnumerator ShowGirlOmelette(string msg)
    {
        yield return StartCoroutine(ShowGirlDialogue(girlOmeletteCloud, girlOmeletteText, msg));
    }

    IEnumerator ShowGirlTea(string msg)
    {
        yield return StartCoroutine(ShowGirlDialogue(girlTeaCloud, girlTeaText, msg));
    }

    IEnumerator ShowGirlMaggie(string msg)
    {
        yield return StartCoroutine(ShowGirlDialogue(girlMaggieCloud, girlMaggieText, msg));
    }

    IEnumerator ShowGirlDialogue(GameObject cloud, TMP_Text textUI, string message)
    {
        if (cloud != null) cloud.SetActive(true);
        if (textUI != null) textUI.text = "";

        isTyping = true;
        skipTyping = false;

        if (textUI != null)
        {
            foreach (char c in message)
            {
                if (skipTyping) { textUI.text = message; break; }
                textUI.text += c;
                yield return new WaitForSeconds(typingSpeed);
            }
        }

        isTyping = false;
        waitingForTap = true;
        while (waitingForTap) yield return null;

        if (cloud != null) cloud.SetActive(false);
    }

    // =====================================================
    // STANDARD DIALOGUE SYSTEM (used for intro scenes)
    // =====================================================

    IEnumerator ShowHusband(string msg) { yield return StartCoroutine(ShowDialogue(husbandCloud, husbandText, msg)); }
    IEnumerator ShowWife(string msg) { yield return StartCoroutine(ShowDialogue(wifeCloud, wifeText, msg)); }
    IEnumerator ShowMother(string msg) { yield return StartCoroutine(ShowDialogue(motherCloud, motherText, msg)); }
    IEnumerator ShowChild(string msg) { yield return StartCoroutine(ShowDialogue(childCloud, childText, msg)); }

    IEnumerator ShowDialogue(GameObject cloud, TMP_Text textUI, string message)
    {
        HideAllClouds();
        if (dialogueBackground != null) dialogueBackground.SetActive(true);

        cloud.SetActive(true);
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

        cloud.SetActive(false);
        if (dialogueBackground != null) dialogueBackground.SetActive(false);
    }

    // =====================================================
    // WIN / FAIL PANELS
    // =====================================================

    public void ShowWinPanel()
    {
        if (winPanel != null) winPanel.SetActive(true);

        if (winPopupBox != null)
        {
            winPopupBox.transform.localScale = Vector3.one;
            iTween.ScaleFrom(winPopupBox, iTween.Hash(
                "scale", Vector3.zero, "time", popupTime,
                "easetype", iTween.EaseType.easeOutBack, "ignoretimescale", true));
        }

        StartCoroutine(PauseAfterDelay(popupTime));
    }

    IEnumerator PauseAfterDelay(float delay)
    {
        float t = 0f;
        while (t < delay) { t += Time.unscaledDeltaTime; yield return null; }
        Time.timeScale = 0f;
    }

    public void ShowFailPanel()
    {
        if (failPanel != null) failPanel.SetActive(true);

        if (failPopupBox != null)
        {
            failPopupBox.transform.localScale = Vector3.one;
            iTween.ScaleFrom(failPopupBox, iTween.Hash(
                "scale", Vector3.zero, "time", popupTime,
                "easetype", iTween.EaseType.easeOutBack, "ignoretimescale", true));
        }
        if (crossPopup != null) crossPopup.SetActive(false);
        StartCoroutine(PauseAfterDelay(popupTime));
    }

    // =====================================================
    // BUTTON HANDLERS
    // =====================================================

    public void RetryLevel()
    {
        if (crossPopup != null) crossPopup.SetActive(false);
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        if (crossPopup != null) crossPopup.SetActive(false);
        Time.timeScale = 1f;
        SceneManager.LoadScene(nextSceneName);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(homeSceneName);
    }

    // =====================================================
    // UNUSED TypeText helper (kept for safety)
    // =====================================================

    IEnumerator TypeText(TMP_Text textUI, string message)
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
    }
}