using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Battle/Actions/New Action")]
public abstract class BattleAction : ScriptableObject 
{
    public string actionName;
    public float mpCost;
    public bool targetsAllies = false;

    public bool isAttack;
    public bool isAbility;

    public string animatorTrigger = "Attack";

    public abstract void Execute(Combatant user, List<Combatant> targets);
}