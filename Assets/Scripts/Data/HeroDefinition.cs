using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Heroes/HeroDefinition")]
public class HeroDefinition : ScriptableObject
{
    public string heroName;
    public float maxHP;
    public float maxMP;
    public int baseATK;
    public int baseDEF;
    public int baseAGI;
    public int expToNextLevel = 100;
    public List<BattleAction> abilities;
}