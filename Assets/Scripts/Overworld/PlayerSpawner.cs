using UnityEngine;
using System.Collections.Generic;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject player;
    public List <HeroDefinition> heroDefinitions;

    private bool positionApplied = false;

    void Start()
    {
        Debug.Log($"PlayerSpawner: returnPosition = {BattleData.playerReturnPosition}");
        
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
    }    

    void LateUpdate()
    {
        if (positionApplied) return;
        if (BattleData.playerReturnPosition != Vector3.zero)
        {
            player.transform.position = BattleData.playerReturnPosition;
            Debug.Log($"[PlayerSpawner] Set hero position to {player.transform.position}");
        }

        positionApplied = true;
    }
}
       
