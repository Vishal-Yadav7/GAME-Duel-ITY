using UnityEngine;

public class HideRevealObject : MonoBehaviour
{
    [Header("Hidden Object")]
    public GameObject hiddenObject;

    [Header("Cover Object")]
    public GameObject coverObject;

    [Header("Popup Effect")]
    public bool usePopupEffect = true;

    public float popupTime = 0.25f;

    private bool opened = false;

    void Start()
    {
        // HIDE OBJECT INITIALLY
        if (hiddenObject != null)
        {
            SpriteRenderer sr =
                hiddenObject.GetComponent<SpriteRenderer>();

            Collider2D col =
                hiddenObject.GetComponent<Collider2D>();

            if (sr != null)
            {
                sr.enabled = false;
            }

            if (col != null)
            {
                col.enabled = false;
            }
        }
    }

    void OnMouseDown()
    {
        // ALREADY OPENED
        if (opened)
            return;

        opened = true;

        // SHOW OBJECT
        if (hiddenObject != null)
        {
            SpriteRenderer sr =
                hiddenObject.GetComponent<SpriteRenderer>();

            Collider2D col =
                hiddenObject.GetComponent<Collider2D>();

            if (sr != null)
            {
                sr.enabled = true;
            }

            if (col != null)
            {
                col.enabled = true;
            }

            // POPUP EFFECT
            if (usePopupEffect)
            {
                Vector3 originalScale =
                    hiddenObject.transform.localScale;

                hiddenObject.transform.localScale =
                    Vector3.zero;

                iTween.ScaleTo(
                    hiddenObject,
                    iTween.Hash(
                        "scale",
                        originalScale,
                        "time",
                        popupTime,
                        "easetype",
                        iTween.EaseType.easeOutBack
                    )
                );
            }
        }

        // HIDE COVER
        if (coverObject != null)
        {
            coverObject.SetActive(false);
        }
    }
}