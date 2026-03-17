using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;


public class HealStation : MonoBehaviour   
{
    public float interactRange = 3f;
    public TMP_Text promptText; // E to rest

    private Transform player;
    private bool playerInRange = false;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        if (promptText != null) promptText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        playerInRange = distance <= interactRange;

        if (promptText!= null)
            promptText.gameObject.SetActive(playerInRange);

        if (playerInRange && Keyboard.current.eKey.wasPressedThisFrame)
            Heal();
    }

    void Heal()
    {
        for (int i=0; i< BattleData.heroStats.Count; i++)
        {
            HeroData hero = BattleData.heroStats[i];
            hero.currHP = hero.maxHP;
            hero.currMP = hero.maxMP;
            BattleData.heroStats[i] = hero;
        }

        Debug.Log ("Party fully healed!");

        if (promptText != null)
        {
            promptText.text = "Party fully restored!";
        }
        Invoke(nameof(ResetPrompt), 2f);
    }

    void ResetPrompt()
    {
        if(promptText != null)
            promptText.text = "Press E to fully recover";
    }
}