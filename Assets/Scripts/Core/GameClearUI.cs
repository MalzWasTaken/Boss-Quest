using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameClearUI : MonoBehaviour
{
    public static GameClearUI Instance;

    [Header("Panel")]
    public GameObject clearPanel;

    [Header("Stats")]
    public TMP_Text titleText;
    public TMP_Text levelText;
    public TMP_Text goldText;
    public TMP_Text flavorText;

    [Header("Button")]
    public Button mainMenuButton;

    [Header("Fade")]
    public Image fadeImage;
    public float fadeInDuration = 1.5f;

    void Awake()
    {
        Instance = this;
        if (clearPanel != null) clearPanel.SetActive(false);
    }

    public void ShowGameClear()
    {
        StartCoroutine(ShowGameClearRoutine());
    }

    IEnumerator ShowGameClearRoutine()
    {
        yield return StartCoroutine(FadeIn());

        if (clearPanel != null) clearPanel.SetActive(true);

        int highestLevel = 1;
        int totalGold = 0;
        foreach (var hero in BattleData.heroStats)
        {
            if (hero.level > highestLevel) highestLevel = hero.level;
            totalGold += hero.gold;
        }

        if (titleText != null) titleText.text = "The Overlord has fallen.";
        if (levelText != null) levelText.text = $"Party Level: {highestLevel}";
        if (goldText != null) goldText.text = $"Gold: {totalGold}";
        if (flavorText != null) flavorText.text = "Your divine powers are restored.\nThe world is saved.";

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(() =>
            {
                BattleData.heroStats.Clear();
                BattleData.defeatedEnemyIDs.Clear();
                BattleData.isFinalBoss = false;
                Time.timeScale = 1f;
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
            });

        AudioManager.Instance?.PlayVictoryJingle();
    }

    IEnumerator FadeIn()
    {
        float elapsed = 0f;
        Color c = fadeImage.color;
        c.a = 0f;
        fadeImage.color = c;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Clamp01(elapsed / fadeInDuration);
            fadeImage.color = c;
            yield return null;
        }
    }
}
