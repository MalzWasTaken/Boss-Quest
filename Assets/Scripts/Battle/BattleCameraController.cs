using UnityEngine;
using System.Collections;

public class BattleCameraController : MonoBehaviour
{
    public static BattleCameraController Instance;

    [Header("Orbit Settings")]
    public Transform orbitTarget; //empty gameobject center battlefield
    public float orbitRadius = 8f;
    public float orbitHeight = 3f;
    public float orbitSpeed = 20f; //degrees per second

    [Header("Focus Settings")]
    public float focusDistance = 4f;
    public float focusHeight = 2f;
    public float focusOffsetSide = 1.5f; //slight offset

    private float currentAngle = 0f;
    private bool isOrbiting;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (isOrbiting)
        {
            currentAngle += orbitSpeed * Time.deltaTime;
            float rad = currentAngle * Mathf.Deg2Rad;
           Vector3 offset = new Vector3(
                Mathf.Sin(rad) * orbitRadius,
                orbitHeight,
                Mathf.Cos(rad) * orbitRadius
           );

            transform.position = orbitTarget.position + offset;
            transform.LookAt(orbitTarget.position + Vector3.up * -1f);
        }
    }

    public void StartOrbiting()
    {
        isOrbiting = true;
    }

    public void StopOrbiting()
    {
        isOrbiting = false;
    }

    public void FocusOn(Combatant attacker, Combatant target, bool introShot = false)
    {
        isOrbiting = false;
        StopAllCoroutines();
        StartCoroutine(FocusRoutine(attacker, target, introShot));
    }

    IEnumerator FocusRoutine(Combatant attacker, Combatant target, bool introShot = false)
    {
        Transform hero = attacker is BaseHero ? attacker.transform : target.transform;
        Transform enemy = attacker is BaseEnemy ? attacker.transform : target.transform;

        Vector3 directionToEnemy = (enemy.position - hero.position).normalized;
        Vector3 sideOffset = Vector3.Cross(directionToEnemy, Vector3.up) * focusOffsetSide;
        Vector3 camPos;

        
            camPos = hero.position
                - directionToEnemy * focusDistance
                + Vector3.up * focusHeight
                + sideOffset;
      

        transform.position = camPos;
        transform.LookAt(enemy.position + Vector3.up * -0.5f);

        yield return null;
    }

    public void FocusIntro(Combatant enemy)
    {
        isOrbiting = false;
        StopAllCoroutines();
        StartCoroutine(IntroRoutine(enemy));
    }

    IEnumerator IntroRoutine(Combatant enemy)
    {
        Vector3 heroCenter = Vector3.zero;
        foreach (var hero in BattleManager.Instance.heroes)
            heroCenter += hero.transform.position;
        heroCenter /= BattleManager.Instance.heroes.Count;

        Vector3 directionToEnemy = (enemy.transform.position - heroCenter).normalized;

        // Hero side but pushed closer toward the enemy
        Vector3 camPos = enemy.transform.position
            - directionToEnemy * (8 * 0.7f)  // closer than normal
            + Vector3.up * 3;

        transform.position = camPos;
        transform.LookAt(enemy.transform.position + Vector3.up * -0.5f);

        yield return null;
    }
    public void FocusOnHeal(Combatant target)
    {
        isOrbiting = false;
        StopAllCoroutines();
        StartCoroutine(HealFocusRoutine(target));
    }



    IEnumerator HealFocusRoutine(Combatant target)
    {
        // Position on the enemy side looking at the hero
        Vector3 enemySide = Vector3.zero;
        foreach (var enemy in BattleManager.Instance.enemies)
        {
            if (enemy.IsAlive)
            {
                enemySide = enemy.transform.position;
                break;
            }
        }

        Vector3 directionToHero = (target.transform.position - enemySide).normalized;
        Vector3 camPos = target.transform.position
            - directionToHero * focusDistance
            + Vector3.up * focusHeight;

        transform.position = camPos;
        transform.LookAt(target.transform.position + Vector3.up * -0.5f);

        yield return null;
    }
}