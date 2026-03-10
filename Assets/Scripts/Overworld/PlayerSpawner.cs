using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject player;

    void Start()
    {
        if (BattleData.playerReturnPosition != Vector3.zero)
        {
            player.transform.position = BattleData.playerReturnPosition;
        }
    }
}
