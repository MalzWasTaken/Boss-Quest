using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class LearnableAbility
{
    public BattleAction ability;
    public int levelRequired;
}


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
    public List<LearnableAbility> learnableAbilities;
}