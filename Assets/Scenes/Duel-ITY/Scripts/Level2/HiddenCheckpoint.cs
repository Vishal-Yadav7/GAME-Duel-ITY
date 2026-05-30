using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Attach to any hidden trigger zone in the maze.
///
/// ConfidenceBoost  → player walks through = +confidence (collected once, object destroyed).
/// WrongRoute       → player walks through repeatedly = -confidence after N crossings.
///
/// HOW TO SET UP IN UNITY:
/// 1. Create an empty GameObject in the scene, add a Collider2D (set Is Trigger = true).
/// 2. Add this script.
/// 3. Set Checkpoint Type in Inspector.
/// 4. For ConfidenceBoost: set Confidence Boost amount.
/// 5. For WrongRoute: set Wrong Route Penalty and Max Cross Before Penalty.
/// 6. Drag as many of these objects into the scene as you like — no list needed,
///    each one manages itself independently.
/// </summary>
public class HiddenCheckpoint : MonoBehaviour
{
    // =====================================================
    // INSPECTOR FIELDS
    // =====================================================

    public enum CheckpointType
    {
        ConfidenceBoost,
        WrongRoute
    }

    [Header("Checkpoint Type")]
    public CheckpointType checkpointType = CheckpointType.ConfidenceBoost;

    [Header("Confidence Boost Settings")]
    [Tooltip("How much confidence the player gains on collection.")]
    public int confidenceBoost = 5;

    [Header("Wrong Route Settings")]
    [Tooltip("Confidence removed each time the penalty fires.")]
    public int wrongRoutePenalty = 7;

    [Tooltip("How many times the player must cross before penalty triggers.")]
    public int maxCrossBeforePenalty = 2;

    [Header("Visual Feedback (optional)")]
    [Tooltip("Optional particle or glow object to disable after collection.")]
    public GameObject collectEffect;

    // =====================================================
    // PRIVATE STATE
    // =====================================================

    private bool collected = false;   // for ConfidenceBoost: one-shot
    private int crossCount = 0;      // for WrongRoute: counts crossings

    // =====================================================
    // TRIGGER
    // =====================================================

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        switch (checkpointType)
        {
            case CheckpointType.ConfidenceBoost:
                HandleConfidenceBoost();
                break;

            case CheckpointType.WrongRoute:
                HandleWrongRoute();
                break;
        }
    }

    // =====================================================
    // CONFIDENCE BOOST  (collected once → object destroyed)
    // =====================================================

    void HandleConfidenceBoost()
    {
        if (collected) return;
        collected = true;

        if (MazeLevelManager.instance != null)
            MazeLevelManager.instance.AddConfidence(confidenceBoost);

        Debug.Log($"[Checkpoint] ConfidenceBoost collected: +{confidenceBoost}");

        // Play collect effect if assigned
        if (collectEffect != null)
            collectEffect.SetActive(false);

        // Destroy the trigger zone so it can never fire again
        Destroy(gameObject);
    }

    // =====================================================
    // WRONG ROUTE  (penalizes after N crossings, resets counter)
    // =====================================================

    void HandleWrongRoute()
    {
        crossCount++;

        Debug.Log($"[Checkpoint] WrongRoute crossed {crossCount}/{maxCrossBeforePenalty}");

        if (crossCount >= maxCrossBeforePenalty)
        {
            if (MazeLevelManager.instance != null)
                MazeLevelManager.instance.RemoveConfidence(wrongRoutePenalty);

            Debug.Log($"[Checkpoint] WrongRoute penalty: -{wrongRoutePenalty}");

            crossCount = 0; // reset so it can penalize again if player keeps going wrong way
        }
    }

#if UNITY_EDITOR
    // =====================================================
    // EDITOR GIZMO — shows zone colour in Scene view
    // =====================================================

    private void OnDrawGizmos()
    {
        Gizmos.color = checkpointType == CheckpointType.ConfidenceBoost
            ? new Color(0f, 1f, 0f, 0.25f)   // green  = boost
            : new Color(1f, 0f, 0f, 0.25f);   // red    = wrong route

        // Draw a flat cube over the collider bounds
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            Gizmos.DrawCube(col.bounds.center, col.bounds.size);
        else
            Gizmos.DrawCube(transform.position, Vector3.one);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = checkpointType == CheckpointType.ConfidenceBoost
            ? new Color(0f, 1f, 0f, 0.6f)
            : new Color(1f, 0f, 0f, 0.6f);

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
        else
            Gizmos.DrawWireCube(transform.position, Vector3.one);
    }
#endif
}