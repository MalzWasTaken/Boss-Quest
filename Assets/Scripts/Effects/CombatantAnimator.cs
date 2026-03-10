using UnityEngine;
using System.Collections;

public class CombatantAnimator : MonoBehaviour
{
    public float moveSpeed = 8f;
    public float attackDistance = 1.5f; // how far it lunges toward target

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    public IEnumerator PlayAttackAnimation(Transform target)
    {
        // Lunge toward target
        Vector3 direction = (target.position - transform.position).normalized;
        Vector3 lungeTarget = target.position - direction * attackDistance;

        yield return StartCoroutine(MoveToPosition(lungeTarget));
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(MoveToPosition(startPosition));
    }

    IEnumerator MoveToPosition(Vector3 destination)
    {
        while (Vector3.Distance(transform.position, destination) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                destination,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        transform.position = destination;
    }
}