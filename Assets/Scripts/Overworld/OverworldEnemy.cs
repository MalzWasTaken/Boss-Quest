using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;

public class OverworldEnemy : MonoBehaviour
{
    [Header("Formations")]
    public List<BattleFormation> possibleFormations; // assign 1 or more formations

    [Header("Detection")]
    public float detectionRange = 8f;
    public float wanderRadius = 5f;
    public float chaseSpeed = 4f;
    public float wanderSpeed = 2f;

    public Transform player;
    private NavMeshAgent agent;
    private Vector3 wanderTarget;
    private bool battleTriggered = false;

    private Animator animator;
    public string roamBool = "isRoaming";
    public string chaseBool = "isChasing";

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        SetNewWanderTarget();

        if (BattleData.defeatedEnemyIDs.Contains(gameObject.name))
            gameObject.SetActive(false);
    }

    void Update()
    {
        if (battleTriggered || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance < detectionRange)
        {      
            SetChasing(true);
            SetWalking(false);
            agent.speed = chaseSpeed;
            agent.SetDestination(player.position);
            agent.updateRotation = true;
        }
        else
        {
            SetChasing(false);
            SetWalking(true);
            agent.speed = wanderSpeed;
            if (agent.remainingDistance < 1f)
                SetNewWanderTarget();
        }

        if (distance <= 1.4f)
            TriggerBattle();
    }

    void SetNewWanderTarget()
    {
        Vector3 randomDir = Random.insideUnitSphere * wanderRadius + transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDir, out hit, wanderRadius, NavMesh.AllAreas))
            wanderTarget = hit.position;
        agent.SetDestination(wanderTarget);
    }

    void TriggerBattle()
    {
        if (possibleFormations == null || possibleFormations.Count == 0)
        {
            Debug.LogWarning("No formations assigned to " + gameObject.name);
            return;
        }

        battleTriggered = true;
        Time.timeScale = 0f;

        // Pick a random formation
        BattleFormation formation = possibleFormations[Random.Range(0, possibleFormations.Count)];

        BattleData.enemiesToSpawn.Clear();
        foreach (var enemy in formation.enemies)
            BattleData.enemiesToSpawn.Add(enemy);

        BattleData.returnScene = "Overworld";
        BattleData.triggeredEnemyID = gameObject.name;
        BattleData.playerReturnPosition = player.position;

        AudioManager.Instance?.PlayBattleMusic();
        WarpEffect.Instance.TriggerWarp(() =>
        {
            Time.timeScale = 1f;
            UnityEngine.SceneManagement.SceneManager.LoadScene("BattleScene");
        });
    }

    public void SetWalking(bool value) => animator?.SetBool(roamBool, value);
    public void SetChasing(bool value) => animator?.SetBool(chaseBool, value);
}