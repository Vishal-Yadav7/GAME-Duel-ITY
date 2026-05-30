using UnityEngine;
using UnityEngine.UI;

public class WinPanelTextManager : MonoBehaviour
{
    [Header("Title Texts")]
    public Text title1;
    public Text title2;
    public Text title3;
    public Text title4;

    [Header("Subtitle Texts")]
    public Text subtitle1;
    public Text subtitle2;
    public Text subtitle3;
    public Text subtitle4;

    void OnEnable()
    {
        // Line 1
        if (title1 != null)
            title1.text = "She spent the entire day working...";

        if (subtitle1 != null)
            subtitle1.text = "Yet she still cared for everyone.";

        // Line 2
        if (title2 != null)
            title2.text = "No one asked how tired she was.";

        if (subtitle2 != null)
            subtitle2.text = "But she continued with a smile.";

        // Line 3
        if (title3 != null)
            title3.text = "Sometimes love is not spoken.";

        if (subtitle3 != null)
            subtitle3.text = "Sometimes it is shown through actions.";

        // Line 4
        if (title4 != null)
            title4.text = "Sometimes love is served on a plate.";

        if (subtitle4 != null)
            subtitle4.text = "Every small act of care matters.";
    }
}