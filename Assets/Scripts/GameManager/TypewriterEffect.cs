using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TypewriterEffect : MonoBehaviour
{
    [Header("UI Reference")]
    public TMP_Text dialogueText; // Assign this in Inspector

    [Header("Typing Settings")]
    public float typingSpeed = 0.05f;
    public float linePause = 0.85f;

    [Header("Cutscene Settings")]
    public bool isFinalCutscene = false;

    private bool isTyping = false;
    private bool skipTyping = false;
    private bool nextRequested = false;

    private string[] introDialogue =
    {
        "Hello, Player. It's me, Shinzo.",
        "Listen carefully—I have a secret to tell you.",
        "The developer gives hints throughout the game...",
        "But there's a catch—every hint is a lie.",
        "They are meant to trick you.",
        "If you want to pass the levels, do the opposite of what the hints say.",
        "Oh, and one more thing...",
        "Don't tell anyone I told you this.",
        "It's our little secret."
    };

    private string[] finalDialogue =
    {
        "So, you made it to the end...",
        "You really thought I was guiding you?",
        "...You poor thing.",
        "Every hint you followed...",
        "Every wrong answer you corrected...",
        "It was all *me*.",
        "I was the one who set the wrong hints.",
        "You never outsmarted the game...",
        "You only followed *my* rules.",
        "Funny, isn't it?",
        "We look for hints, believing they’ll lead us to the truth...",
        "But what if every hint was a lie from the start?",
        "What if the right path never existed at all?"
    };

    void Start()
    {
        if (dialogueText == null)
        {
#if UNITY_2023_1_OR_NEWER
            dialogueText = Object.FindFirstObjectByType<TMP_Text>();
#else
            dialogueText = FindObjectOfType<TMP_Text>();
#endif
            if (dialogueText == null)
            {
                Debug.LogError("❌ No TMP_Text found! Please assign a Dialogue Text UI in the Inspector.");
                return;
            }
        }

        dialogueText.text = "";

        if (!isFinalCutscene && PlayerPrefs.GetInt("CutsceneWatched", 0) == 1 && !Application.isEditor)
        {
            SkipCutscene();
            return;
        }

        StartCoroutine(DisplayDialogue());
    }

    void Update()
    {
        // Debug reset key → press "R" to reset cutscene flag
        if (Input.GetKeyDown(KeyCode.R))
        {
            PlayerPrefs.DeleteKey("CutsceneWatched");
            Debug.Log("🔄 Cutscene reset: intro will play again.");
        }

        // On click/tap/space
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
                skipTyping = true;   // Finish current line instantly
            else
                nextRequested = true; // Move to next line immediately
        }
    }

    IEnumerator DisplayDialogue()
    {
        string[] lines = isFinalCutscene ? finalDialogue : introDialogue;

        foreach (string line in lines)
        {
            yield return StartCoroutine(TypeLine(line));

            // Wait for either player click OR timeout (auto advance)
            float timer = 0f;
            nextRequested = false;

            while (!nextRequested && timer < linePause)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            nextRequested = false; // reset
            dialogueText.text = "";
        }

        if (!isFinalCutscene)
        {
            PlayerPrefs.SetInt("CutsceneWatched", 1);
            PlayerPrefs.Save();
        }

        LoadNextScene();
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in line)
        {
            if (skipTyping)
            {
                dialogueText.text = line; // instantly finish
                break;
            }

            dialogueText.text += letter;

            if (AudioManager.instance != null)
                AudioManager.instance.Play("typeSound");

            if (isFinalCutscene && Random.value > 0.9f)
            {
                dialogueText.text = "<color=red>" + dialogueText.text + "</color>";
                yield return new WaitForSeconds(0.1f);
                dialogueText.text = dialogueText.text.Replace("<color=red>", "").Replace("</color>", "");
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        skipTyping = false;
    }

    void SkipCutscene()
    {
        LoadNextScene();
    }

    void LoadNextScene()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;

        if (!isFinalCutscene)
        {
            if (currentIndex + 1 < SceneManager.sceneCountInBuildSettings)
                SceneManager.LoadScene(currentIndex + 1);
            else
                Debug.LogError("❌ No next scene found in Build Settings!");
        }
        else
        {
            SceneManager.LoadScene(0); // Return to main menu
        }
    }
}
