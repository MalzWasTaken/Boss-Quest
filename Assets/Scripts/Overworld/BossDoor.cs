using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class BossDoor : MonoBehaviour
{
    [Header("Trigger")]
    public float interactRange = 3f;
    public Transform player;

    [Header("Boss Setup")]
    public Transform teleportPoint;       // where the player ends up after confirming
    public BattleFormation bossFormation; // your final boss formation
    public int recommendedLevel = 5;

    [Header("UI References")]
    public GameObject prompt;          // small "Press E to challenge Wrecktrix" UI
    public GameObject confirmPanel;    // bigger panel with warning + Yes/No
    public TMP_Text confirmText;
    public Button yesButton;
    public Button noButton;

    bool playerInRange = false;
    bool confirmOpen = false;

    void Start()
    {
        prompt.SetActive(false);
        confirmPanel.SetActive(false);
        yesButton.onClick.AddListener(EnterBossArena);
        noButton.onClick.AddListener(CloseConfirm);
    }

    void Update()
    {
        if (player == null) return;
        if (confirmOpen) return; // don't track range while confirm is up

        float dist = Vector3.Distance(transform.position, player.position);
        bool nowInRange = dist <= interactRange;

        if (nowInRange != playerInRange)
        {
            playerInRange = nowInRange;
            prompt.SetActive(playerInRange);
        }

        if (playerInRange && Keyboard.current.eKey.wasPressedThisFrame)
        {
            OpenConfirm();
        }
    }

    void OpenConfirm()
    {
        int playerLevel = GetPlayerLevel();
        confirmOpen = true;
        prompt.SetActive(false);
        AudioManager.Instance?.PauseOverworldMusic(); 

        if (playerLevel < recommendedLevel)
        {
            confirmText.text =
                $"Recommended Level: {recommendedLevel}\n" +
                $"Your Level: {playerLevel}\n\n" +
                $"You may not be ready. Once you enter, there is no return.\n\n" +
                $"Challenge Wrecktrix?";
        }
        else
        {
            confirmText.text =
                $"Once you enter, there is no return.\n\n" +
                $"Challenge Wrecktrix?";
        }

        confirmPanel.SetActive(true);

        // Free cursor so player can click Yes/No
        CameraFollow.SuppressCursorLock = true;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        yesButton.Select(); // keyboard focus on Yes by default
    }

    void CloseConfirm()
    {
        confirmOpen = false;
        confirmPanel.SetActive(false);

        // Hand cursor back to normal gameplay
        CameraFollow.SuppressCursorLock = false;
        AudioManager.Instance?.PlayOverworldMusic();

        // If still in range, show prompt again
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= interactRange)
        {
            playerInRange = true;
            prompt.SetActive(true);
        }
    }

    void EnterBossArena()
    {
        confirmPanel.SetActive(false);
        CameraFollow.SuppressCursorLock = false;

        // Teleport so the visual context shifts even though it's the same scene
        if (teleportPoint != null)
            player.position = teleportPoint.position;
    }

    int GetPlayerLevel()
    {
        if (BattleData.heroStats != null && BattleData.heroStats.Count > 0)
            return BattleData.heroStats[0].level;
        return 1;
    }
}