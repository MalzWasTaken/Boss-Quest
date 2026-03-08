using UnityEngine;

[System.Serializable]
public class BaseHero : Combatant
{
    public int stamina;
    public int intellect;
    public int dexterity;

    // TakeTurn no longer used - actions are pre-selected now
    public override void TakeTurn(TurnManager turnManager)
    {
        // This shouldn't be called in the new DQ9-style system
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        if(!IsAlive)
            Debug.Log($"{combatantName} died!");
            BattleLog.Instance.AddMessage($"{combatantName} died!");
    }
}
    