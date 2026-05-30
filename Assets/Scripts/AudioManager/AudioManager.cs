using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("All Sounds")]
    public Sound[] sounds;

    [Header("Background Volume")]
    [Range(0f, 1f)]
    public float normalBackgroundVolume = 0.45f;

    [Range(0f, 1f)]
    public float cutsceneBackgroundVolume = 0.08f;

    void Awake()
    {
        // SINGLETON
        if (instance != null &&
            instance != this)
        {
            Destroy(gameObject);

            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);

        // CREATE AUDIO SOURCES
        foreach (Sound s in sounds)
        {
            s.source =
                gameObject
                .AddComponent<AudioSource>();

            s.source.clip =
                s.clip;

            s.source.volume =
                s.volume;

            s.source.pitch =
                s.pitch;

            s.source.loop =
                s.loop;

            s.source.playOnAwake =
                false;
        }
    }

    void Start()
    {
        // AUTO PLAY BACKGROUND
        Play("Background");
    }

    // PLAY SOUND
    public void Play(string soundName)
    {
        Sound s =
            Array.Find(
                sounds,
                item => item.name ==
                soundName
            );

        if (s == null)
        {
            Debug.LogWarning(
                "Sound Missing : " +
                soundName
            );

            return;
        }

        // PREVENT DOUBLE PLAY
        if (!s.source.isPlaying)
        {
            s.source.Play();
        }
    }

    // STOP SOUND
    public void Stop(string soundName)
    {
        Sound s =
            Array.Find(
                sounds,
                item => item.name ==
                soundName
            );

        if (s == null)
            return;

        s.source.Stop();
    }

    // PLAY ONE SHOT
    public void PlayOneShot(
        string soundName
    )
    {
        Sound s =
            Array.Find(
                sounds,
                item => item.name ==
                soundName
            );

        if (s == null)
            return;

        s.source.PlayOneShot(
            s.clip
        );
    }

    // CUTSCENE MODE ON
    public void CutsceneModeOn()
    {
        // LOWER BACKGROUND SOUND
        SetVolume(
            "Background",
            cutsceneBackgroundVolume
        );
    }

    // CUTSCENE MODE OFF
    public void CutsceneModeOff()
    {
        // NORMAL BACKGROUND SOUND
        SetVolume(
            "Background",
            normalBackgroundVolume
        );
    }

    // SET SOUND VOLUME
    public void SetVolume(
        string soundName,
        float volume
    )
    {
        Sound s =
            Array.Find(
                sounds,
                item => item.name ==
                soundName
            );

        if (s == null)
            return;

        s.source.volume =
            volume;
    }

    // FADE TO LOW
    public void FadeToCutscene()
    {
        StopAllCoroutines();

        StartCoroutine(
            FadeBackground(
                cutsceneBackgroundVolume
            )
        );
    }

    // FADE TO NORMAL
    public void FadeToNormal()
    {
        StopAllCoroutines();

        StartCoroutine(
            FadeBackground(
                normalBackgroundVolume
            )
        );
    }

    // SMOOTH FADE
    System.Collections.IEnumerator
    FadeBackground(
        float targetVolume
    )
    {
        Sound bg =
            Array.Find(
                sounds,
                item => item.name ==
                "Background"
            );

        if (bg == null)
            yield break;

        float startVolume =
            bg.source.volume;

        float time = 0f;

        while (time < 1f)
        {
            time +=
                Time.deltaTime * 2f;

            bg.source.volume =
                Mathf.Lerp(
                    startVolume,
                    targetVolume,
                    time
                );

            yield return null;
        }

        bg.source.volume =
            targetVolume;
    }
}