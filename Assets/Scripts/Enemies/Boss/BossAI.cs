#define DEBUG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Vector3 = UnityEngine.Vector3;

/*
 * Boss AI and how it works. The boss will follow a set of instructions on a random interval and place.
 *  (1) Randomly get a close attack path from the list of start and end points. (right attacks vs left side attacks)
 *  (2) Moves into the position and orientation to perform the dash.
 *  (3) Indicate to the player (via blinking red) where the boss intends to attack for a few seconds.
 *  (4) Does the dash with a lot of speed. (With no indicator)
 *  (5) Slowly returns to a rest spot from the list of idle positions. (was random now it's the nearest)
 *  (6) Begins idling there until its next attack.
 */
#if DEBUG
enum AttackStage
{
    PREPARATION = 0,
    ALERTING,
    PRE_ATTACK,
    ATTACKING,
    RESETTING,
    IDLING
}
#endif

public class BossAI : MonoBehaviour
{
    [Header("Right Attacks")] public List<GameObject> rightAttackStartPos;
    public List<GameObject> rightAttackEndPos;

    [Header("Left Attacks")] public List<GameObject> leftAttackStartPos;
    public List<GameObject> leftAttackEndPos;

    [Header("Idle Spots")] public List<GameObject> idlePositions;

    [Header("Utilities")] [SerializeField] private GameObject arenaCenter;
    [SerializeField] private GameObject hitIndicator;
    [SerializeField] private GameObject bossSprite;

    private Vector3 _centerPosition;
    private HitIndicator _hitIndicator;
    private HitIndicator _bossSpriteIndicator;
    private BossMover _bossMover;
    private Health _health;
    private BossCutsceneManager _bossCutsceneManager;
    private Weapon _weapon;
    private Animator _animator;

    private IEnumerator _attackCoroutine;

#if DEBUG
    [SerializeField] private AttackStage attackStage;
#endif

    void Start()
    {
        Assert.AreEqual(rightAttackStartPos.Count, leftAttackStartPos.Count);
        Assert.AreEqual(rightAttackEndPos.Count, leftAttackEndPos.Count);
        Assert.IsTrue(rightAttackStartPos.Count > 0);
        Assert.IsTrue(idlePositions.Count > 0);
        Assert.IsNotNull(arenaCenter);
        Assert.IsNotNull(hitIndicator);
        Assert.IsNotNull(bossSprite);

        _bossMover = GetComponent<BossMover>();
        _health = GetComponent<Health>();
        if (_health)
        {
            _health.Death += OnDeath;
            _health.HealthChanged += OnHealthChanged;
        }

        _centerPosition = arenaCenter.transform.localPosition;
        _hitIndicator = hitIndicator.GetComponent<HitIndicator>();
        _bossSpriteIndicator = bossSprite.GetComponent<HitIndicator>();
        _animator = GetComponentInChildren<Animator>();

        _weapon = GetComponent<Weapon>();
        Assert.IsNotNull(_weapon);

        _bossCutsceneManager = GetComponent<BossCutsceneManager>();
        Assert.IsNotNull(_bossCutsceneManager);

        _attackCoroutine = Attack();

#if DEBUG
        EventManager.Sub(InputManager.GetKeyDownEventName(KeyBinds.DEBUG8), StartBossFight);
        EventManager.Sub(InputManager.GetKeyDownEventName(KeyBinds.DEBUG9), OnDeath);
#endif

        //StartBossFight(); // I start the boss fight at start but this should occur when the player is done with dialog.
    }

    public void StartBossFight()
    {
		FindObjectOfType<SoundManager>().PlayBossLaugh();
        StartCoroutine(_attackCoroutine);
        _animator.SetBool("inFight", true);
    }

    void OnDeath()
    {
		FindObjectOfType<PlayerController>().FreezeInput(true);
        // Stop the boss wherever it died
        StopCoroutine(_attackCoroutine);
        _bossMover.BreakMotion();

        // Don't let the boss be able to hurt the player anymore.
        GetComponent<TouchDamage>().Active = false;

        // Do the death animation.
        _bossCutsceneManager.AnimateDeath();

        // TODO replace idle aniamtion with death aniamtion
        _animator.SetBool("inFight", false);

#if DEBUG
        Debug.Log("The boss is dying!");
#endif
    }

    void OnHealthChanged(float oldHealth, float newHealth)
    {
        _bossSpriteIndicator.IndicateBlinking(BossParameters.BOSS_HIT_FLASH_MIN_ALPHA,
            BossParameters.BOSS_HIT_FLASH_MAX_ALPHA, BossParameters.BOSS_HIT_FLASH_DURATION);
    }

    IEnumerator Attack()
    {
        while (_health.health > 0)
        {
            // ================================= (1) PREPARATION =================================
#if DEBUG
            attackStage = AttackStage.PREPARATION;
#endif

            // (1) Randomly get an attack path from the list of start and end points from the specific side.
            Vector3 startPosition;
            Vector3 endPosition;
            Vector3 centerVector;
            if (CheckIfOnRight())
            {
                int attackIndex = Random.Range(0, rightAttackStartPos.Count);
                startPosition = rightAttackStartPos[attackIndex].transform.localPosition;
                endPosition = rightAttackEndPos[attackIndex].transform.localPosition;
                centerVector = (endPosition - startPosition) * ((endPosition - startPosition).magnitude / 2);
            }
            else
            {
                int attackIndex = Random.Range(0, leftAttackStartPos.Count);
                startPosition = leftAttackStartPos[attackIndex].transform.localPosition;
                endPosition = leftAttackEndPos[attackIndex].transform.localPosition;
                centerVector = (endPosition - startPosition) * ((endPosition - startPosition).magnitude / 2);
            }

            // ================================= (2) PRE-ATTACK =================================
#if DEBUG
            attackStage = AttackStage.PRE_ATTACK;
#endif

            // (3) Moves into the position and orientation to perform the dash.
            _bossMover.MoveBoss(true, startPosition,
                false, new Vector3(0, transform.localRotation.eulerAngles.y, Vector3.Angle(centerVector, Vector3.zero)),
                BossParameters.PRE_ATTACK_TRANSITION_DURATION);
            yield return new WaitForSeconds(BossParameters.PRE_ATTACK_TRANSITION_DURATION);
            transform.right = -centerVector;

            // ================================= (3) ALERTING =================================
#if DEBUG
            attackStage = AttackStage.ALERTING;
#endif

            // (2) Indicate to the player (via blinking red) where the boss intends to attack for a few seconds.
            _hitIndicator.IndicateBlinking(BossParameters.HIT_INDICATOR_MIN_ALPHA,
                BossParameters.HIT_INDICATOR_MAX_ALPHA, BossParameters.PRE_ATTACK_PAUSE_DURATION);
            _bossSpriteIndicator.ChangeColor(Color.red);

            yield return
                new WaitForSeconds(BossParameters.PRE_ATTACK_PAUSE_DURATION); // Wait a bit longer for the player.
            _hitIndicator.StopBlinking();

            // ================================= (4) ATTACKING =================================
#if DEBUG
            attackStage = AttackStage.ATTACKING;
#endif

            // (4) Does the dash with a lot of speed.
            _weapon.damage = 100f;
			FindObjectOfType<SoundManager>().PlayBossAttack();
            _bossMover.MoveBoss(true, endPosition,
                false, Vector3.zero, BossParameters.ATTACKING_TRANSITION_DURATION);
            yield return new WaitForSeconds(BossParameters.ATTACKING_TRANSITION_DURATION);

            yield return
                new WaitForSeconds(BossParameters.ATTACKING_PAUSE_DURATION); // Wait a bit longer for the player.

            _weapon.damage = 1f;

            // ================================= (5) RESETTING =================================
#if DEBUG
            attackStage = AttackStage.RESETTING;
#endif

            // (5) Slowly returns to a rest spot from the list of idle positions.
            _bossSpriteIndicator.ChangeColor(Color.white);
            Vector3 idlePosition = GetClosestIdlePosition();

            _bossMover.MoveBoss(true, idlePosition,
                true, new Vector3(0, transform.localRotation.eulerAngles.y, 0),
                BossParameters.IDLING_TRANSITION_DURATION);
            yield return new WaitForSeconds(BossParameters.IDLING_TRANSITION_DURATION);
            transform.right = idlePosition - _centerPosition;

            // ================================= (6) IDLING =================================
#if DEBUG
            attackStage = AttackStage.IDLING;
#endif

            // (6) Begins idling there until its next attack.
            _bossMover.MoveBoss(true, transform.localPosition + new Vector3(0, BossParameters.IDLING_BOB_DISTANCE, 0),
                false, Vector3.zero, BossParameters.IDLING_BOB_DURATION, -1);

            yield return new WaitForSeconds(BossParameters.ATTACK_COOLDOWN);
        }
    }

    bool CheckIfOnRight()
    {
        return _centerPosition.x < transform.localPosition.x;
    }

    Vector3 GetClosestIdlePosition()
    {
        Vector3 bossPos = transform.localPosition;
        int closest = -1;
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < idlePositions.Count; i++)
        {
            float distance = Vector3.Distance(idlePositions[i].transform.localPosition, bossPos);
            if (distance < closestDistance)
            {
                closest = i;
                closestDistance = distance;
            }
        }

        return idlePositions[closest].transform.localPosition;
    }
}