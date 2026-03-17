using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Music")]
    public AudioSource musicSource;
    public AudioClip overworldMusic;
    public AudioClip battleMusic;
    public AudioClip bossMusic;
    public AudioClip victoryJingle;

    [Header("Settings")]
    public float musicVolume = 0.7f;
    public float fadeDuration = 1f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        PlayOverworldMusic();
    }

    public void PlayOverworldMusic() => PlayMusicWithFade(overworldMusic, true);
    public void PlayBattleMusic() => PlayMusicWithFade(battleMusic, true);
    public void PlayBossMusic() => PlayMusicWithFade(bossMusic, true);

    public void PlayVictoryJingle()
    {
       PlayMusicWithFade(victoryJingle, false);
    }

    void PlayMusicWithFade(AudioClip clip, bool loop)
    {
        if (clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return;
        StopAllCoroutines();
        StartCoroutine(FadeToClip(clip,loop));
    }

    IEnumerator FadeToClip(AudioClip clip, bool loop)
    {
        // Fade out current track
        if (musicSource.isPlaying)
        {
            float startVolume = musicSource.volume;
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                musicSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeDuration);
                yield return null;
            }
        }

        // Swap clip
        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.volume = 0f;
        musicSource.Play();

        // Fade in new track
        float fadeElapsed = 0f;
        while (fadeElapsed < fadeDuration)
        {
            fadeElapsed += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(0f, musicVolume, fadeElapsed / fadeDuration);
            yield return null;
        }

        musicSource.volume = musicVolume;
    }

    public void StopMusic()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float startVolume = musicSource.volume;
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeDuration);
            yield return null;
        }
        musicSource.Stop();
        musicSource.clip = null;
    }
    void PlayMusic(AudioClip clip)
    {
        Debug.Log($"PlayMusic called, clip:  {clip} ");
        if (clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return; // already playing

        musicSource.loop = true;
        musicSource.clip = clip;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }
}