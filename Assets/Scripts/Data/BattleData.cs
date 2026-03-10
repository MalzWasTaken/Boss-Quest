using System.Collections.Generic;
using UnityEngine;
public static class BattleData
{
    public static List<EnemyDefinition> enemiesToSpawn = new List<EnemyDefinition>();
    public static string returnScene = "Overworld";
    public static string triggeredEnemyID = "";
    public static Vector3 playerReturnPosition;
    public static List<HeroData> heroStats = new List<HeroData>();
}

public class HeroData
{
    public string heroName;
    public float currHP;
    public float currMP;
    public int experience;
    public int gold;
    public int level;
    public int expToNextLevel;
}