using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Battle/Actions/MultiHeal")]
public class MultiHealAction : BattleAction
{
    public float baseHeal = 30f;
    public float healVariance = 0.25f;
    public override void Execute (Combatant user, List<Combatant> targets)
    {

        List<BaseHero> allHeroes = BattleManager.Instance.GetLivingHeroes();

        foreach(var hero in allHeroes)
        {
            float variance = Random.Range(1f - healVariance, 1f + healVariance);
            float healAmount = Mathf.RoundToInt(baseHeal * variance);

            hero.Heal(healAmount);
            BattleLogUI.Instance?.AddMessage($"{user.combatantName} healed {hero.combatantName} for {healAmount} HP!");
            DamageNumberSpawner.Instance?.SpawnHeal(healAmount, hero.transform.position);

            HitEffect hitEffect = hero.GetComponent<HitEffect>();
            hitEffect?.PlayHealEffect();
            
        }
    }
}