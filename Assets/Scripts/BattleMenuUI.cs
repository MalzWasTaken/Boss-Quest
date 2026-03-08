using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public enum MenuState
{
    ActionSelect,
    AbilitySelect,
    ItemSelect,
    EnemySelect,
    Inactive
}

public class BattleMenuUI : MonoBehaviour
{
    public GameObject actionPanel; //Attack / abilitiy / item / defend
    public GameObject abilityPanel; //List of ablities
    public GameObject itemPanel; //List of items
    public GameObject enemyPanel; //Enemy select

    public GameObject buttonPrefab;

    public TMP_Text statusText;

    public BattleAction attackAction;
    public BattleAction defendAction;


    private MenuState state;
    private BattleAction pendingAction; //holds chosen action
    
    private BaseHero currentHero;
    public void ShowForHero(BaseHero hero)
    {
        currentHero = hero;
        statusText.text = $"{hero.combatantName} is deciding...";
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
            case MenuState.EnemySelect:
                enemyPanel.SetActive(true);
                BuildEnemyList();
                break;
        }
    }

    //action menu buttons
    public void OnAttackPressed()
    {
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
        BattleManager.Instance.OnActionConfirmed(defendAction, null);
    }

    //abilty and item submenus
    public void OnAbilitySelected(BattleAction ability)
    {
        pendingAction = ability;
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
        BattleManager.Instance.OnActionConfirmed(pendingAction, enemy);
        
    }

    //back button
    public void OnBackPressed()
    {
        switch (state)
        {
            case MenuState.AbilitySelect:
            case MenuState.ItemSelect:
                SetState(MenuState.ActionSelect); //back one level
                break;
            case MenuState.EnemySelect:
            //go back to where we came from
               SetState(MenuState.ActionSelect);
               break;
        }
    }

    void BuildAbilityList(BaseHero hero)
    {
        foreach (Transform child in abilityPanel.transform)
            Destroy(child.gameObject);
        
        foreach (BattleAction action in hero.actions)
        {
            GameObject btn = Instantiate(buttonPrefab, abilityPanel.transform);
            btn.GetComponentInChildren<TMP_Text>().text = action.actionName;
            btn.GetComponent<Button>().onClick.AddListener(() => OnAbilitySelected(action));

            // Grey out if not enough MP
            btn.GetComponent<Button>().interactable = hero.currMP >= action.mpCost;
        }
    }

    public void BuildEnemyList()
    {
        Debug.Log($"Living enemies: {BattleManager.Instance.GetLivingEnemies().Count}");
        List<GameObject> toDestroy = new List<GameObject>();
        foreach (Transform child in enemyPanel.transform)
        if (child.name != "OUTLINE")
        toDestroy.Add(child.gameObject);

        foreach (GameObject obj in toDestroy)
            DestroyImmediate(obj);

        foreach (BaseEnemy enemy in BattleManager.Instance.GetLivingEnemies())
        {
            GameObject btn = Instantiate(buttonPrefab, enemyPanel.transform);
            btn.GetComponentInChildren<TMP_Text>().text = enemy.combatantName;

            BaseEnemy enemyRef = enemy;
            btn.GetComponent<Button>().onClick.AddListener(() => OnEnemySelected(enemyRef));
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
