using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Battle/Actions/DoubleSlash")]
public class DoubleSlash : BattleAction
{
    public float baseDamage = 20f;
    public float accuracy = 1.0f;
    public float damageVariance = 0.25f;
    public float criticalChance = 0.05f;
    public float criticalMultiplier = 1.5f;

    public override void Execute(Combatant user, List<Combatant> targets)
    {
        BattleManager.Instance.StartCoroutine(DoubleSlashRoutine(user, targets));
    }

    IEnumerator DoubleSlashRoutine(Combatant user, List<Combatant> targets)
    {
        // First hit
        Strike(user, targets, "First");
        
        yield return new WaitForSeconds(0.4f);
        
        // Second hit
        Strike(user, targets, "Second");
    }

    void Strike(Combatant user, List<Combatant> targets, string hitLabel)
    {
        if (Random.value > accuracy)
        {
            Debug.Log($"{user.combatantName}'s attack missed!");
            BattleLog.Instance?.AddMessage($"{user.combatantName}'s attack missed!");
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
                BattleLog.Instance?.AddMessage("Critical hit!");
            }

            damage = Mathf.Max(1, damage);

            
            Debug.Log($"{user.combatantName} dealt {damage} damage to {target.combatantName}");
            BattleLog.Instance?.AddMessage($"{hitLabel} slash hits {target.combatantName} for {damage:F0} damage!");
            target.TakeDamage(damage);
            DamageNumberSpawner.Instance?.Spawn(damage, target.transform.position, isCrit);
        }

    }
}