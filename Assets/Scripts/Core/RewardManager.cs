using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

        //total reward calculation
        foreach (var enemy in defeatedEnemies)
        {
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
                StartCoroutine(LevelUp(hero));
        }
    }


    IEnumerator LevelUp(BaseHero hero)
    {
        hero.experience -= hero.expToNextLevel;
        hero.level++;
        hero.expToNextLevel = Mathf.RoundToInt(hero.expToNextLevel * 1.5f);

        //level up stat increases
        hero.maxHP += 10;
        hero.currHP += 10;
        hero.maxMP += 3;
        hero.baseATK += 2;
        hero.currATK += 2;
        hero.baseDEF += 1;
        hero.currDEF += 1;
        hero.baseAGI += 1;
        hero.currAGI += 1;

        yield return new WaitForSeconds(0.5f);
        Debug.Log($"{hero.combatantName} levelled up to level {hero.level}!");
    }
}