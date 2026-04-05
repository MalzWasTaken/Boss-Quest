//wolf slope rotation

using UnityEngine;
using UnityEngine.AI;

public class SlopeAlignment : MonoBehaviour
{
    public LayerMask groundLayer;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (agent == null || agent.velocity.magnitude < 0.1f) return;

        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out hit, 2f, groundLayer))
        {
            Quaternion slopeRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, slopeRotation, 10f * Time.deltaTime);
        }
    }
}