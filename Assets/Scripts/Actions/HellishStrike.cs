using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Battle/Actions/HellishStrike")]
public class HellishStrike : BattleAction
{
    public float baseDamage = 35f;
    public float accuracy = 0.9f;
    public float damageVariance = 0.2f;
    public float criticalChance = 0.1f;
    public float criticalMultiplier = 1.75f;

    public override void Execute(Combatant user, List<Combatant> targets)
    {
        BattleLogUI.Instance?.AddMessage($"{user.combatantName} unleashes a Hellish Strike!");
        
        if (Random.value > accuracy)
        {
            BattleLogUI.Instance?.AddMessage($"{user.combatantName}'s attack missed!");
            return;
        }

        foreach (var target in targets)
        {
            if (!target.IsAlive) continue;

            float variance = Random.Range(1f - damageVariance, 1f + damageVariance);
            float damage = (baseDamage + user.currATK) * variance - target.currDEF;

            bool isCrit = Random.value < criticalChance;
            if (isCrit)
            {
                damage *= criticalMultiplier;
                BattleLogUI.Instance?.AddMessage($"Critical hit!");
            }

            damage = Mathf.Max(1, damage);
            float roundedDamage = Mathf.RoundToInt(damage);

            BattleLogUI.Instance?.AddMessage($"{user.combatantName} dealt {roundedDamage} damage to {target.combatantName}!");

            target.TakeDamage(roundedDamage);
            DamageNumberSpawner.Instance?.Spawn(roundedDamage, target.transform.position, isCrit);
        }
    }
}