using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Confidence : MonoBehaviour
{
    public static
        Confidence instance;

    public Slider confidenceSlider;

    public TMP_Text confidenceText;

    public int confidence = 100;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        UpdateUI();
    }

    public void RemoveConfidence(
        int amount
    )
    {
        confidence -= amount;

        if (confidence < 0)
            confidence = 0;

        UpdateUI();
    }

    public void AddConfidence(
        int amount
    )
    {
        confidence += amount;

        if (confidence > 100)
            confidence = 100;

        UpdateUI();
    }

    void UpdateUI()
    {
        confidenceSlider.value =
            confidence;

        confidenceText.text =
            confidence + "%";
    }
}