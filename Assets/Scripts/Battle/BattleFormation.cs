using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Battle/BattleFormation")]
public class BattleFormation : ScriptableObject
{
    public string formationName;
    public List<EnemyDefinition> enemies;
    public bool isFinalBoss = false;
}