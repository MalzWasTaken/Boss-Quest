using UnityEngine;
using System.Collections.Generic;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject player;
    public List <HeroDefinition> heroDefinitions;

    void Start()
    {
        
        if(BattleData.heroStats.Count == 0)
        {
            foreach(var def in heroDefinitions)
            {
                BattleData.heroStats.Add(new HeroData
                {
                    heroName = def.heroName,
                    currHP = def.maxHP,
                    maxHP = def.maxHP,
                    currMP = def.maxMP,
                    maxMP = def.maxMP,
                    level = 1,
                    expToNextLevel = def.expToNextLevel
                });
            }
        }
        

        if (BattleData.playerReturnPosition != Vector3.zero)
        {
            player.transform.position = BattleData.playerReturnPosition;
        }

    }
}
