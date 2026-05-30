using UnityEngine;
using UnityEngine.UI;

public class HoldCookButton : MonoBehaviour
{
    [Header("UI")]
    public Slider cookSlider;

    [Header("Manager")]
    public CookingRecipeManager recipeManager;

    [Header("Settings")]
    public float cookSpeed = 35f;

    private bool isHolding;
    private float progress;

    void Start()
    {
        if (cookSlider != null)
        {
            cookSlider.value = 0;
            cookSlider.gameObject.SetActive(false);
        }

        gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isHolding)
            return;

        progress += Time.deltaTime * cookSpeed;

        progress = Mathf.Clamp(progress, 0f, 100f);

        if (cookSlider != null)
        {
            cookSlider.value = progress;
        }

        if (progress >= 100f)
        {
            isHolding = false;

            if (recipeManager != null)
            {
                recipeManager.CookingFinished();
            }

            gameObject.SetActive(false);

            if (cookSlider != null)
            {
                cookSlider.gameObject.SetActive(false);
            }
        }
    }

    // EVENT TRIGGER → POINTER DOWN
    public void StartHolding()
    {
        isHolding = true;
    }

    // EVENT TRIGGER → POINTER UP
    public void StopHolding()
    {
        isHolding = false;
    }

    public void ShowCookButton()
    {
        progress = 0f;

        if (cookSlider != null)
        {
            cookSlider.value = 0;
            cookSlider.gameObject.SetActive(true);
        }

        gameObject.SetActive(true);
    }

    public void ResetCook()
    {
        isHolding = false;

        progress = 0f;

        if (cookSlider != null)
        {
            cookSlider.value = 0;
        }
    }
}