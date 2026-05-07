using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class BaseEnemy : Combatant
{
    public enum Rarity { COMMON, UNCOMMON, RARE, SUPERRARE }

    public Rarity rarity;
    public int expReward;
    public int goldReward;
    public EnemyDefinition definition;

    public override void TakeTurn(TurnManager turnManager)
    {
        //not used
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        if (IsAlive)
        {
            GetComponent<CombatantAnimator>()?.PlayHitAnimation();
        }
        else
        {
            GetComponent<HitEffect>()?.PlayDeathEffect();
            Debug.Log($"{combatantName} was defeated!");
        }
    }

    
    
    public BattleAction PickAction()
    {
        var picked = definition != null ? definition.PickAction() : actions[Random.Range(0, actions.Count)];
        Debug.Log($"[PICK] {combatantName} picked {picked.actionName} (def null? {definition == null})");
        return picked;
    }
}

