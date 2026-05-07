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

    private Animator animator;


    void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(BattleIntro());
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    IEnumerator BattleIntro()
    {
        yield return null;
        foreach (var enemy in enemies)
        {
            EnemyIndicator indicator = enemy.GetComponentInChildren<EnemyIndicator>();
            indicator?.SetName(enemy.combatantName);
        }

        if (enemies.Count > 0)
        {
            BattleCameraController.Instance?.StopOrbiting();
            BattleCameraController.Instance?.FocusIntro(enemies[0]);
        }

        // Build intro message
        BattleLogUI.Instance?.AddMessage(GetIntroMessage());

        yield return new WaitForSeconds(1.2f);
        StartPlanningPhase();
    }

    string GetIntroMessage()
    {
        List<BaseEnemy> living = GetLivingEnemies();

        if (living.Count == 1)
        {
            return $"A {living[0].combatantName} draws near!";
        }

        // Check if all enemies are the same type
        bool allSame = living.TrueForAll(e => e.combatantName == living[0].combatantName);

        if (allSame)
        {
            return living.Count == 2
                ? $"Two {living[0].combatantName}s draw near!"
                : $"Some {living[0].combatantName}s draw near!";
        }

        // Check if only two different types
        if (living.Count == 2)
            return $"A {living[0].combatantName} and a {living[1].combatantName} draw near!";

        return "Enemies draw near!";
    }

    //planning phase
    void StartPlanningPhase()
    {
        BattleCameraController.Instance?.StartOrbiting();
        foreach (var hero in heroes) hero.isDefending = false;
        foreach (var enemy in enemies) enemy.isDefending = false;

        foreach (var hero in heroes)
        {
            hero.isDefending = false;
            hero.GetComponent<HeroAnimator>()?.StopBlocking();
            hero.GetComponent<HeroAnimator>()?.StartIdling();
        }

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
        if (action != null)
        {
            if(action is DefendAction)
            {
                heroes[currentHeroIndex].isDefending = true;
                heroes[currentHeroIndex].GetComponent<HeroAnimator>()?.StopIdling();
                heroes[currentHeroIndex].GetComponent<HeroAnimator>()?.StartBlocking();
            }

            plannedActions.Add(new PlannedAction
            {
                user = heroes[currentHeroIndex],
                action = action,
                targets = target != null ? new List<Combatant> { target } : new List<Combatant>()
            });
        }

        currentHeroIndex++;
        PromptNextHero();
    }

    //execution phase



    void StartExecutionPhase()
    {
        battleMenuUI.ClearStatusText();
        // Debug.Log($"Execution phase started with {plannedActions.Count} actions");
        foreach (var hero  in heroes)
        {
            hero.GetComponent<HeroAnimator>()?.StopIdling();
            hero.GetComponent<HeroAnimator>()?.PlayDefaultIdle();
        }
        foreach (var enemy in enemies)
        {
            if (enemy.IsAlive)
            {

                List<BaseHero> livingHeroes = GetLivingHeroes();
                BattleAction action = enemy.PickAction();
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

            // Reset animator state
            CombatantAnimator combatantAnim = plan.user.GetComponent<CombatantAnimator>();
            HeroAnimator heroAnim = plan.user.GetComponent<HeroAnimator>();
            combatantAnim?.ResetTriggers();
            combatantAnim?.SetWalking(false);

            if (combatantAnim is BossAnimator bossAnim)
                bossAnim.nextAttackTrigger = plan.action.animatorTrigger;

            // Redirect to living target if original died
            plan.targets.RemoveAll(t => !t.IsAlive);
            if (plan.targets.Count == 0)
            {
                List<Combatant> livingTargets = plan.user is BaseHero
                    ? GetLivingEnemies().Cast<Combatant>().ToList()
                    : GetLivingHeroes().Cast<Combatant>().ToList();

                if (livingTargets.Count == 0) continue;
                plan.targets.Add(livingTargets[Random.Range(0, livingTargets.Count)]);
            }

            // Camera and animation
            yield return StartCoroutine(HandleActionAnimation(plan, combatantAnim, heroAnim));

            // Log and execute
            BattleLogUI.Instance?.AddMessage($"{plan.user.combatantName} used {plan.action.actionName}!");

            if (plan.user.currMP >= plan.action.mpCost)
            {
                plan.user.currMP -= plan.action.mpCost;
                plan.action.Execute(plan.user, plan.targets);
            }
            else
            {
                BattleLogUI.Instance?.AddMessage($"{plan.user.combatantName} doesn't have enough MP!");
            }

            yield return new WaitForSeconds(1.8f);
            if (CheckBattleOver()) yield break;
        }

        StartPlanningPhase();
    }

    IEnumerator HandleActionAnimation(PlannedAction plan, CombatantAnimator combatantAnim, HeroAnimator heroAnim)
    {
        Combatant target = plan.targets.Count > 0 ? plan.targets[0] : null;

        if (plan.action.targetsAllies)
        {
            // Heal / support ability
            heroAnim?.PlayCast();
            if (target != null)
                BattleCameraController.Instance?.FocusOnHeal(target);
            yield break;
        }

        if (plan.action.isAttack && combatantAnim != null && target != null)
        {
            // Physical attack - lunge animation
            BattleCameraController.Instance?.FocusOn(plan.user, target);
            yield return combatantAnim.PlayAttackAnimation(target.transform);
            yield break;
        }

        if (plan.action.isAbility && combatantAnim != null && plan.user is BaseEnemy)
        {
            Debug.Log($"[BOSS ABILITY BRANCH] action={plan.action.actionName}, trigger={plan.action.animatorTrigger}, isAbility={plan.action.isAbility}, isAttack={plan.action.isAttack}");
            BattleCameraController.Instance?.FocusOn(plan.user, target);
            yield return combatantAnim.PlayAttackInPlace();
            yield break;
        }

        if (plan.action.isAbility && heroAnim != null)
        {
            // Offensive ability - cast animation
            heroAnim.PlayCast();
            BattleCameraController.Instance?.FocusOn(plan.user, target);
            yield return new WaitForSeconds(0.8f);
            yield break;
        }

        if(plan.action is DefendAction)
        {
            heroAnim?.StopIdling();
            heroAnim?.StartBlocking();
            BattleCameraController.Instance?.StartOrbiting();
            yield break;
        }

        // Fallback - just focus camera
        if (target != null)
            BattleCameraController.Instance?.FocusOn(plan.user, target);
    }
    
    
    bool CheckBattleOver()
    {
        if (heroes.All(h => !h.IsAlive))
        {
            Debug.Log("Game Over!");
            StartCoroutine(ShowGameOverAfterDelay());
            return true;
        }
        if (enemies.All(e => !e.IsAlive))
        {
            Debug.Log("Victory!");
            if (!string.IsNullOrEmpty(BattleData.triggeredEnemyID)
                &&  !BattleData.defeatedEnemyIDs.Contains(BattleData.triggeredEnemyID))
            {
                BattleData.defeatedEnemyIDs.Add(BattleData.triggeredEnemyID);
                Debug.Log($"[BattleManager] Marked '{BattleData.triggeredEnemyID}' as defeated");
            }


            if (BattleData.isFinalBoss)
            {
                RewardManager.Instance?.GiveRewardsGameClear(heroes, enemies);
            }
            else
            {
                RewardManager.Instance?.GiveRewards(heroes, enemies);
            }
            return true;
        }
        return false;
    }

    IEnumerator ShowGameOverAfterDelay()
    {
        yield return new WaitForSeconds(1.5f);
        BattleResultUI.Instance?.ShowGameOver();
    }

    IEnumerator ReturnToOverworld(float delay)
    {
        yield return new WaitForSeconds(delay);
        AudioManager.Instance?.PlayOverworldMusic();
        BattleData.heroStats.Clear();
        foreach (var hero in heroes)
        {
            BattleData.heroStats.Add(new HeroData
            {
                heroName = hero.combatantName,
                currHP = hero.currHP,
                currMP = hero.currMP,
                maxHP = hero.maxHP,
                maxMP = hero.maxMP,
                experience = hero.experience,
                gold = hero.gold,
                level = hero.level,
                expToNextLevel = hero.expToNextLevel
            });
        }
        BattleData.enemiesToSpawn.Clear();
        UnityEngine.SceneManagement.SceneManager.LoadScene(BattleData.returnScene);
    }


    public List<BaseEnemy> GetLivingEnemies()
    {
        return enemies.Where(e => e.IsAlive).ToList();
    }

    public List<BaseHero> GetLivingHeroes()
    {
        return heroes.Where(h => h.IsAlive).ToList();
    }
    // Update is called once per frame
    void Update()
    {

    }
}
