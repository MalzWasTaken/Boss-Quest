using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HeroBarUI : MonoBehaviour
{
    [Header("Hero Reference")]
    public BaseHero hero;

    [Header("UI Elements")]
    public TMP_Text heroName;
    public TMP_Text hpValue;
    public TMP_Text mpValue;
    public Slider hpBar;
    public Slider mpBar;

    void Update()
    {
        if (hero == null) return;

        heroName.text = hero.combatantName;
        hpValue.text = $"{(int)hero.currHP}/{(int)hero.maxHP}";
        mpValue.text = $"{(int)hero.currMP}/{(int)hero.maxMP}";
        hpBar.value = hero.currHP / hero.maxHP;
        mpBar.value = hero.currMP / hero.maxMP;
    }
}