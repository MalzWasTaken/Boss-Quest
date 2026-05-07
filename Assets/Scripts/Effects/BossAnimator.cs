using UnityEngine;

public class BossAnimator : CombatantAnimator
{
    [HideInInspector] public string nextAttackTrigger;

    protected override string GetAttackTrigger()
    {
        return string.IsNullOrEmpty(nextAttackTrigger) ? attackTrigger : nextAttackTrigger;
    }

    public string[] allBossTriggers = { "Swipe", "JumpAttack", "Punch", "Roar" };

    public new void ResetTriggers()
    {
        base.ResetTriggers();
        var anim = GetComponentInChildren<Animator>();
        foreach (var t in allBossTriggers)
            anim?.ResetTrigger(t);
    }
}