using System.Collections.Generic;

public struct PlannedAction
{
    public Combatant user;
    public BattleAction action;
    public List<Combatant> targets;
}