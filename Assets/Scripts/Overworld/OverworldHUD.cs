using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class OverworldHUD : MonoBehaviour
{
    [System.Serializable]
    public class HeroHUDEntry
    {
        [Header("Collapsed")]
        public Slider hpBarCollapsed;
        public Slider mpBarCollapsed;

        [Header("Expanded")]
        public GameObject expandedPanel;
        public TMP_Text heroName;
        public TMP_Text levelText;
        public TMP_Text hpText;
        public TMP_Text mpText;
        public Slider hpBarExpanded;
        public Slider mpBarExpanded;
    }

    public List<HeroHUDEntry> entries;

    private bool isExpanded = false;

    void Start()
    {
        RefreshLayout();
    }

    void Update()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            isExpanded = !isExpanded;
            RefreshLayout();
        }

        UpdateBars();
    }

    void RefreshLayout()
    {
        foreach (var entry in entries)
        {
            if (entry.expandedPanel != null)
                entry.expandedPanel.SetActive(isExpanded);
        }
    }

    void UpdateBars()
    {
        for (int i = 0; i < entries.Count; i++)
        {
            // Use BattleData.heroStats if available, otherwise skip
            if (i >= BattleData.heroStats.Count) continue;

            HeroData data = BattleData.heroStats[i];
            HeroHUDEntry entry = entries[i];

            // Need max values — store them in HeroData
            float hpRatio = data.maxHP > 0 ? data.currHP / data.maxHP : 1f;
            float mpRatio = data.maxMP > 0 ? data.currMP / data.maxMP : 1f;

            if (entry.hpBarCollapsed != null) entry.hpBarCollapsed.value = hpRatio;
            if (entry.mpBarCollapsed != null) entry.mpBarCollapsed.value = mpRatio;
            if (entry.heroName != null) entry.heroName.text = data.heroName;

            if (isExpanded)
            {
                if (entry.hpBarExpanded != null) entry.hpBarExpanded.value = hpRatio;
                if (entry.mpBarExpanded != null) entry.mpBarExpanded.value = mpRatio;
                if (entry.levelText != null) entry.levelText.text = $"LVL {data.level}";
                if (entry.hpText != null) entry.hpText.text = $"{Mathf.CeilToInt(data.currHP)}/{Mathf.CeilToInt(data.maxHP)}";
                if (entry.mpText != null) entry.mpText.text = $"{Mathf.CeilToInt(data.currMP)}/{Mathf.CeilToInt(data.maxMP)}";
            }
        }
    }
}