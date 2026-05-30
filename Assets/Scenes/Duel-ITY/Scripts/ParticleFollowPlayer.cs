using UnityEngine;

public class ParticleFollowPlayer : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    [Header("Offset")]
    public Vector3 offset =
        new Vector3(0f, 0f, 0f);

    [Header("Smooth Follow")]
    public float followSpeed = 8f;

    void LateUpdate()
    {
        // NO PLAYER
        if (player == null)
            return;

        // TARGET POSITION
        Vector3 targetPosition =
            player.position + offset;

        // KEEP PARTICLE Z
        targetPosition.z =
            transform.position.z;

        // SMOOTH FOLLOW
        transform.position =
            Vector3.Lerp(
                transform.position,
                targetPosition,
                followSpeed * Time.deltaTime
            );
    }
}