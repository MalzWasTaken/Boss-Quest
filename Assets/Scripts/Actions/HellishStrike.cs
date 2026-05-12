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

    [Header("VFX")]
    public GameObject earthShatterPrefab;
    public float vfxDelay = 0.4f;       // wait for the animation impact frame
    public float vfxLifetime = 3f;       // how long before the VFX object cleans up
    public bool spawnAtTarget = false;   // false = at boss's feet, true = at target's feet

    public override void Execute(Combatant user, List<Combatant> targets)
    {
        BattleManager.Instance.StartCoroutine(HellishStrikeRoutine(user, targets));
    }

    IEnumerator HellishStrikeRoutine(Combatant user, List<Combatant> targets)
    {
        BattleLogUI.Instance?.AddMessage($"{user.combatantName} unleashes a Hellish Strike!");
        Debug.Log($"[HellishStrike] Routine started. prefab={earthShatterPrefab}, delay={vfxDelay}");

        // Wait for the impact frame of the swing animation
        yield return new WaitForSeconds(vfxDelay);

        // Spawn the earth shatter VFX
        if (earthShatterPrefab != null)
        {
            Vector3 spawnPos;
            if (spawnAtTarget && targets.Count > 0)
                spawnPos = targets[0].transform.position;
            else
                spawnPos = user.transform.position;

            

            Debug.Log($"[HellishStrike] Spawning VFX at {spawnPos}");
            GameObject vfx = Object.Instantiate(earthShatterPrefab, spawnPos, Quaternion.identity);
            Debug.Log($"[HellishStrike] VFX instantiated: {vfx.name}, active={vfx.activeInHierarchy}");

            Object.Destroy(vfx, vfxLifetime);
        }
        else
        {
            Debug.LogWarning("[HellishStrike] earthShatterPrefab is NULL");

        }

        // Accuracy check
        if (Random.value > accuracy)
        {
            BattleLogUI.Instance?.AddMessage($"{user.combatantName}'s attack missed!");
            yield break;
        }

        // Apply damage
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