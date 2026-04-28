using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BattleSpawner : MonoBehaviour
{
    [Header("Spawn Points")]
    public List<Transform> enemySpawnPoints;

    [Header("Default Enemies")]
    public List<EnemyDefinition> defaultEnemies; // used when testing battle scene directly
    public List<HeroDefinition> heroDefinitions;

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
        for (int i = 0; i < BattleManager.Instance.heroes.Count; i++)
        {
            BaseHero hero = BattleManager.Instance.heroes[i];
            HeroDefinition def = i < heroDefinitions.Count ? heroDefinitions[i] : null;

            // Apply base stats from definition first
            if (def != null)
            {
                hero.maxHP = def.maxHP;
                hero.maxMP = def.maxMP;
                hero.baseATK = def.baseATK;
                hero.currATK = def.baseATK;
                hero.baseDEF = def.baseDEF;
                hero.currDEF = def.baseDEF;
                hero.baseAGI = def.baseAGI;
                hero.currAGI = def.baseAGI;
                hero.abilities = def.learnableAbilities
                .Where(a => a.levelRequired <= hero.level)
                .Select(a => a.ability)
                .ToList();
            }

            // Then override with saved stats if they exist
            if (i < BattleData.heroStats.Count)
            {
                HeroData data = BattleData.heroStats[i];
                hero.combatantName = data.heroName;
                hero.currHP = data.currHP;
                hero.currMP = data.currMP;
                hero.maxHP = data.maxHP;
                hero.maxMP = data.maxMP;
                hero.experience = data.experience;
                hero.gold = data.gold;
                hero.level = data.level;
                hero.expToNextLevel = data.expToNextLevel;
            }
        }
    }

    void SpawnEnemies(List<EnemyDefinition> definitions)
    {
        List<int> spawnIndices = GetSpawnIndices(definitions.Count);
        
        for (int i = 0; i < definitions.Count; i++)
        {
            EnemyDefinition def = definitions[i];
            int spawnIndex = spawnIndices[i];
            if (def.enemyPrefab == null)
            {
                Debug.LogWarning($"No prefab assigned for {def.enemyName}!");
                continue;
            }

            // Spawn the enemy prefab
            GameObject enemyObj = Instantiate(def.enemyPrefab, 
                enemySpawnPoints[spawnIndex].position, 
                enemySpawnPoints[spawnIndex].rotation);

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
                if(enemy != null)
                {
                    enemy.combatantName = def.enemyName;
                    BattleManager.Instance.enemies.Add(enemy);

                    EnemyIndicator indicator = enemy.GetComponentInChildren<EnemyIndicator>();
                    indicator?.SetName(def.enemyName);
                    indicator?.ShowNameOnly();
                }
               
            }
        }
    }

    List<int> GetSpawnIndices(int enemyCount)
    {
        switch (enemyCount)
        {
            case 1:
                return new List<int> {1}; //middle spawn point
            case 2:
                return new List<int> {0,2}; //left and right spawns
            case 3:
                return new List<int> {0,1,2}; // all 3 points
            default:
                return new List<int> {1};
        }
    }
}