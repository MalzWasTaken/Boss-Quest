using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class TurnManager : MonoBehaviour
{
    public List<Combatant> allCombatants = new List<Combatant>();

    public List<Combatant> GetEnemies(Combatant user)
    {
        return allCombatants.FindAll(c => c.IsAlive && !(c is BaseHero) && user is BaseHero ||
                                         c.IsAlive && !(c is BaseEnemy) && user is BaseEnemy);
    }
}