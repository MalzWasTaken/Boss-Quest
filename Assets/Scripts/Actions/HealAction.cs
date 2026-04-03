using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Battle/Actions/Heal")]
public class HealAction : BattleAction
{
    public float baseHeal = 30f;
    public float healVariance = 0.25f;
    public override void Execute (Combatant user, List<Combatant> targets)
    {
        foreach(var target in targets)
        {
            float variance = Random.Range(1f - healVariance, 1f + healVariance);
            float healAmount = Mathf.RoundToInt(baseHeal * variance);

            target.Heal(healAmount);
            BattleLogUI.Instance?.AddMessage($"{user.combatantName} healed {target.combatantName} for {healAmount} HP!");
            DamageNumberSpawner.Instance?.SpawnHeal(healAmount, target.transform.position);

            HitEffect hitEffect = target.GetComponent<HitEffect>();
            hitEffect?.PlayHealEffect();
            
        }
    }
}