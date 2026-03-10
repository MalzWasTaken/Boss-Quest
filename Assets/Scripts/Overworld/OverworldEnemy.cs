using UnityEngine;
using UnityEngine.AI;

public class OverworldEnemy : MonoBehaviour
{
    [Header("Enemy Definition")]
    public EnemyDefinition enemyDefinition;

    [Header("Wander Settings")]
    public float wanderRadius = 10f;
    public float wanderTimer = 3f;
    public float moveSpeed = 2f;

    [Header("Chase Settings")]
    public float detectionRange = 5f;
    public float chaseSpeed = 4f;
    public Transform player;

    private NavMeshAgent agent;
    private float timer;

    void Start()
    {
        //disable if enemy defeated
        if(BattleData.triggeredEnemyID == gameObject.name)
        {
            if(BattleData.enemiesToSpawn.Count == 0) //battle was won
                gameObject.SetActive(false);
            BattleData.triggeredEnemyID = "";
        }

        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;
        agent.speed = moveSpeed;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRange)
        {
            agent.speed = chaseSpeed;
            agent.SetDestination(player.position);
        }
        else
        {
            agent.speed = moveSpeed;
            timer += Time.deltaTime;
            if (timer >= wanderTimer)
            {
                agent.SetDestination(GetRandomPoint());
                timer = 0f;
            }
        }
    }

    Vector3 GetRandomPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;
        randomDirection.y = transform.position.y;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, 1);
        return hit.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (enemyDefinition == null)
            {
                Debug.LogWarning("No EnemyDefinition assigned!");
                return;
            }

            Time.timeScale = 0f;
            
            // Pass enemy data to battle scene
            BattleData.enemiesToSpawn.Clear();
            BattleData.enemiesToSpawn.Add(enemyDefinition);
            BattleData.returnScene = "Overworld";
            BattleData.triggeredEnemyID = gameObject.name;
            BattleData.playerReturnPosition = other.transform.position;

            AudioManager.Instance?.PlayBattleMusic();

           

            WarpEffect.Instance.TriggerWarp(() =>
            {
                Time.timeScale = 1f;
                UnityEngine.SceneManagement.SceneManager.LoadScene("BattleScene");
            });
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);
    }
}