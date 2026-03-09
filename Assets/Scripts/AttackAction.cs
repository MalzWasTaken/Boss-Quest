using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Battle/Actions/Attack")]
[System.Serializable]

public class AttackAction : BattleAction
{
    public float baseDamage = 10f;
    public float accuracy = 1.0f;
    public float damageVariance = 0.25f;
    public float criticalChance = 0.05f;
    public float criticalMultiplier = 1.5f;

    public override void Execute(Combatant user, List<Combatant> targets)
    {
        Debug.Log("AttackAction Execute called!");
        //hit check
        if (Random.value > accuracy)
        {
            Debug.Log($"{user.combatantName}'s attack missed!");
            return;
        }

        foreach (var target in targets)
        {
            // Calculate damage with variance
            float variance = Random.Range(1f - damageVariance, 1f + damageVariance);
            float damage = (baseDamage + user.currATK) * variance - target.currDEF;

            // Critical hit chance
            bool isCrit = Random.value < criticalChance;
            if (isCrit)
            {
                damage *= criticalMultiplier;
                Debug.Log($"{user.combatantName}'s critical hit!");
            }

            damage = Mathf.Max(1, damage);



            Debug.Log($"{user.combatantName} dealt {damage} damage to {target.combatantName}");
            if (BattleLog.Instance != null)
                BattleLog.Instance.AddMessage($"{user.combatantName} attacked {target.combatantName} for {damage:F0} damage!");
            target.TakeDamage(damage);
            DamageNumberSpawner.Instance?.Spawn(damage, target.transform.position, isCrit);
        }
    }
}