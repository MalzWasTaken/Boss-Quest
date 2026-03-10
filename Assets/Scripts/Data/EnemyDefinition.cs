using UnityEngine;
using System.Collections.Generic;

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
    public BaseEnemy.Type enemyType;
    public BaseEnemy.Rarity rarity;
}