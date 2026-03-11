using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;


public class BattleResultUI : MonoBehaviour {
    public static BattleResultUI Instance;

    [Header("Panels")]
    public GameObject victoryPanel;
    public GameObject gameOverPanel;

    [Header("Victory UI")]
    public TMP_Text victoryExpText;
    public TMP_Text victoryGoldText;
    public TMP_Text victoryLevelUpText;
    public Button victoryContinueButton;

    [Header("Game Over UI")]
    public Button gameOverReturnButton;

    [Header("Shared")]
    public Image fadeImage;
    public float fadeInDuration = 1f;

    void Awake()
    {
        Instance = this;
        victoryPanel.SetActive(false);
        gameOverPanel.SetActive(false);
    } 

    public void ShowVictory(int expGained, int goldGained, List<string> levelUps)
    {
        StartCoroutine(ShowVictoryRoutine(expGained, goldGained, levelUps));
    }

    public void ShowGameOver()
    {
        StartCoroutine(ShowGameOverRoutine());
    }

    IEnumerator ShowVictoryRoutine(int expGained, int goldGained, List<string> levelUps)
    {
        yield return StartCoroutine(FadeIn());

        victoryPanel.SetActive(true);
        victoryExpText.text = $"EXP Gained: {expGained}";
        victoryGoldText.text = $"Gold Gained: {goldGained}";

        if(levelUps.Count > 0)
            victoryLevelUpText.text = string.Join("\n", levelUps);
        else
            victoryLevelUpText.text = "";
        
        victoryContinueButton.onClick.AddListener(() =>
        {
            StartCoroutine(ReturnToOverworld());
        });

    }

    IEnumerator ShowGameOverRoutine()
    {
        yield return StartCoroutine(FadeIn());
        gameOverPanel.SetActive(true);

        gameOverReturnButton.onClick.AddListener(() =>
        {
            StartCoroutine(ReturnToOverworld());
        });
    }

    IEnumerator FadeIn()
    {
        float elapsed = 0f;
        Color color = fadeImage.color;
        color.a = 0f;
        fadeImage.color = color;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsed / fadeInDuration);
            fadeImage.color = color;
            yield return null;
        }
    }

    IEnumerator ReturnToOverworld()
    {
        AudioManager.Instance?.PlayOverworldMusic();
        BattleData.heroStats.Clear();
        foreach (var hero in BattleManager.Instance.heroes)
        {
            BattleData.heroStats.Add(new HeroData
            {
                heroName = hero.combatantName,
                currHP = hero.currHP,
                currMP = hero.currMP,
                experience = hero.experience,
                gold = hero.gold,
                level = hero.level,
                expToNextLevel = hero.expToNextLevel
            });
        }
        BattleData.enemiesToSpawn.Clear();
        UnityEngine.SceneManagement.SceneManager.LoadScene(BattleData.returnScene);
        yield return null;
    }
    
}