using UnityEngine;
using System.Collections;

public class HeroAnimator : MonoBehaviour
{
    private Animator animator;

    public string idleBool = "isIdling";
    public string idleVariantTrigger1 = "IdleVariant1";
    public string idleVariantTrigger2 = "IdleVariant2";
    public string attackTrigger = "Attack";
    public string blockBool = "isBlocking";
    public string castTrigger = "Cast";
    public string hitTrigger = "Hit";
    public string deathTrigger = "Death";
    public float idleVariantDelay = 4f;
    public float rotationOffset = 0f;

    private bool isIdling = false;
    private Coroutine idleRoutine;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void PlayAttack()
    {
        animator?.SetTrigger(attackTrigger);
    }

    public void StartBlocking()
    {
        animator?.SetBool(blockBool, true);
    }

    public void StopBlocking()
    {
        animator?.SetBool(blockBool, false);
    }

    public void PlayCast()
    {
        animator?.SetTrigger(castTrigger);
    }

    public void PlayHit()
    {
        animator?.SetTrigger(hitTrigger);
    }

    public void PlayDeath()
    {
        animator?.SetTrigger(deathTrigger);
    }

    public void ResetAnimator()
    {
        animator?.ResetTrigger(attackTrigger);
        animator?.ResetTrigger(castTrigger);
        animator?.ResetTrigger(hitTrigger);
    }

    public void StartIdling()
    {
        Debug.Log($"{gameObject.name} StartIdling called");
        isIdling = true;
        animator?.SetBool(idleBool, true);
        idleRoutine = StartCoroutine(IdleVariantRoutine());
    }

    public void StopIdling()
    {
        isIdling = false;
        if (idleRoutine != null)
        {
            StopCoroutine(idleRoutine);
            idleRoutine = null;
        }
    }

    public void PlayDefaultIdle()
    {
        animator?.SetBool(idleBool,true);
    }

    public Quaternion GetRotationOffset()
    {
        return Quaternion.Euler(0,rotationOffset,0);
    }

    IEnumerator IdleVariantRoutine()
    {
        Debug.Log("IdleVariantRoutineStarted");
        while (isIdling)
        {
            yield return new WaitForSeconds(idleVariantDelay);
            if (!isIdling) break;

            // Pick random variant
            int variant = Random.Range(0, 2);
            Debug.Log($"Playing idle variant {variant}");
            if (variant == 0)
                animator?.SetTrigger(idleVariantTrigger1);
            else
                animator?.SetTrigger(idleVariantTrigger2);

            // Wait for variant to finish before looping back to idle
            yield return new WaitForSeconds(7.6f);
        }
    }
}