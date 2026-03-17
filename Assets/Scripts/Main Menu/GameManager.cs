using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public SaveData currentSave;
    public int currentSlot = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void NewGame(int slot)
    {
        currentSlot = slot;
        currentSave = new SaveData();
        currentSave.defeatedEnemyIDs = new List<string>();
        Debug.Log($"New game started in slot {slot}");
    }

    public void SaveGame(List<BaseHero> heroes, Vector3 playerPosition)
    {
        if (currentSave == null) currentSave = new SaveData();

        currentSave.heroes.Clear();
        foreach (var hero in heroes)
        {
            currentSave.heroes.Add(new HeroSaveData
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

        currentSave.playerX = playerPosition.x;
        currentSave.playerY = playerPosition.y;
        currentSave.playerZ = playerPosition.z;

        SaveSystem.Save(currentSlot, currentSave);
    }

    public void LoadGame(int slot)
    {
        currentSlot = slot;
        currentSave = SaveSystem.Load(slot);
    }

    public void AddDefeatedEnemy(string enemyID)
    {
        if (currentSave == null) return;
        if (!currentSave.defeatedEnemyIDs.Contains(enemyID))
            currentSave.defeatedEnemyIDs.Add(enemyID);
    }

    public bool IsEnemyDefeated(string enemyID)
    {
        if (currentSave == null) return false;
        return currentSave.defeatedEnemyIDs.Contains(enemyID);
    }

    public Vector3 GetPlayerPosition()
    {
        if (currentSave == null) return Vector3.zero;
        return new Vector3(currentSave.playerX, currentSave.playerY, currentSave.playerZ);
    }
}