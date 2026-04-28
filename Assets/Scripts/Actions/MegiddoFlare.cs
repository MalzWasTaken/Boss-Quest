using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(menuName = "Battle/Actions/MegiddoFlare")]
public class MegiddoFlare : BattleAction
{
    public float baseDamage = 22f;
    public float damageVariance = 0.15f;

    public override void Execute(Combatant user, List<Combatant> targets)
    {
        BattleManager.Instance.StartCoroutine(MegiddoFlareRoutine(user));
    }

    IEnumerator MegiddoFlareRoutine(Combatant user)
    {
        BattleLogUI.Instance?.AddMessage($"{user.combatantName} channels Megiddo Flare!");
        yield return new WaitForSeconds(0.6f);

        // Hits ALL living heroes regardless of who was originally targeted
        List<BaseHero> allHeroes = BattleManager.Instance.GetLivingHeroes();

        foreach (var target in allHeroes)
        {
            float variance = Random.Range(1f - damageVariance, 1f + damageVariance);
            float damage = (baseDamage + user.currATK * 0.7f) * variance - target.currDEF * 0.5f;
            damage = Mathf.Max(1, damage);
            float roundedDamage = Mathf.RoundToInt(damage);

            BattleLogUI.Instance?.AddMessage($"{target.combatantName} took {roundedDamage} damage!");
            target.TakeDamage(roundedDamage);
            DamageNumberSpawner.Instance?.Spawn(roundedDamage, target.transform.position, false);

            yield return new WaitForSeconds(0.15f);
        }
    }
}