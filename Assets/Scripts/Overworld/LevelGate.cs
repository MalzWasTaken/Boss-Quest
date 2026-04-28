using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class LevelGate : MonoBehaviour
{
    [Header("Gate")]
    public int requiredLevel = 3;
    public Collider wallCollider;

    [Header("UI")]
    public TMP_Text messageText;
    public string lockedMessage = "The path ahead is too dangerous.\nYour party must reach a higher level.";

    private bool playerInRange = false;
    private bool isOpen = false;

    void Update()
    {
        if (isOpen) return;

        if (GetPartyLevel() >= requiredLevel)
            OpenGate();

        if (messageText != null)
            messageText.gameObject.SetActive(playerInRange && !isOpen);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;
        if (!isOpen && messageText != null)
            messageText.text = $"{lockedMessage}\n(Required Level: {requiredLevel})";
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;
    }

    void OpenGate()
    {
        isOpen = true;
        if (wallCollider != null)
            wallCollider.enabled = false;
        if (messageText != null)
            messageText.gameObject.SetActive(false);
    }

    int GetPartyLevel()
    {
        if (BattleData.heroStats.Count == 0) return 1;
        int highest = 0;
        foreach (var hero in BattleData.heroStats)
            if (hero.level > highest) highest = hero.level;
        return highest;
    }
}
