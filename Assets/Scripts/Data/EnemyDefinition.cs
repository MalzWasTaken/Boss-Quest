using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(menuName = "Enemies/EnemyDefinition")]
public class EnemyDefinition : ScriptableObject
{
    [Header("Basic Info")]
    public string enemyName;
    public GameObject enemyPrefab; // visual prefab to spawn in battle

    [Header("Stats")]
    public float maxHP;
    public float maxMP;
    public float attack;
    public float defense;
    public float agility;

    [Header("Rewards")]
    public int expReward;
    public int goldReward;

    [Header("Actions")]
    public List<BattleAction> actions;

    [Header("Enemy Info")]
    public BaseEnemy.Rarity rarity;

    [System.Serializable]
    public class WeightedAction
    {
        public BattleAction action;
        public int weight = 1;
    }

    public List<WeightedAction> weightedActions;

    public BattleAction PickAction()
    {
        if (weightedActions == null || weightedActions.Count == 0)
            return actions[Random.Range(0, actions.Count)];

        int total = weightedActions.Sum(w => w.weight);
        int roll = Random.Range(0, total);
        int cumulative = 0;
        foreach (var w in weightedActions)
        {
            cumulative += w.weight;
            if (roll < cumulative) return w.action;
        }
        return weightedActions[0].action;
    }
}