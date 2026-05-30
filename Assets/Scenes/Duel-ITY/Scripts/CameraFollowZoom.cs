using UnityEngine;

public class CameraFollowZoom : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    [Header("Camera")]
    public Camera cam;

    [Header("Follow")]
    public bool followPlayer = false;

    public float followSpeed = 5f;

    [Header("Zoom")]
    public float gameplayZoom = 3f;

    public float fullMapZoom = 8f;

    public float zoomSpeed = 5f;

    [Header("Map Button")]
    public bool holdZoomOut = false;

    private bool gameplayStarted = false;

    void Start()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }

        // START FULL MAP
        cam.orthographicSize =
            fullMapZoom;
    }

    void LateUpdate()
    {
        // FOLLOW PLAYER
        if (followPlayer &&
            gameplayStarted &&
            player != null)
        {
            Vector3 targetPos =
                new Vector3(
                    player.position.x,
                    player.position.y,
                    -10f
                );

            transform.position =
                Vector3.Lerp(
                    transform.position,
                    targetPos,
                    followSpeed *
                    Time.deltaTime
                );
        }

        // BEFORE GAMEPLAY
        if (!gameplayStarted)
        {
            cam.orthographicSize =
                Mathf.Lerp(
                    cam.orthographicSize,
                    fullMapZoom,
                    zoomSpeed *
                    Time.deltaTime
                );

            return;
        }

        // AFTER GAMEPLAY
        float targetZoom =
            holdZoomOut
            ? fullMapZoom
            : gameplayZoom;

        cam.orthographicSize =
            Mathf.Lerp(
                cam.orthographicSize,
                targetZoom,
                zoomSpeed *
                Time.deltaTime
            );
    }

    // START GAMEPLAY
    public void StartGameplayCamera()
    {
        gameplayStarted = true;

        followPlayer = true;
    }

    // HOLD BUTTON
    public void HoldZoomOut()
    {
        holdZoomOut = true;
    }

    // RELEASE BUTTON
    public void ReleaseZoomOut()
    {
        holdZoomOut = false;
    }
}