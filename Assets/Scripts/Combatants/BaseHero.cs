using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class BaseHero : Combatant
{
    public int stamina;
    public int intellect;
    public int dexterity;
    public List<BattleAction> abilities;
    public int experience;
    public int gold;
    public int level = 1;
    public int expToNextLevel = 100;
    
    public override void TakeTurn(TurnManager turnManager)
    {
        // not used
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        GetComponent<HeroAnimator>()?.PlayHit();
        if (!IsAlive)
        {
            GetComponent<HeroAnimator>()?.PlayDeath();
            Debug.Log($"{combatantName} died!");
            //battle log message
            BattleLogUI.Instance?.AddMessage($"{combatantName} died!");
        }
            

    }
}
    