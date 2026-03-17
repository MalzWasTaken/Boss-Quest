using UnityEngine;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class HeroSaveData
{
    public string heroName;
    public float currHP;
    public float currMP;
    public float maxHP;
    public float maxMP;
    public int experience;
    public int gold;
    public int level;
    public int expToNextLevel;
}

[System.Serializable]
public class SaveData
{
    public int slotIndex;
    public string saveDateTime;
    public List<HeroSaveData> heroes = new List<HeroSaveData>();
    public float playerX;
    public float playerY;
    public float playerZ;
    public List<string> defeatedEnemyIDs = new List<string>();
    public int totalBattlesWon;
}

public static class SaveSystem
{
    private const int SLOT_COUNT = 3;

    static string GetPath(int slot) =>
        Path.Combine(Application.persistentDataPath, $"save_slot_{slot}.json");

    public static void Save(int slot, SaveData data)
    {
        data.slotIndex = slot;
        data.saveDateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetPath(slot), json);
        Debug.Log($"Game saved to slot {slot}");
    }

    public static SaveData Load(int slot)
    {
        string path = GetPath(slot);
        if (!File.Exists(path)) return null;
        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<SaveData>(json);
    }

    public static bool SlotExists(int slot) =>
        File.Exists(GetPath(slot));

    public static void Delete(int slot)
    {
        string path = GetPath(slot);
        if (File.Exists(path)) File.Delete(path);
    }

    public static int GetSlotCount() => SLOT_COUNT;
}