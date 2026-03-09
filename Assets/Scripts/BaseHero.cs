using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class BaseHero : Combatant
{
    public int stamina;
    public int intellect;
    public int dexterity;

    public List<BattleAction> abilities;

    
    public override void TakeTurn(TurnManager turnManager)
    {
        // not used
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        if(!IsAlive)
            Debug.Log($"{combatantName} died!");
            if (BattleLog.Instance != null)
            BattleLog.Instance.AddMessage($"{combatantName} died!");
    }
}
    