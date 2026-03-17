using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MainMenuUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject saveSlotPanel;
    public GameObject settingsPanel;

    [Header("Save Slot Buttons")]
    public Button slot1Button;
    public Button slot2Button;
    public Button slot3Button;
    public TMP_Text slot1Text;
    public TMP_Text slot2Text;
    public TMP_Text slot3Text;
    public Button saveSlotBackButton;

    [Header("Settings")]
    public Slider volumeSlider;
    public Button settingsBackButton;

    [Header("Fade")]
    public Image fadeImage;
    public float fadeDuration = 1f;

    private bool isNewGame = false;

    void Start()
    {
        ShowPanel(mainPanel);
        StartCoroutine(FadeIn());
    }

    // Main buttons
    public void OnNewGamePressed()
    {
        isNewGame = true;
        RefreshSaveSlots();
        ShowPanel(saveSlotPanel);
    }

    public void OnContinuePressed()
    {
        isNewGame = false;
        RefreshSaveSlots();
        ShowPanel(saveSlotPanel);
    }

    public void OnSettingsPressed()
    {
        ShowPanel(settingsPanel);
        if (volumeSlider != null && AudioManager.Instance != null)
        {
            volumeSlider.onValueChanged.RemoveAllListeners();
            volumeSlider.value = AudioManager.Instance.musicVolume;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }
    }

    public void OnSaveSlotBackPressed()
    {
        ShowPanel(mainPanel);
    }

    public void OnSettingsBackPressed()
    {
        ShowPanel(mainPanel);
    }

    public void OnQuitPressed()
    {
        Application.Quit();
        Debug.Log("Quit");
    }

    // Settings
    public void OnVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.musicVolume = value;
            if (AudioManager.Instance.musicSource.isPlaying)
            {
                AudioManager.Instance.musicSource.volume = value;
            }
        }
    }

    // Save slots
    void RefreshSaveSlots()
    {
        UpdateSlotText(slot1Text, 0);
        UpdateSlotText(slot2Text, 1);
        UpdateSlotText(slot3Text, 2);

        // If continuing, grey out empty slots
        if (!isNewGame)
        {
            slot1Button.interactable = SaveSystem.SlotExists(0);
            slot2Button.interactable = SaveSystem.SlotExists(1);
            slot3Button.interactable = SaveSystem.SlotExists(2);
        }
        else
        {
            slot1Button.interactable = true;
            slot2Button.interactable = true;
            slot3Button.interactable = true;
        }
    }

    void UpdateSlotText(TMP_Text text, int slot)
    {
        if (text == null) return;
        if (SaveSystem.SlotExists(slot))
        {
            SaveData data = SaveSystem.Load(slot);
            text.text = $"Slot {slot + 1}  |  LVL {(data.heroes.Count > 0 ? data.heroes[0].level : 1)}  |  {data.saveDateTime}";
        }
        else
        {
            text.text = $"Slot {slot + 1}  |  Empty";
        }
    }

    public void OnSlotSelected(int slot)
    {
        if (isNewGame)
        {
            GameManager.Instance.NewGame(slot);
            StartCoroutine(LoadOverworld());
        }
        else
        {
            if (SaveSystem.SlotExists(slot))
            {
                GameManager.Instance.LoadGame(slot);
                ApplySaveToHeroData();
                StartCoroutine(LoadOverworld());
            }
        }
    }

    void ApplySaveToHeroData()
    {
        SaveData save = GameManager.Instance.currentSave;
        if (save == null) return;

        BattleData.heroStats.Clear();
        foreach (var h in save.heroes)
        {
            BattleData.heroStats.Add(new HeroData
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

        // Restore defeated enemies
        foreach (var id in save.defeatedEnemyIDs)
            BattleData.defeatedEnemyIDs.Add(id);

        // Restore player position
        BattleData.playerReturnPosition = GameManager.Instance.GetPlayerPosition();
    }

    void ShowPanel(GameObject panel)
    {
        mainPanel.SetActive(false);
        saveSlotPanel.SetActive(false);
        settingsPanel.SetActive(false);
        panel.SetActive(true);
    }

    IEnumerator LoadOverworld()
    {
        yield return StartCoroutine(FadeOut());
        UnityEngine.SceneManagement.SceneManager.LoadScene("Overworld");
    }

    IEnumerator FadeIn()
    {
        float elapsed = 0f;
        Color c = fadeImage.color;
        c.a = 1f;
        fadeImage.color = c;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = 1f - Mathf.Clamp01(elapsed / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }
        c.a = 0f;
        fadeImage.color = c;
    }

    IEnumerator FadeOut()
    {
        float elapsed = 0f;
        Color c = fadeImage.color;
        c.a = 0f;
        fadeImage.color = c;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Clamp01(elapsed / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }
    }
}