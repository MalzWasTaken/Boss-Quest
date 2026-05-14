using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(menuName = "Battle/Actions/MegiddoFlare")]
public class MegiddoFlare : BattleAction
{
    public float baseDamage = 22f;
    public float damageVariance = 0.15f;

    [Header("VFX")]
    public GameObject breathPrefab;
    public float breathSpawnDelay = 0.4f;  // wait for the boss to start leaning in
    public float breathDuration = 2.5f;     // how long the breath plays before damage applies
    public float damageDelay = 1.2f;        // when in the breath the damage actually hits

    public override void Execute(Combatant user, List<Combatant> targets)
    {
        BattleManager.Instance.StartCoroutine(MegiddoFlareRoutine(user));
    }

    IEnumerator MegiddoFlareRoutine(Combatant user)
    {
        BattleLogUI.Instance?.AddMessage($"{user.combatantName} channels Megiddo Flare!");

        // Wait for the wind-up animation
        yield return new WaitForSeconds(breathSpawnDelay);

        // Spawn the breath attached to the boss's mouth
        GameObject vfx = null;
        BossAnimator bossAnim = user.GetComponent<BossAnimator>();
        if (breathPrefab != null && bossAnim != null && bossAnim.mouthPoint != null)
        {
            vfx = Object.Instantiate(breathPrefab, bossAnim.mouthPoint.position, bossAnim.mouthPoint.rotation);
            vfx.transform.SetParent(bossAnim.mouthPoint, worldPositionStays: true);
            Object.Destroy(vfx, breathDuration);
        }

        // Damage hits mid-breath
        yield return new WaitForSeconds(damageDelay);

        List<BaseHero> allHeroes = BattleManager.Instance.GetLivingHeroes();
        foreach (var target in allHeroes)
        {
            float variance = Random.Range(1f - damageVariance, 1f + damageVariance);
            float damage = (baseDamage + user.currATK * 0.7f) * variance - target.currDEF * 0.5f;
           Debug.Log($"[Damage] {target.combatantName} isDefending={target.isDefending}, damage before defend={damage}");
            if (target.isDefending) damage *= 0.5f;
            Debug.Log($"[Damage] damage after defend check={damage}");
            damage = Mathf.Max(1, damage);
            float roundedDamage = Mathf.RoundToInt(damage);

            BattleLogUI.Instance?.AddMessage($"{target.combatantName} took {roundedDamage} damage!");
            target.TakeDamage(roundedDamage);
            DamageNumberSpawner.Instance?.Spawn(roundedDamage, target.transform.position, false);

            yield return new WaitForSeconds(0.15f);
        }
    }
}