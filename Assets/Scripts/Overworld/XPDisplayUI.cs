using UnityEngine;
using TMPro;

public class XPDisplayUI : MonoBehaviour
{
    public TMP_Text xpText;

    void Update()
    {
        if (BattleData.heroStats == null || BattleData.heroStats.Count == 0)
        {
            xpText.text = "";
            return;
        }

        HeroData lead = BattleData.heroStats[0];
        int needed = lead.expToNextLevel - lead.experience;
        xpText.text = $"Lv {lead.level}   EXP: {lead.experience}/{lead.expToNextLevel}   ({needed} to next)";
    }
}