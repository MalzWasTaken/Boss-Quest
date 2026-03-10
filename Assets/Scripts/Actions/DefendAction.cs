using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(menuName = "Battle/Actions/Defend")]
public class DefendAction : BattleAction
{
    public float damageReduction = 0.5f; // 50% damage reduction

    public override void Execute(Combatant user, List<Combatant> targets)
    {
        user.isDefending = true;
    }
}