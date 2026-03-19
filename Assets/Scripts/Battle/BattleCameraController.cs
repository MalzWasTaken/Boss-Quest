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

    public void FocusOn(Combatant attacker, Combatant target)
    {
        isOrbiting = false;
        StopAllCoroutines();
        StartCoroutine(FocusRoutine(attacker, target));
    }

    IEnumerator FocusRoutine(Combatant attacker, Combatant target)
    {
        //postioning behind a hero
        Transform hero = attacker is BaseHero ? attacker.transform : target.transform;
        Transform enemy = attacker is BaseEnemy ? attacker.transform : target.transform;

        Vector3 directionToEnemy = (enemy.position - hero.position).normalized;

        //position behind the anchor
        Vector3 camPos = hero.position 
            - directionToEnemy * focusDistance
            + Vector3.up * focusHeight;

        transform.position = camPos;
        transform.LookAt(target.transform.position + Vector3.up * -2f);

        yield return null;
    }
}