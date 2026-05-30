using System.Collections;
using UnityEngine;

public class ClickableObject : MonoBehaviour
{
    [Header("Markers")]
    public GameObject circleMarker;

    public GameObject wrongMarker;

    [Header("Inventory")]
    public GameObject inventoryIcon;

    [Header("Inventory Slot")]
    public int inventorySlotIndex;

    [Header("Player")]
    public Transform player;

    [Header("Required Distance")]
    public float requiredDistance = 2f;

    [Header("Popup Animation")]
    public float popupTime = 0.25f;

    [HideInInspector]
    public bool isFound = false;

    [HideInInspector]
    public bool isTaskObject = false;

    private SpriteRenderer sr;

    private Collider2D col;

    private DuelityLevelManager levelManager;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

        col = GetComponent<Collider2D>();

#if UNITY_2023_1_OR_NEWER
        levelManager =
            Object.FindFirstObjectByType
            <DuelityLevelManager>();
#else
        levelManager =
            FindObjectOfType
            <DuelityLevelManager>();
#endif
    }

    void Start()
    {
        // HIDE TICK
        if (circleMarker != null)
        {
            circleMarker.SetActive(false);
        }

        // HIDE WRONG
        if (wrongMarker != null)
        {
            wrongMarker.SetActive(false);
        }

        // HIDE OBJECT INITIALLY
        HideObject();
    }

    void Update()
    {
        // CLICK
        if (Input.GetMouseButtonDown(0))
        {
            DetectClick();
        }
    }

    void DetectClick()
    {
        // CAMERA CHECK
        if (Camera.main == null)
            return;

        // MOUSE POSITION
        Vector2 mousePos =
            Camera.main.ScreenToWorldPoint(
                Input.mousePosition
            );

        // DETECT OBJECT
        Collider2D hit =
            Physics2D.OverlapPoint(
                mousePos
            );

        // NOTHING
        if (hit == null)
            return;

        // NOT THIS OBJECT
        if (hit.gameObject != gameObject)
            return;

        // ALREADY FOUND
        if (isFound)
            return;

        // DISTANCE CHECK
        float distance =
            Vector2.Distance(
                player.position,
                transform.position
            );

        // TOO FAR
        if (distance > requiredDistance)
        {
            return;
        }

        // CORRECT OBJECT
        if (isTaskObject)
        {
            CorrectClick();
        }
        // WRONG OBJECT
        else
        {
            WrongClick();
        }
    }

    // CORRECT CLICK
    void CorrectClick()
    {
        isFound = true;

        // SHOW TICK
        if (circleMarker != null)
        {
            circleMarker.SetActive(true);

            // RESET SCALE
            circleMarker.transform.localScale =
                Vector3.one;
        }

        // LEVEL MANAGER
        if (levelManager != null)
        {
            levelManager
                .CorrectObjectFound(this);
        }

        // HIDE AFTER DELAY
        StartCoroutine(
            HideCorrectAfterDelay()
        );
    }

    IEnumerator HideCorrectAfterDelay()
    {
        // WAIT
        yield return new WaitForSeconds(0.7f);

        // HIDE TICK
        if (circleMarker != null)
        {
            circleMarker.SetActive(false);
        }

        // HIDE OBJECT
        HideObject();
    }

    // WRONG CLICK
    void WrongClick()
    {
        // MARK AS FOUND SO IT CANT BE CLICKED AGAIN
        isFound = true;

        // TELL LEVEL MANAGER
        if (levelManager != null)
        {
            levelManager
                .WrongObjectClicked(this);
        }

        // SHOW CROSS THEN HIDE
        StartCoroutine(
            ShowCrossThenHide()
        );
    }

    IEnumerator ShowCrossThenHide()
    {
        // SHOW CROSS
        if (wrongMarker != null)
        {
            wrongMarker.transform.localScale =
                Vector3.one;

            wrongMarker.SetActive(true);
        }

        // WAIT
        yield return new WaitForSeconds(0.7f);

        // HIDE CROSS
        if (wrongMarker != null)
        {
            wrongMarker.SetActive(false);
        }

        // HIDE OBJECT
        HideObject();
    }

    // SHOW OBJECT
    public void ShowObject()
    {
        if (sr != null)
        {
            sr.enabled = true;
        }

        if (col != null)
        {
            col.enabled = true;
        }
    }

    // HIDE OBJECT
    public void HideObject()
    {
        if (sr != null)
        {
            sr.enabled = false;
        }

        if (col != null)
        {
            col.enabled = false;
        }
    }

    // RESET OBJECT
    public void ResetObject()
    {
        isFound = false;

        isTaskObject = false;

        HideObject();

        // HIDE TICK
        if (circleMarker != null)
        {
            circleMarker.SetActive(false);
        }

        // HIDE WRONG
        if (wrongMarker != null)
        {
            wrongMarker.SetActive(false);
        }
    }
}