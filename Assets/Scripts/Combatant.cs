using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public abstract class Combatant : MonoBehaviour
{
    public string combatantName;
    public float currHP, maxHP, currMP, maxMP;
    public float currATK, baseATK, currDEF, baseDEF, currAGI, baseAGI;
    public List<BattleAction> actions;
    public bool IsAlive => currHP > 0;

    // Called when it's this combatant's turn
    public abstract void TakeTurn(TurnManager turnManager);

    public virtual void TakeDamage(float damage)
    {
        currHP -= damage;
        if (currHP < 0) currHP = 0;
    }

    public virtual void Heal(float amount)
{
    currHP += amount;
    if (currHP > maxHP) currHP = maxHP;
}
}