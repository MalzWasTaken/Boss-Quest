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
}