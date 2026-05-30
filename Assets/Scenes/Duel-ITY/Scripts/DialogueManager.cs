using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [Header("Boy Character")]
    public GameObject boyCharacter;

    [Header("Boy Cloud")]
    public GameObject boyPopup;

    public TMP_Text boyText;

    [Header("Girl Character")]
    public GameObject girlCharacter;

    [Header("Girl Cloud")]
    public GameObject girlPopup;

    public TMP_Text girlText;

    [Header("Dialogue Background")]
    public GameObject dialogueBackground;

    [Header("Typing Speed")]
    public float typingSpeed = 0.03f;

    [Header("Popup Animation")]
    public float popupTime = 0.25f;

    private bool waitingForTap = false;

    private bool skipTyping = false;

    private bool isTyping = false;

    // BLOCKS INPUT DURING AUTO MODE
    private bool isAutoMode = false;

    // ORIGINAL SCALES
    private Vector3 boyPopupScale;

    private Vector3 girlPopupScale;

    void Start()
    {
        // SAVE ORIGINAL SCALE
        if (boyPopup != null)
        {
            boyPopupScale =
                boyPopup.transform.localScale;
        }

        if (girlPopup != null)
        {
            girlPopupScale =
                girlPopup.transform.localScale;
        }
    }

    void Update()
    {
        // BLOCK ALL INPUT DURING AUTO MODE
        if (isAutoMode)
            return;

        // CLICK / SPACE
        if (Input.GetMouseButtonDown(0) ||
            Input.GetKeyDown(KeyCode.Space))
        {
            // SKIP TYPING
            if (isTyping)
            {
                skipTyping = true;
            }
            // NEXT
            else if (waitingForTap)
            {
                waitingForTap = false;
            }
        }
    }

    // BOY DIALOGUE
    public IEnumerator ShowBoy(string message)
    {
        // SHOW BACKGROUND
        if (dialogueBackground != null)
        {
            dialogueBackground.SetActive(true);
        }

        // SHOW CHARACTERS
        if (boyCharacter != null)
        {
            boyCharacter.SetActive(true);
        }

        if (girlCharacter != null)
        {
            girlCharacter.SetActive(true);
        }

        // HIDE GIRL CLOUD
        if (girlPopup != null)
        {
            girlPopup.SetActive(false);
        }

        // SHOW BOY CLOUD
        if (boyPopup != null)
        {
            boyPopup.SetActive(true);

            // POPUP EFFECT
            AnimatePopup(
                boyPopup,
                boyPopupScale
            );
        }

        // CLEAR TEXT
        boyText.text = "";

        // TYPE EFFECT
        yield return StartCoroutine(
            TypeText(
                boyText,
                message
            )
        );

        // WAIT TAP
        yield return StartCoroutine(
            WaitForTap()
        );
    }

    // GIRL DIALOGUE
    public IEnumerator ShowGirl(string message)
    {
        // SHOW BACKGROUND
        if (dialogueBackground != null)
        {
            dialogueBackground.SetActive(true);
        }

        // SHOW CHARACTERS
        if (boyCharacter != null)
        {
            boyCharacter.SetActive(true);
        }

        if (girlCharacter != null)
        {
            girlCharacter.SetActive(true);
        }

        // HIDE BOY CLOUD
        if (boyPopup != null)
        {
            boyPopup.SetActive(false);
        }

        // SHOW GIRL CLOUD
        if (girlPopup != null)
        {
            girlPopup.SetActive(true);

            // POPUP EFFECT
            AnimatePopup(
                girlPopup,
                girlPopupScale
            );
        }

        // CLEAR TEXT
        girlText.text = "";

        // TYPE EFFECT
        yield return StartCoroutine(
            TypeText(
                girlText,
                message
            )
        );

        // WAIT TAP
        yield return StartCoroutine(
            WaitForTap()
        );
    }

    // BOY DIALOGUE AUTO — NO TAP NEEDED
    public IEnumerator ShowBoyAuto(
        string message,
        float displayTime = 2f
    )
    {
        // BLOCK INPUT
        isAutoMode = true;

        // SHOW BACKGROUND
        if (dialogueBackground != null)
        {
            dialogueBackground.SetActive(true);
        }

        // SHOW CHARACTERS
        if (boyCharacter != null)
        {
            boyCharacter.SetActive(true);
        }

        if (girlCharacter != null)
        {
            girlCharacter.SetActive(true);
        }

        // HIDE GIRL CLOUD
        if (girlPopup != null)
        {
            girlPopup.SetActive(false);
        }

        // SHOW BOY CLOUD
        if (boyPopup != null)
        {
            boyPopup.SetActive(true);

            // POPUP EFFECT
            AnimatePopup(
                boyPopup,
                boyPopupScale
            );
        }

        // CLEAR TEXT
        boyText.text = "";

        // TYPE FULL TEXT INSTANTLY — no typing animation
        // so no tap can interfere mid-type
        boyText.text = message;

        // WAIT FIXED TIME
        yield return new WaitForSeconds(displayTime);

        // HIDE CLOUDS
        HideDialogueClouds();

        // RESTORE INPUT
        isAutoMode = false;
    }

    // TYPE TEXT
    IEnumerator TypeText(
        TMP_Text textUI,
        string message
    )
    {
        isTyping = true;

        skipTyping = false;

        textUI.text = "";

        foreach (char c in message)
        {
            // SKIP
            if (skipTyping)
            {
                textUI.text = message;

                break;
            }

            textUI.text += c;

            yield return new WaitForSeconds(
                typingSpeed
            );
        }

        isTyping = false;
    }

    // WAIT FOR TAP
    IEnumerator WaitForTap()
    {
        waitingForTap = true;

        while (waitingForTap)
        {
            yield return null;
        }

        // HIDE CLOUDS
        HideDialogueClouds();
    }

    // POPUP EFFECT
    void AnimatePopup(
        GameObject popup,
        Vector3 originalScale
    )
    {
        // START SMALL
        popup.transform.localScale =
            Vector3.zero;

        // RETURN TO ORIGINAL SIZE
        iTween.ScaleTo(
            popup,
            iTween.Hash(
                "scale", originalScale,
                "time", popupTime,
                "easetype",
                iTween.EaseType.easeOutBack
            )
        );
    }

    // HIDE CLOUDS ONLY
    public void HideDialogueClouds()
    {
        // HIDE BOY CLOUD
        if (boyPopup != null)
        {
            boyPopup.SetActive(false);
        }

        // HIDE GIRL CLOUD
        if (girlPopup != null)
        {
            girlPopup.SetActive(false);
        }

        // HIDE BACKGROUND
        if (dialogueBackground != null)
        {
            dialogueBackground.SetActive(false);
        }
    }

    // END FULL CONVERSATION
    public void EndConversation()
    {
        // RESET AUTO MODE JUST IN CASE
        isAutoMode = false;

        // HIDE CLOUDS
        HideDialogueClouds();

        // HIDE BOY CHARACTER
        if (boyCharacter != null)
        {
            boyCharacter.SetActive(false);
        }

        // HIDE GIRL CHARACTER
        if (girlCharacter != null)
        {
            girlCharacter.SetActive(false);
        }
    }
}