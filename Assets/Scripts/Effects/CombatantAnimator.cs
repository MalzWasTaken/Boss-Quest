using UnityEngine;
using System.Collections;

public class CombatantAnimator : MonoBehaviour
{
    private Animator animator;
    public float attackDistance = 1.5f;
    private Vector3 startPosition;

    // Animator parameter names - set these per enemy in Inspector
    public string attackTrigger = "Attack";
    public string hitTrigger = "Hit";
    public string deathTrigger = "Death";
    public string walkBool = "isWalking"; // isRunning for Goblin, isGalloping for Wolf

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        startPosition = transform.position;
    }

    public IEnumerator PlayAttackAnimation(Transform target)
    {
        startPosition = transform.position;
        Quaternion startRotation = transform.rotation;
        float lockedY = startPosition.y;

        // Rotate to face target
        Vector3 lookDir = target.position - transform.position;
        lookDir.y = 0f;
        if (lookDir != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(lookDir);
            Quaternion offset = Quaternion.Euler(0,-90f,0);
            transform.rotation = lookRotation * offset;
        }

        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0f;
        direction.Normalize();

        Vector3 attackPos = target.position - direction * attackDistance;
        attackPos.y = lockedY;

        SetWalking(true);

        float elapsed = 0f;
        float duration = 0.4f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            Vector3 pos = Vector3.Lerp(startPosition, attackPos, elapsed / duration);
            pos.y = lockedY;
            transform.position = pos;
            yield return null;
        }

        SetWalking(false);
        animator?.SetTrigger(attackTrigger);
        yield return new WaitForSeconds(0.6f);

        SetWalking(true);
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            Vector3 pos = Vector3.Lerp(attackPos, startPosition, elapsed / duration);
            pos.y = lockedY;
            transform.position = pos;
            yield return null;
        }

        SetWalking(false);
        transform.position = startPosition;
        transform.rotation = startRotation;
    }


    public void PlayHitAnimation()
    {
        animator?.SetTrigger(hitTrigger);
    }

    public void PlayDeathAnimation()
    {
        animator?.SetTrigger(deathTrigger);
    }

    public void SetWalking(bool value)
    {
        animator?.SetBool(walkBool, value);
    }
}

    // IEnumerator MoveToPosition(Vector3 destination)
    // {
    //     while (Vector3.Distance(transform.position, destination) > 0.05f)
    //     {
    //         transform.position = Vector3.MoveTowards(
    //             transform.position,
    //             destination,
    //             moveSpeed * Time.deltaTime
    //         );
    //         yield return null;
    //     }

    //     transform.position = destination;
    // }
