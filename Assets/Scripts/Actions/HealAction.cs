using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Battle/Actions/Heal")]
public class HealAction : BattleAction
{
    public float healAmount = 30f;
    public override void Execute (Combatant user, List<Combatant> targets)
    {
        foreach(var target in targets)
        {
            target.Heal(healAmount);
            BattleLogUI.Instance?.AddMessage($"{user.combatantName} healed {target.combatantName} for {healAmount} HP!");

            HitEffect hitEffect = target.GetComponent<HitEffect>();
            hitEffect?.PlayHealEffect();
            DamageNumberSpawner.Instance?.SpawnHeal(healAmount, target.transform.position);
        }
    }
}