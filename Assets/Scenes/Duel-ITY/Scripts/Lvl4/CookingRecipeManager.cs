using System.Collections;
using UnityEngine;

public class CookingRecipeManager : MonoBehaviour
{
    [Header("Managers")]
    public CookingGameManager gameManager;
    public HoldCookButton cookButton;

    // =========================================
    // WRONG POPUP
    // =========================================

    public GameObject wrongCross;

    // =========================================
    // OMELETTE objects
    // Hierarchy: egg tray empty (parent)
    //               └─ egg tray full  (child)
    //            broken egg (parent)
    //               └─ single egg(1)  (child)
    //            Egg pan (parent)
    //               └─ Omlet ready    (child)
    // =========================================

    public GameObject eggTrayFull;     // child of egg tray empty
    public GameObject eggTrayEmpty;    // parent (always visible start)
    public GameObject brokenEgg;       // parent  — hidden at start, shown on tray tap
    public GameObject singleEgg;       // child of brokenEgg — hidden on first double-tap
    public GameObject eggPan;          // Egg pan parent — visible during omelette
    public GameObject omeletReady;     // child of Egg pan — shown after broken egg tap

    // =========================================
    // TEA objects
    // Hierarchy: TeaLeave, Water, milk pkt,
    //            TpanMilk, Tpan.Empty, pan water, tea ready
    // =========================================

    public GameObject teaLeave;        // tap-able tea leaves ingredient
    public GameObject water;           // tap-able water (Tea)
    public GameObject milkPkt;         // tap-able milk packet
    public GameObject tPanMilk;        // pan showing milk — hidden at start
    public GameObject tPanEmpty;       // tea pan empty — shown when Tea starts
    public GameObject panWater;        // pan showing water — shown on water tap
    public GameObject teaReady;        // final tea — shown after tea leaves tap

    // =========================================
    // MAGGIE objects
    // Hierarchy: Water, Pkts, Mpan, panWater, Ready
    // =========================================

    public GameObject mWater;          // tap-able maggie water ingredient
    public GameObject packets;         // tap-able maggie packet
    public GameObject mPan;            // maggie pan — visible when Maggie starts
    public GameObject mPanWater;       // pan with water — shown on water tap
    public GameObject ready;           // final maggie — shown after packet tap

    // =========================================

    enum Recipe { None, Omelette, Tea, Maggie }
    Recipe currentRecipe;
    int step = 0;

    // Double-tap counters
    int singleEggTaps = 0;
    int brokenEggTaps = 0;

    // =========================================
    // INITIALISE — call from Awake/Start order
    // Everything starts hidden except what
    // should be visible at scene open.
    // =========================================

    void Awake()
    {
        // --- OMELETTE ---
        // eggTrayEmpty visible, eggTrayFull visible (child), brokenEgg hidden,
        // singleEgg hidden, eggPan visible, omeletReady hidden
        if (eggTrayEmpty != null) eggTrayEmpty.SetActive(true);
        if (eggTrayFull != null) eggTrayFull.SetActive(true);
        if (brokenEgg != null) brokenEgg.SetActive(false);
        if (singleEgg != null) singleEgg.SetActive(false);
        if (eggPan != null) eggPan.SetActive(true);
        if (omeletReady != null) omeletReady.SetActive(false);

        // --- TEA ---
        if (teaLeave != null) teaLeave.SetActive(true);
        if (water != null) water.SetActive(true);
        if (milkPkt != null) milkPkt.SetActive(true);
        if (tPanEmpty != null) tPanEmpty.SetActive(false); // shown when Tea starts
        if (panWater != null) panWater.SetActive(false);
        if (tPanMilk != null) tPanMilk.SetActive(false);
        if (teaReady != null) teaReady.SetActive(false);

        // --- MAGGIE ---
        if (mWater != null) mWater.SetActive(true);
        if (packets != null) packets.SetActive(true);
        if (mPan != null) mPan.SetActive(false);    // shown when Maggie starts
        if (mPanWater != null) mPanWater.SetActive(false);
        if (ready != null) ready.SetActive(false);
    }

    // =========================================
    // START RECIPES
    // =========================================

    public void StartOmelette()
    {
        currentRecipe = Recipe.Omelette;
        step = 0;
        singleEggTaps = 0;
        brokenEggTaps = 0;

        gameManager.taskText.text = "Prepare Omelette";
        gameManager.hintText.text = "Tap Egg Tray";

        gameManager.SetCookReady(false);
    }

    public void StartTea()
    {
        currentRecipe = Recipe.Tea;
        step = 0;

        gameManager.taskText.text = "Prepare Tea";
        gameManager.hintText.text = "Tap Water";

        // Force hide ALL omelette cooking objects
        // eggTrayEmpty is intentionally NOT touched — stays visible on counter
        if (eggTrayFull != null) eggTrayFull.SetActive(false);
        if (brokenEgg != null) brokenEgg.SetActive(false);
        if (singleEgg != null) singleEgg.SetActive(false);
        if (eggPan != null) eggPan.SetActive(false);
        if (omeletReady != null) omeletReady.SetActive(false);

        // Show tea pan empty — player taps water next
        if (tPanEmpty != null) tPanEmpty.SetActive(true);

        gameManager.SetCookReady(false);
    }

    public void StartMaggie()
    {
        currentRecipe = Recipe.Maggie;
        step = 0;

        gameManager.taskText.text = "Prepare Maggie";
        gameManager.hintText.text = "Tap Water";

        // Hide tea pan objects
        // eggTrayEmpty intentionally NOT touched — stays visible on counter
        if (tPanEmpty != null) tPanEmpty.SetActive(false);
        if (teaReady != null) teaReady.SetActive(false);

        // water, teaLeave, milkPkt stay visible (they sit on counter naturally)

        // Show maggie pan — player taps water next
        if (mPan != null) mPan.SetActive(true);

        gameManager.SetCookReady(false);
    }

    // =========================================
    // OMELETTE STEPS
    // =========================================

    // Step 0 — Tap egg tray full
    public void TapEggTray()
    {
        if (currentRecipe != Recipe.Omelette || step != 0)
        {
            WrongClick();
            return;
        }

        // eggTrayFull hides (tray now empty — parent stays visible showing empty tray)
        eggTrayFull.SetActive(false);

        // Show broken egg parent + single egg child
        brokenEgg.SetActive(true);
        singleEgg.SetActive(true);

        step = 1;
        gameManager.hintText.text = "Double Tap Single Egg";
    }

    // Step 1 — Double tap single egg
    public void TapSingleEgg()
    {
        if (currentRecipe != Recipe.Omelette || step != 1)
        {
            WrongClick();
            return;
        }

        singleEggTaps++;

        if (singleEggTaps == 1)
        {
            // First tap — give feedback
            gameManager.hintText.text = "Tap Again!";
            return;
        }

        // Second tap — hide single egg, move to next step
        singleEgg.SetActive(false);
        singleEggTaps = 0;

        step = 2;
        gameManager.hintText.text = "Double Tap Broken Egg";
    }

    // Step 2 — Double tap broken egg (parent)
    public void TapBrokenEgg()
    {
        if (currentRecipe != Recipe.Omelette || step != 2)
        {
            WrongClick();
            return;
        }

        brokenEggTaps++;

        if (brokenEggTaps == 1)
        {
            gameManager.hintText.text = "Tap Again!";
            return;
        }

        // Second tap — hide broken egg, show omelet ready
        brokenEgg.SetActive(false);
        omeletReady.SetActive(true);
        brokenEggTaps = 0;

        step = 3;
        gameManager.hintText.text = "Hold Cook Button";

        // Unlock cook button
        gameManager.SetCookReady(true);
    }

    // =========================================
    // TEA STEPS
    // =========================================

    // Step 0 — Tap water
    public void TapWater()
    {
        if (currentRecipe == Recipe.Tea && step == 0)
        {
            // Hide empty tea pan, show pan with water
            tPanEmpty.SetActive(false);
            panWater.SetActive(true);

            step = 1;
            gameManager.hintText.text = "Tap Milk Packet";
            return;
        }

        if (currentRecipe == Recipe.Maggie && step == 0)
        {
            // Hide empty maggie pan, show pan with water
            mPan.SetActive(false);
            mPanWater.SetActive(true);

            step = 1;
            gameManager.hintText.text = "Tap Maggie Packet";
            return;
        }

        // Wrong during Omelette or wrong step
        WrongClick();
    }

    // Step 1 — Tap milk packet
    public void TapMilk()
    {
        if (currentRecipe != Recipe.Tea || step != 1)
        {
            WrongClick();
            return;
        }

        // Hide pan water, show tpan milk
        panWater.SetActive(false);
        tPanMilk.SetActive(true);

        step = 2;
        gameManager.hintText.text = "Tap Tea Leaves";
    }

    // Step 2 — Tap tea leaves
    public void TapTeaLeaves()
    {
        if (currentRecipe != Recipe.Tea || step != 2)
        {
            WrongClick();
            return;
        }

        // Hide tpan milk, show tea ready
        tPanMilk.SetActive(false);
        teaReady.SetActive(true);

        step = 3;
        gameManager.hintText.text = "Hold Cook Button";

        // Unlock cook button
        gameManager.SetCookReady(true);
    }

    // =========================================
    // MAGGIE STEPS
    // =========================================

    // Step 0 — Water handled in TapWater() above

    // Step 1 — Tap packet
    public void TapMaggiePacket()
    {
        if (currentRecipe != Recipe.Maggie || step != 1)
        {
            WrongClick();
            return;
        }

        // Hide pan water, show ready
        mPanWater.SetActive(false);
        ready.SetActive(true);

        step = 2;
        gameManager.hintText.text = "Hold Cook Button";

        // Unlock cook button
        gameManager.SetCookReady(true);
    }

    // =========================================
    // WRONG INGREDIENT GUARDS
    // These are called from the ingredient
    // GameObjects' button/click handlers.
    // We handle wrong-recipe taps here by
    // routing everything through TapWater etc.
    // — wrong recipe/step already calls WrongClick().
    //
    // Extra explicit guards for clarity:
    // =========================================

    // Called by egg tray object button
    // Already guarded in TapEggTray() — wrong during Tea/Maggie shows cross

    // Called by milk packet object button
    // Already guarded in TapMilk() — wrong during Omelette/Maggie shows cross

    // Called by tea leaves object button
    // Already guarded in TapTeaLeaves() — wrong during Omelette/Maggie shows cross

    // Called by maggie packet object button
    // Already guarded in TapMaggiePacket() — wrong during Omelette/Tea shows cross

    // =========================================
    // COOK FINISHED (called by GameManager
    // after hold-slider reaches 100)
    // =========================================

    public void CookingFinished()
    {
        cookButton.ResetCook();

        if (currentRecipe == Recipe.Omelette && step == 3)
            StartCoroutine(OmeletteDone());

        else if (currentRecipe == Recipe.Tea && step == 3)
            StartCoroutine(TeaDone());

        else if (currentRecipe == Recipe.Maggie && step == 2)
            StartCoroutine(MaggieDone());
    }

    IEnumerator OmeletteDone()
    {
        gameManager.hintText.text = "Good Job!";
        yield return new WaitForSeconds(1f);

        // Hide pan and ready — keep eggTrayEmpty visible on counter
        if (eggPan != null) eggPan.SetActive(false);
        if (omeletReady != null) omeletReady.SetActive(false);

        gameManager.HusbandThankYou();
    }

    IEnumerator TeaDone()
    {
        gameManager.hintText.text = "Good Job!";
        yield return new WaitForSeconds(1f);
        gameManager.MotherThankYou();
    }

    IEnumerator MaggieDone()
    {
        gameManager.hintText.text = "Good Job!";
        yield return new WaitForSeconds(1f);
        gameManager.ChildThankYou();
    }

    // =========================================
    // WRONG CLICK
    // =========================================

    private Coroutine wrongCrossRoutine;

    void WrongClick()
    {
        Confidence.instance.RemoveConfidence(5);

        // Stop previous popup if still showing, then restart
        if (wrongCrossRoutine != null)
            StopCoroutine(wrongCrossRoutine);

        wrongCrossRoutine = StartCoroutine(ShowWrongCross());
    }

    IEnumerator ShowWrongCross()
    {
        wrongCross.SetActive(false); // reset first in case mid-animation
        wrongCross.SetActive(true);

        iTween.ScaleFrom(
            wrongCross,
            iTween.Hash(
                "scale", Vector3.zero,
                "time", 0.2f,
                "easetype", iTween.EaseType.easeOutBack
            )
        );

        yield return new WaitForSeconds(0.7f);

        wrongCross.SetActive(false);
        wrongCrossRoutine = null;
    }
}