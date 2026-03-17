using UnityEngine;
using System.Collections.Generic;

public class BattleSpawner : MonoBehaviour
{
    [Header("Spawn Points")]
    public List<Transform> enemySpawnPoints;

    [Header("Default Enemies")]
    public List<EnemyDefinition> defaultEnemies; // used when testing battle scene directly

    void Start()
    {
        List<EnemyDefinition> enemiesToSpawn = BattleData.enemiesToSpawn.Count > 0
            ? BattleData.enemiesToSpawn
            : defaultEnemies;

        SpawnEnemies(enemiesToSpawn);
        RestoreHeroStats();
    }

    void RestoreHeroStats()
    {
        if(BattleData.heroStats.Count == 0) return;

        foreach (var heroData in BattleData.heroStats)
        {
            BaseHero hero = BattleManager.Instance.heroes
                .Find(h => h.combatantName == heroData.heroName);
            
            if(hero != null)
            {
                hero.currHP = heroData.currHP;
                hero.currMP = heroData.currMP;
                hero.experience = heroData.experience;
                hero.gold = heroData.gold;
                hero.level = heroData.level;
                hero.expToNextLevel = heroData.expToNextLevel;
            }
        }
    }

    void SpawnEnemies(List<EnemyDefinition> definitions)
    {
        for (int i = 0; i < definitions.Count; i++)
        {
            if (i >= enemySpawnPoints.Count)
            {
                Debug.LogWarning("Not enough spawn points!");
                break;
            }

            EnemyDefinition def = definitions[i];

            if (def.enemyPrefab == null)
            {
                Debug.LogWarning($"No prefab assigned for {def.enemyName}!");
                continue;
            }

            // Spawn the enemy prefab
            GameObject enemyObj = Instantiate(def.enemyPrefab, 
                enemySpawnPoints[i].position, 
                enemySpawnPoints[i].rotation);

            // Apply stats from definition
            BaseEnemy enemy = enemyObj.GetComponent<BaseEnemy>();
            if (enemy != null)
            {
                enemy.combatantName = def.enemyName;
                enemy.maxHP = def.maxHP;
                enemy.currHP = def.maxHP;
                enemy.maxMP = def.maxMP;
                enemy.currMP = def.maxMP;
                enemy.currATK = def.attack;
                enemy.baseATK = def.attack;
                enemy.currDEF = def.defense;
                enemy.baseDEF = def.defense;
                enemy.currAGI = def.agility;
                enemy.baseAGI = def.agility;
                enemy.actions = def.actions;
                enemy.rarity = def.rarity;
                enemy.expReward = def.expReward;
                enemy.goldReward = def.goldReward;

                // Register with BattleManager
                BattleManager.Instance.enemies.Add(enemy);
            }
        }
    }
}