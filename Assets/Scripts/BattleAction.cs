using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Battle/Actions/New Action")]
public abstract class BattleAction : ScriptableObject 
{
    public string actionName;
    public string description;
    public float mpCost;

    public abstract void Execute(Combatant user, List<Combatant> targets);
}