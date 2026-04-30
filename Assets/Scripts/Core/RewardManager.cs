using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class RewardManager : MonoBehaviour {
    public static RewardManager Instance;

    void Awake()
    {
        Instance = this;
    }

    public void GiveRewards(List<BaseHero> heroes, List<BaseEnemy> defeatedEnemies)
    {
        int totalExp = 0;
        int totalGold = 0;
        List <string> levelUps = new List<string>();

        //total reward calculation
        Debug.Log($"GiveRewards called with {defeatedEnemies.Count} enemies");
        foreach (var enemy in defeatedEnemies)
        {
            Debug.Log($"Enemy: {enemy.combatantName}, EXP: {enemy.expReward}");
            totalExp += enemy.expReward;
            totalGold += enemy.goldReward;
        }
        int expPerHero = totalExp / Mathf.Max(1,heroes.Count);


        BaseHero partyLeader = heroes.Find(h => h.IsAlive);
        if(partyLeader != null)
        {
            partyLeader.gold += totalGold;
            Debug.Log($"Party gained {totalGold} gold!");
        }

        foreach(var hero in heroes)
        {
            if(!hero.IsAlive) continue;
            hero.experience += expPerHero;
            Debug.Log($"{hero.combatantName} gained {expPerHero} EXP!");

            //check for level up
            if(hero.experience >= hero.expToNextLevel)
            {
                StartCoroutine(LevelUp(hero,levelUps));
            }
                
        }
        StartCoroutine(ShowVictoryAfterDelay(expPerHero,totalGold,levelUps));
    }

    public void GiveRewardsGameClear(List<BaseHero> heroes, List<BaseEnemy> defeatedEnemies)
    {
        int totalExp = 0;
        int totalGold = 0;

        foreach (var enemy in defeatedEnemies)
        {
            totalExp += enemy.expReward;
            totalGold += enemy.goldReward;
        }

        int expPerHero = totalExp / Mathf.Max(1, heroes.Count);
        BaseHero partyLeader = heroes.Find(h => h.IsAlive);
        if (partyLeader != null) partyLeader.gold += totalGold;

        foreach (var hero in heroes)
        {
            if (!hero.IsAlive) continue;
            hero.experience += expPerHero;
        }

        StartCoroutine(ShowGameClearAfterDelay());
    }

    IEnumerator ShowGameClearAfterDelay()
    {
        yield return new WaitForSeconds(1.5f);
        GameClearUI.Instance?.ShowGameClear();
    }

    IEnumerator ShowVictoryAfterDelay(int exp, int gold, List<string> levelUps)
    {
        yield return new WaitForSeconds(0.2f);
        BattleResultUI.Instance?.ShowVictory(exp,gold,levelUps);
    }

    IEnumerator LevelUp(BaseHero hero, List<string> levelUps)
    {
        hero.experience -= hero.expToNextLevel;
        hero.level++;
        hero.expToNextLevel = Mathf.RoundToInt(hero.expToNextLevel * 1.5f);

        hero.maxHP += 10;
        hero.currHP += 10;
        hero.maxMP += 3;
        hero.baseATK += 2;
        hero.currATK += 2;
        hero.baseDEF += 1;
        hero.currDEF += 1;
        hero.baseAGI += 5;
        hero.currAGI += 5;

        // Refresh abilities based on new level
        HeroDefinition def = BattleSpawner.Instance?.GetDefinitionForHero(hero);
        if (def != null)
        {
            var previousAbilities = new HashSet<BattleAction>(hero.abilities ?? new List<BattleAction>());

            hero.abilities = def.learnableAbilities
                .Where(a => a.levelRequired <= hero.level)
                .Select(a => a.ability)
                .ToList();

            foreach (var ability in hero.abilities)
            {
                if (!previousAbilities.Contains(ability))
                {
                    levelUps.Add($"{hero.combatantName} learned {ability.actionName}!");
                    Debug.Log($"{hero.combatantName} learned {ability.actionName}!");
                }
            }
        }

        levelUps.Add($"{hero.combatantName} reached level {hero.level}!");
        Debug.Log($"{hero.combatantName} levelled up to level {hero.level}!");
        yield return null;
    }
}