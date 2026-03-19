using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.EventSystems;

public enum MenuState
{
    ActionSelect,
    AbilitySelect,
    ItemSelect,
    EnemySelect,
    AllySelect,
    Inactive
}

public class BattleMenuUI : MonoBehaviour
{
    public GameObject actionPanel; //Attack / abilitiy / item / defend
    public GameObject abilityPanel; //List of ablities
    public GameObject itemPanel; //List of items
    public GameObject enemyPanel; //Enemy select

    public GameObject allyPanel; //ally select

    public GameObject buttonPrefab;

    public TMP_Text statusText;

    public BattleAction attackAction;
    public BattleAction defendAction;

    public Button abilitiesButton;


    private MenuState state;
    private BattleAction pendingAction; //holds chosen action
    
    private BaseHero currentHero;
    public void ShowForHero(BaseHero hero)
    {
        currentHero = hero;
        statusText.text = $"{hero.combatantName} is deciding...";

        //grey out abilities button if none
        abilitiesButton.interactable = hero.abilities != null && hero.abilities.Count > 0;
        SetState(MenuState.ActionSelect);
    }

    void SetState(MenuState newState)
    {
        state = newState;

        //hide all panels first
        actionPanel.SetActive(false);
        abilityPanel.SetActive(false);
        itemPanel.SetActive(false);
        enemyPanel.SetActive(false);
        allyPanel.SetActive(false);

        switch (state)
        {
            case MenuState.ActionSelect:
                actionPanel.SetActive(true);
                break;
            case MenuState.AbilitySelect:
                abilityPanel.SetActive(true);
                break;
            case MenuState.ItemSelect:
                itemPanel.SetActive(true);
                break;
            case MenuState.AllySelect:
                allyPanel.SetActive(true);
                BuildAllyList();
                break;
            case MenuState.EnemySelect:
                enemyPanel.SetActive(true);
                BuildEnemyList();
                break;
        }
    }

    //action menu buttons
    public void OnAttackPressed()
    {
        Debug.Log("Attack button pressed!");
        pendingAction = attackAction;
        SetState(MenuState.EnemySelect);
    }

    public void OnAbilityPressed()
    {
        SetState(MenuState.AbilitySelect);
        BuildAbilityList(currentHero);
    }

    public void OnItemPressed()
    {
        SetState(MenuState.ItemSelect);
        BuildItemList();
    }

    public void OnDefendPressed()
    {
        SetState(MenuState.Inactive);
        BattleManager.Instance.OnActionConfirmed(defendAction, null);
    }

    public void OnRunPressed()
{
    if (Random.value > 0.5f)
    {
        Debug.Log("Successfully fled the battle!");
        SetState(MenuState.Inactive);
        StartCoroutine(RunSuccess());
        // load overworld scene etc
    }
    else
    {
        Debug.Log("Couldn't escape!");
        SetState(MenuState.Inactive);
        BattleManager.Instance.OnActionConfirmed(null, null);
    }
}

    IEnumerator RunSuccess()
    {
        yield return new WaitForSeconds(1.5f);
        AudioManager.Instance?.PlayOverworldMusic();
        BattleData.enemiesToSpawn.Clear();

        BattleData.heroStats.Clear();
        foreach (var hero in BattleManager.Instance.heroes)
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
        UnityEngine.SceneManagement.SceneManager.LoadScene(BattleData.returnScene);
    }

    //abilty and item submenus
    public void OnAbilitySelected(BattleAction ability)
    {
        pendingAction = ability;
        if(ability.targetsAllies)
        SetState(MenuState.AllySelect);
        else
        SetState(MenuState.EnemySelect);
    }

    public void OnItemSelected(BattleAction item)
    {
        pendingAction = item;
        SetState(MenuState.EnemySelect);
    }


    //enemy select

    public void OnEnemySelected(BaseEnemy enemy)
    {
        Debug.Log($"Enemy selected: {enemy.combatantName}");
        statusText.text = "";
        SetState(MenuState.Inactive);
        BattleManager.Instance?.OnActionConfirmed(pendingAction, enemy);
        
    }

    public void OnAllySelected(BaseHero hero)
    {
        Debug.Log($"Ally selected: {hero.combatantName}");
        statusText.text = "";
        SetState(MenuState.Inactive);
        BattleManager.Instance?.OnActionConfirmed(pendingAction, hero);
    }

    //back button
    public void OnBackPressed()
    {
        switch (state)
        {
            case MenuState.AbilitySelect:
            case MenuState.ItemSelect:
            HideAllIndicators();
                SetState(MenuState.ActionSelect); //back one level
                break;
            case MenuState.EnemySelect:
            //go back to where we came from
            HideAllIndicators();
               SetState(MenuState.ActionSelect);
               break;
            case MenuState.AllySelect:
            HideAllIndicators();
                SetState(MenuState.ActionSelect);
                break;
        }
    }

    void BuildAbilityList(BaseHero hero)
{
    List<GameObject> toDestroy = new List<GameObject>();
    foreach (Transform child in abilityPanel.transform)
        if (child.name != "OUTLINE" && child.name != "BACKBUTTON")
            toDestroy.Add(child.gameObject);
    foreach (GameObject obj in toDestroy)
        DestroyImmediate(obj);

    if (hero.abilities == null || hero.abilities.Count == 0)
    {
        SetState(MenuState.ActionSelect);
        return;
    }

    foreach (BattleAction action in hero.abilities)
    {
        GameObject btn = Instantiate(buttonPrefab, abilityPanel.transform);
        btn.GetComponentInChildren<TMP_Text>().text = action.actionName;
        btn.GetComponent<Button>().interactable = hero.currMP >= action.mpCost;

        BattleAction actionRef = action;
        btn.GetComponent<Button>().onClick.AddListener(() => OnAbilitySelected(actionRef));
    }
}


    public void BuildEnemyList()
    {
        List<GameObject> toDestroy = new List<GameObject>();
        foreach (Transform child in enemyPanel.transform)
        if (child.name != "OUTLINE" && child.name != "BACKBUTTON")
        toDestroy.Add(child.gameObject);

        foreach (GameObject obj in toDestroy)
            DestroyImmediate(obj);

        foreach (BaseEnemy enemy in BattleManager.Instance.GetLivingEnemies())
        {
            GameObject btn = Instantiate(buttonPrefab, enemyPanel.transform);
            btn.GetComponentInChildren<TMP_Text>().text = enemy.combatantName;

            BaseEnemy enemyRef = enemy;

            EnemyIndicator indicator = enemy.GetComponent<EnemyIndicator>();
            Debug.Log($"Enemy: {enemy.combatantName}, Indicator: {indicator}");

            // Hover events
            EventTrigger trigger = btn.AddComponent<EventTrigger>();

            EventTrigger.Entry enterEntry = new EventTrigger.Entry();
            enterEntry.eventID = EventTriggerType.PointerEnter;
            enterEntry.callback.AddListener((_) =>
            {
                Debug.Log("Hovering over enemy button");
                HideAllIndicators();
                if (indicator != null) indicator.ShowIndicator();
                else Debug.Log("Indicator is null");
            });
            trigger.triggers.Add(enterEntry);

            EventTrigger.Entry exitEntry = new EventTrigger.Entry();
            exitEntry.eventID = EventTriggerType.PointerExit;
            exitEntry.callback.AddListener((_) =>
            {
                if (indicator != null)
                {
                    indicator.HideIndicator();
                    indicator.ShowNameOnly();
                }
            });
            trigger.triggers.Add(exitEntry);

            btn.GetComponent<Button>().onClick.AddListener(() => { HideAllIndicators();OnEnemySelected(enemyRef);});
        }
    }

    void HideAllIndicators()
    {
        foreach (var enemy in BattleManager.Instance.GetLivingEnemies())
        {
            EnemyIndicator indicator = enemy.GetComponent<EnemyIndicator>();
            if (indicator != null) indicator.HideIndicator();
        }
    }

    public void BuildAllyList()
    {
        Debug.Log($"Living allies: {BattleManager.Instance?.GetLivingHeroes().Count}");
        List<GameObject> toDestroy = new List<GameObject>();
        foreach (Transform child in allyPanel.transform)
        if (child.name != "OUTLINE" && child.name != "BACKBUTTON")
        toDestroy.Add(child.gameObject);

        foreach (GameObject obj in toDestroy)
            DestroyImmediate(obj);

        foreach (BaseHero hero in BattleManager.Instance.GetLivingHeroes())
        {
            GameObject btn = Instantiate(buttonPrefab, allyPanel.transform);
            btn.GetComponentInChildren<TMP_Text>().text = hero.combatantName;

            BaseHero heroRef = hero;
            btn.GetComponent<Button>().onClick.AddListener(() => OnAllySelected(heroRef));
        }
    }


    public void OnEnemy1Selected()
{
    OnEnemySelected(BattleManager.Instance.enemies[0]);
}

public void OnEnemy2Selected()
{
    OnEnemySelected(BattleManager.Instance.enemies[1]);
}

    void BuildItemList()
    {
        //populate later
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        actionPanel.SetActive(false);
        abilityPanel.SetActive(false);
        itemPanel.SetActive(false);
        enemyPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
