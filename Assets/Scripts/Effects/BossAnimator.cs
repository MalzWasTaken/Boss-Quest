using UnityEngine;

public class BossAnimator : CombatantAnimator
{
    [HideInInspector] public string nextAttackTrigger;

    [Header("VFX Mount Points")]
    public Transform mouthPoint; // assign the MouthPoint child in Inspector

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