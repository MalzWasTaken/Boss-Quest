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

    public void PlayOverworldMusic()
    {
        PlayMusic(overworldMusic);
    }

    public void PlayBattleMusic()
    {
        Debug.Log("PlayBattleMusic called");
        PlayMusic(battleMusic);
    }

    public void PlayBossMusic()
    {
        PlayMusic(bossMusic);
    }

    public void PlayVictoryJingle()
    {
        musicSource.loop = false;
        musicSource.clip = victoryJingle;
        musicSource.volume = musicVolume;
        musicSource.Play();
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