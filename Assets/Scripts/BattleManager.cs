using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance; //for other scripts to call battle manager

    [Header("Combatants")]
    public List<BaseHero> heroes;
    public List<BaseEnemy> enemies;

    [Header("UI")]
    public BattleMenuUI battleMenuUI;

    private List<PlannedAction> plannedActions = new List<PlannedAction>();
    private int currentHeroIndex = 0;

    void Awake()
    {
        Instance = this;
    }
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartPlanningPhase();
    }

    //planning phase
    void StartPlanningPhase()
    {
        plannedActions.Clear();
        currentHeroIndex = 0;
        Debug.Log("Planning phase started");
        PromptNextHero();
    }

    void PromptNextHero()
    {
        //skipping dead heroes
        while (currentHeroIndex < heroes.Count && !heroes[currentHeroIndex].IsAlive)
            currentHeroIndex++;
        
        if (currentHeroIndex >= heroes.Count)
        {
            //planning phase complete
            Debug.Log("All heroes planned, starting execution");
            StartExecutionPhase();
            return;
        }
         Debug.Log($"Prompting hero {heroes[currentHeroIndex].combatantName}");
        battleMenuUI.ShowForHero(heroes[currentHeroIndex]);
    }

    public void OnActionConfirmed(BattleAction action, Combatant target)
    {
        Debug.Log($"Action confirmed for hero index {currentHeroIndex}");
        plannedActions.Add(new PlannedAction
        {
            user = heroes[currentHeroIndex],
            action = action,
            targets = target != null ? new List<Combatant> {target} : new List<Combatant>()
        });

        currentHeroIndex++;
        PromptNextHero();
    }

    //execution phase

    void StartExecutionPhase()
    {
        Debug.Log($"Execution phase started with {plannedActions.Count} actions");
        foreach(var enemy in enemies)
        {
            if(enemy.IsAlive)
            {

            List<BaseHero> livingHeroes = GetLivingHeroes();
            BattleAction action = enemy.actions[Random.Range(0, enemy.actions.Count)];
            Combatant target = livingHeroes[Random.Range(0, livingHeroes.Count)];

            plannedActions.Add(new PlannedAction
            {
                user = enemy,
                action = action,
                targets = new List<Combatant> { target }
            });
            }
        }

        //sorting by agaility
        plannedActions = plannedActions
            .OrderByDescending(p => p.user.currAGI)
            .ToList();
        
        StartCoroutine(ExecuteAllActions());
    }

    IEnumerator ExecuteAllActions()
    {
        foreach (var plan in plannedActions)
        {
            if (!plan.user.IsAlive) continue;

            plan.action.Execute(plan.user, plan.targets);

            Debug.Log($"{plan.user.combatantName} used {plan.action.actionName}!");
            BattleLog.Instance.AddMessage($"{plan.user.combatantName} used {plan.action.actionName}!");


            yield return new WaitForSeconds(1.2f);

            if (CheckBattleOver()) yield break;
        }

        StartPlanningPhase();
    }

    bool CheckBattleOver()
    {
        if (heroes.All(h => !h.IsAlive))
        {
            Debug.Log("Game Over!");
            BattleLog.Instance.AddMessage("Game Over!");
            return true;
        }
        if (enemies.All(e => !e.IsAlive))
        {
            Debug.Log("Victory!");
            BattleLog.Instance.AddMessage("Victory!");
            return true;
        }
        return false;
    }


    public List<BaseEnemy> GetLivingEnemies()
    {
        return enemies.Where(e => e.IsAlive).ToList();
    }

    public List<BaseHero> GetLivingHeroes()
    {
        return heroes.Where (h => h.IsAlive).ToList();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
