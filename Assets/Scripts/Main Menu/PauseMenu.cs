using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance;

    [Header("Panels")]
    public GameObject pausePanel;
    public GameObject settingsPanel;

    [Header("Settings")]
    public Slider volumeSlider;

    [Header("Save feedback")]
    public TMP_Text saveConfirmText;

    private bool isPaused = false;

    void Awake()
    {
        Instance = this;
        pausePanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (settingsPanel.activeSelf)
                OnSettingsBackPressed();
            else
                TogglePause();
        }
    }

    void TogglePause()
    {
        isPaused = !isPaused;
        pausePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void OnResumePressed()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        settingsPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OnSavePressed()
    {
        if (GameManager.Instance?.currentSave == null)
        {
            ShowSaveConfirm("No active save slot!");
            return;
        }

        // Find player position
        GameObject player = GameObject.FindWithTag("Player");
        Vector3 pos = player != null ? player.transform.position : Vector3.zero;

        // Build hero list from BattleData
        // We don't have direct hero references in overworld so save from BattleData
        GameManager.Instance.currentSave.playerX = pos.x;
        GameManager.Instance.currentSave.playerY = pos.y;
        GameManager.Instance.currentSave.playerZ = pos.z;

        // Update heroes from BattleData
        GameManager.Instance.currentSave.heroes.Clear();
        foreach (var h in BattleData.heroStats)
        {
            GameManager.Instance.currentSave.heroes.Add(new HeroSaveData
            {
                heroName = h.heroName,
                currHP = h.currHP,
                currMP = h.currMP,
                maxHP = h.maxHP,
                maxMP = h.maxMP,
                experience = h.experience,
                gold = h.gold,
                level = h.level,
                expToNextLevel = h.expToNextLevel
            });
        }

        SaveSystem.Save(GameManager.Instance.currentSlot, GameManager.Instance.currentSave);
        ShowSaveConfirm("Game Saved!");
    }

    public void OnSettingsPressed()
    {
        settingsPanel.SetActive(true);
        if (volumeSlider != null && AudioManager.Instance != null)
        {
            volumeSlider.onValueChanged.RemoveAllListeners();
            volumeSlider.value = AudioManager.Instance.musicVolume;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }
    }

    public void OnSettingsBackPressed()
    {
        settingsPanel.SetActive(false);
    }

    public void OnVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.musicVolume = value;
            if (AudioManager.Instance.musicSource.isPlaying)
                AudioManager.Instance.musicSource.volume = value;
        }
    }

    public void OnMainMenuPressed()
    {
        Time.timeScale = 1f;
        AudioManager.Instance?.PlayOverworldMusic();
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public void OnQuitPressed()
    {
        Application.Quit();
        Debug.Log("Quit");
    }

    void ShowSaveConfirm(string message)
    {
        if (saveConfirmText == null) return;
        StopAllCoroutines();
        StartCoroutine(SaveConfirmRoutine(message));
    }

    IEnumerator SaveConfirmRoutine(string message)
    {
        saveConfirmText.text = message;
        saveConfirmText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(2f);
        saveConfirmText.gameObject.SetActive(false);
    }
}