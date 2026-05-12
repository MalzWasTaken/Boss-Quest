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

    [Header("Boss Settings")]
    public float bossDistance = 14f;
    public float bossHeight = 6f;
    public float bossSideOffset = 2f;
    public float bossFOV = 70f;

    [Header("Boss Drift")]
    public float driftAmountX = 0.5f;   // how far left/right
    public float driftAmountY = 0.3f;   // how far up/down
    public float driftSpeedX = 0.4f;    // cycles per second-ish (lower = slower)
    public float driftSpeedY = 0.25f;   // different from X so it doesn't loop obviously

    private Vector3 bossBasePosition;
    private Quaternion bossBaseRotation;
    private bool bossCamReady = false;
    private float currentAngle = 0f;
    private bool isOrbiting;
    private bool isFocusing = false;


    void Start()
    {
        if (BattleData.isFinalBoss)
        {
            SetupBossCamera();
        }
    }

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (BattleData.isFinalBoss)
        {
            if(bossCamReady && !isFocusing) ApplyBossDrift();
            return;
        }

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

    void ApplyBossDrift()
    {
        // Different frequencies on each axis so motion never loops obviously
        float xOffset = Mathf.Sin(Time.time * driftSpeedX * Mathf.PI * 2f) * driftAmountX;
        float yOffset = Mathf.Sin(Time.time * driftSpeedY * Mathf.PI * 2f) * driftAmountY;

        Vector3 targetPos = bossBasePosition + transform.right * xOffset + transform.up * yOffset;
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 3f);
    }

    public void StartOrbiting()
    {
        if(BattleData.isFinalBoss) return;
        isOrbiting = true;
    }

    public void StopOrbiting()
    {
        isOrbiting = false;
    }

    public void EndFocus()
    {
        isFocusing = false;
        if(BattleData.isFinalBoss) SetupBossCamera();
    }

    public void FocusOn(Combatant attacker, Combatant target, bool introShot = false)
    {
        if (BattleData.isFinalBoss)
        {
            isFocusing = true;
            StopAllCoroutines();
            StartCoroutine(SnapToBossBase(0.2f));
            return;
        } 
        isOrbiting = false;
        isFocusing = true;
        StopAllCoroutines();
        StartCoroutine(FocusRoutine(attacker, target, introShot));
    }

    IEnumerator SnapToBossBase(float duration)
    {
        if (!bossCamReady) yield break;
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(startPos, bossBasePosition, t);
            transform.rotation = Quaternion.Slerp(startRot, bossBaseRotation, t);
            yield return null;
        }
        transform.position = bossBasePosition;
        transform.rotation = bossBaseRotation;
    }

    public void FocusOnDefeatedBoss()
    {
        isFocusing = true; // pause any drift
        StopAllCoroutines();
        StartCoroutine(DefeatedBossRoutine());
    }

    IEnumerator DefeatedBossRoutine()
    {
        Transform boss = null;
        if (BattleManager.Instance != null && BattleManager.Instance.enemies.Count > 0)
            boss = BattleManager.Instance.enemies[0].transform;
        if (boss == null) yield break;

        Vector3 startPos = transform.position;
        Vector3 directionToBoss = (boss.position - orbitTarget.position).normalized;
        Vector3 targetPos = boss.position
            - directionToBoss * 6f
            + Vector3.up * 3f;

        float duration = 2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime; // unscaled so slow-mo doesn't slow the camera too
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            transform.LookAt(boss.position + Vector3.up * 1f);
            yield return null;
        }
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
        if (BattleData.isFinalBoss) return;
        isOrbiting = false;
        isFocusing = true;
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
        if (BattleData.isFinalBoss)
        {
            isFocusing = true;
            return;
        }
        isOrbiting = false;
        isFocusing = true;
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

    void SetupBossCamera()
    {
        isOrbiting = false;

        // Find the boss — assume it's the first living enemy in the formation
        Transform boss = null;
        if (BattleManager.Instance != null && BattleManager.Instance.enemies.Count > 0)
            boss = BattleManager.Instance.enemies[0].transform;

        if (boss == null || orbitTarget == null) return;

        // Direction from heroes to boss (use orbitTarget as battlefield center)
        Vector3 directionToBoss = (boss.position - orbitTarget.position).normalized;

        // Position behind the heroes' side, pulled back and raised
        Vector3 camPos = orbitTarget.position
            - directionToBoss * bossDistance
            + Vector3.up * bossHeight
            + Vector3.Cross(directionToBoss, Vector3.up) * bossSideOffset;

        transform.position = camPos;
        Vector3 lookPoint = boss.position - directionToBoss * 2f; // 2 units in front of boss
        lookPoint.y = boss.position.y; // ground level (or wherever the boss's feet are)
        transform.LookAt(lookPoint);

        if (Camera.main != null)
            Camera.main.fieldOfView = bossFOV;

        bossBasePosition = transform.position;
        bossBaseRotation = transform.rotation;
        bossCamReady = true;
    }
}