using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject player;

    void Start()
    {
        
        if(BattleData.heroStats.Count == 0)
        {
            BattleData.heroStats.Add(new HeroData
            {
                heroName = "Malzino",
                currHP = 100, maxHP = 100,
                currMP = 10, maxMP = 10,
                level = 1,
                expToNextLevel = 100
            });

            BattleData.heroStats.Add(new HeroData
            {
                heroName = "SlimeKiller",
                currHP = 100,maxHP = 100,
                currMP = 30, maxMP = 30,
                level = 1,
                expToNextLevel = 100
            });
        }

        if (BattleData.playerReturnPosition != Vector3.zero)
        {
            player.transform.position = BattleData.playerReturnPosition;
        }

    }
}
