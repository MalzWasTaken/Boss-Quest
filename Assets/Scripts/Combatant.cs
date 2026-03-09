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
    public bool isDefending = false;

    // Called when it's this combatant's turn
    public abstract void TakeTurn(TurnManager turnManager);

    public virtual void TakeDamage(float damage)
    {
        if(isDefending) damage *= 0.5f;

        currHP -= damage;
        if (currHP < 0) currHP = 0;

        //hit effect
        HitEffect hitEffect = GetComponent<HitEffect>();
        if(!IsAlive)
            hitEffect?.PlayDeathEffect(); //crush on death
        else
            hitEffect?.PlayHitEffect(); //hit effect if still alive
    }

    public virtual void Heal(float amount)
{
    currHP += amount;
    if (currHP > maxHP) currHP = maxHP;
}
}