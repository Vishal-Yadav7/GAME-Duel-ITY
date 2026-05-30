using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Cutscene : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text dialogueText;

    public GameObject continueButton;

    [Header("Voice Buttons")]
    public Button voiceOnButton;

    public Button voiceOffButton;

    [Header("Typing Settings")]
    public float typingSpeed = 0.04f;

    public float nextLineDelay = 1.2f;

    [Header("Audio Sources")]
    public AudioSource voiceSource;

    public AudioSource typingSource;

    [Header("Typing Sound")]
    public AudioClip typingClip;

    [Header("Voice Clips")]
    public AudioClip[] voiceClips;

    [Header("Voice Volume")]
    [Range(0f, 5f)]
    public float voiceVolume = 3f;

    [Header("Typing Volume")]
    [Range(0f, 1f)]
    public float typingVolume = 0.04f;

    private bool isTyping = false;

    private bool skipTyping = false;

    // START WITH MIC OFF
    private bool voiceEnabled = false;

    private int currentLine = 0;

    // STORY
    private string[] storyLines =
    {
        "Every day, I wake before the world does.",
        "Before the sun rises, my responsibilities already begin.",

        "Cooking. Cleaning. Carrying burdens no one notices.",
        "To them, I am only a homemaker.",

        "But inside me... another woman exists.",
        "A woman with dreams, courage, and ambition.",

        "Two worlds. One soul. One identity.",
        "This is my Duelity."
    };

    void Start()
    {
        // AUTO CREATE AUDIO SOURCES
        if (voiceSource == null)
        {
            voiceSource =
                gameObject.AddComponent
                <AudioSource>();
        }

        if (typingSource == null)
        {
            typingSource =
                gameObject.AddComponent
                <AudioSource>();
        }

        // =========================
        // VOICE QUALITY BOOST
        // =========================

        // 2D SOUND
        voiceSource.spatialBlend = 0f;

        // HIGHEST PRIORITY
        voiceSource.priority = 0;

        // REMOVE DISTORTION
        voiceSource.dopplerLevel = 0f;

        // REMOVE REVERB
        voiceSource.reverbZoneMix = 0f;

        // CENTER SOUND
        voiceSource.panStereo = 0f;

        // LOUDER VOICE
        voiceSource.volume = 3f;

        // CLEANER OUTPUT
        voiceSource.rolloffMode =
            AudioRolloffMode.Linear;

        voiceSource.minDistance = 1f;

        voiceSource.maxDistance = 500f;

        voiceSource.spread = 0;

        // =========================
        // TYPING SETTINGS
        // =========================

        typingSource.spatialBlend = 0f;

        typingSource.priority = 64;

        typingSource.dopplerLevel = 0f;

        typingSource.reverbZoneMix = 0f;

        typingSource.volume =
            typingVolume;

        // BUTTONS
        if (voiceOnButton != null)
        {
            voiceOnButton.onClick
                .AddListener(
                    TurnVoiceOff
                );
        }

        if (voiceOffButton != null)
        {
            voiceOffButton.onClick
                .AddListener(
                    TurnVoiceOn
                );
        }

        // HIDE CONTINUE
        if (continueButton != null)
        {
            continueButton.SetActive(false);
        }

        // START WITH MIC OFF
        TurnVoiceOff();

        // START STORY
        StartCoroutine(
            PlayStory()
        );
    }

    IEnumerator PlayStory()
    {
        dialogueText.text = "";

        for (int i = 0;
             i < storyLines.Length;
             i++)
        {
            currentLine = i;

            // PLAY VOICE
            // ONLY IF MIC ON
            PlayVoice(i);

            // TYPE TEXT
            yield return StartCoroutine(
                TypeLine(
                    storyLines[i]
                )
            );

            // NEW LINE
            dialogueText.text += "\n";

            // EXTRA SPACE
            if ((i + 1) % 2 == 0)
            {
                dialogueText.text += "\n";
            }

            yield return new WaitForSeconds(
                nextLineDelay
            );
        }

        // WAIT
        yield return new WaitForSeconds(1f);

        // NORMAL BACKGROUND
        if (AudioManager.instance != null)
        {
            AudioManager.instance
                .CutsceneModeOff();
        }

        // SHOW CONTINUE
        if (continueButton != null)
        {
            continueButton.SetActive(true);

            Vector3 originalScale =
                continueButton.transform.localScale;

            continueButton.transform.localScale =
                Vector3.zero;

            iTween.ScaleTo(
                continueButton,
                iTween.Hash(
                    "scale",
                    originalScale,
                    "time",
                    0.4f,
                    "easetype",
                    iTween.EaseType.easeOutBack
                )
            );
        }
    }

    IEnumerator TypeLine(
        string line
    )
    {
        isTyping = true;

        skipTyping = false;

        string oldText =
            dialogueText.text;

        foreach (char letter in line)
        {
            // SKIP LINE
            if (skipTyping)
            {
                dialogueText.text =
                    oldText + line;

                break;
            }

            // ADD LETTER
            dialogueText.text += letter;

            // TYPING SOUND
            if (typingClip != null)
            {
                typingSource.PlayOneShot(
                    typingClip,
                    typingVolume
                );
            }

            yield return new WaitForSeconds(
                typingSpeed
            );
        }

        isTyping = false;
    }

    void Update()
    {
        // CLICK / SPACE
        if (Input.GetMouseButtonDown(0) ||
            Input.GetKeyDown(KeyCode.Space))
        {
            // SKIP CURRENT LINE
            if (isTyping)
            {
                skipTyping = true;
            }
        }
    }

    // PLAY VOICE
    void PlayVoice(int index)
    {
        // MIC OFF
        if (!voiceEnabled)
            return;

        // INVALID
        if (index >= voiceClips.Length)
            return;

        // NO CLIP
        if (voiceClips[index] == null)
            return;

        // STOP OLD
        voiceSource.Stop();

        // NEW CLIP
        voiceSource.clip =
            voiceClips[index];

        // MAX VOLUME
        voiceSource.volume = 3f;

        // PLAY
        voiceSource.Play();
    }

    // MIC ON
    public void TurnVoiceOn()
    {
        voiceEnabled = true;

        // LOWER BGM
        if (AudioManager.instance != null)
        {
            AudioManager.instance
                .FadeToCutscene();
        }

        // BUTTONS
        if (voiceOnButton != null)
        {
            voiceOnButton.gameObject
                .SetActive(true);
        }

        if (voiceOffButton != null)
        {
            voiceOffButton.gameObject
                .SetActive(false);
        }

        // IMPORTANT:
        // CURRENT HALF LINE
        // WILL NOT PLAY
    }

    // MIC OFF
    public void TurnVoiceOff()
    {
        voiceEnabled = false;

        // STOP VOICE
        voiceSource.Stop();

        // NORMAL BGM
        if (AudioManager.instance != null)
        {
            AudioManager.instance
                .FadeToNormal();
        }

        // BUTTONS
        if (voiceOnButton != null)
        {
            voiceOnButton.gameObject
                .SetActive(false);
        }

        if (voiceOffButton != null)
        {
            voiceOffButton.gameObject
                .SetActive(true);
        }
    }

    // CONTINUE
    public void ContinueToNextScene()
    {
        int currentScene =
            SceneManager
            .GetActiveScene()
            .buildIndex;

        SceneManager.LoadScene(
            currentScene + 1
        );
    }
}