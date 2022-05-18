#define DEBUG

using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Assertions;
using Vector3 = UnityEngine.Vector3;

/*
 * Boss AI and how it works. The boss will follow a set of instructions on a random interval and place.
 *  (1) Randomly get an attack path from the list of start and end points.
 *  (2) Moves into the position and orientation to perform the dash.
 *  (3) Indicate to the player (via blinking red) where the boss intends to attack for a few seconds.
 *  (4) Does the dash with a lot of speed.
 *  (5) Slowly returns to a rest spot from the list of idle positions.
 *  (6) Begins idling there until its next attack.
 * 
 */

enum AttackStage
{
    PREPARATION = 0,
    ALERTING,
    PRE_ATTACK,
    ATTACKING,
    RESETTING,
    IDLING
}

public class BossAI : MonoBehaviour
{
    private const float PRE_ATTACK_TRANSITION_DURATION = 1f;
    private const float PRE_ATTACK_PAUSE_DURATION = 3f;

    private const float HIT_INDICATOR_MIN_ALPHA = 0.25f;
    private const float HIT_INDICATOR_MAX_ALPHA = 0.6f;

    private const float ATTACKING_TRANSITION_DURATION = 0.5f;
    private const float ATTACKING_PAUSE_DURATION = 1f;

    private const float IDLING_TRANSITION_DURATION = 1f;
    private const float IDLING_BOB_DISTANCE = 0.2f;
    private const float IDLING_BOB_DURATION = 1f;

    private const float ATTACK_COOLDOWN = 5f;

    public List<GameObject> attackStartPositions = null;
    public List<GameObject> attackEndPositions = null;
    public List<GameObject> idlePositions = null;

    [SerializeField] private GameObject arenaCenter = null;
    [SerializeField] private GameObject hitIndicator = null;

    private Vector3 _centerPosition;
    private HitIndicator _hitIndicator;
    private BossMover _bossMover;
    private Health _health;
    private bool _facingRight;

    [SerializeField] private AttackStage attackStage;

    void Start()
    {
        _bossMover = GetComponent<BossMover>();
        _health = GetComponent<Health>();

        Assert.AreEqual(attackStartPositions.Count, attackEndPositions.Count);
        Assert.IsTrue(attackStartPositions.Count > 0);
        Assert.IsTrue(idlePositions.Count > 0);
        Assert.IsNotNull(arenaCenter);
        Assert.IsNotNull(hitIndicator);

        _centerPosition = arenaCenter.transform.position;
        _hitIndicator = hitIndicator.GetComponent<HitIndicator>();

        StartCoroutines(); // I start the boss fight at start but this should occur when the player is done with dialog.

#if DEBUG
        EventManager.Sub(InputManager.GetKeyDownEventName(KeyBinds.CROWCH_DASH_KEY), TestMethod);
#endif
    }

    void StartCoroutines()
    {
        StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        while (_health.health > 0)
        {
            // ================================= (1) PREPARATION =================================

            attackStage = AttackStage.PREPARATION;

            // (1) Randomly get an attack path from the list of start and end points.
            int attackIndex = Random.Range(0, attackStartPositions.Count);
            Vector3 startPosition = attackStartPositions[attackIndex].transform.position;
            Vector3 endPosition = attackEndPositions[attackIndex].transform.position;
            Vector3 centerVector = (endPosition - startPosition) * ((endPosition - startPosition).magnitude / 2);

            // ================================= (2) PRE-ATTACK =================================

            attackStage = AttackStage.PRE_ATTACK;

            // (3) Moves into the position and orientation to perform the dash.
            MoveBoss(true, startPosition,
                false, new Vector3(0, transform.rotation.eulerAngles.y, Vector3.Angle(centerVector, Vector3.zero)),
                PRE_ATTACK_TRANSITION_DURATION);
            yield return new WaitForSeconds(PRE_ATTACK_TRANSITION_DURATION);
            transform.right = -centerVector;

            // ================================= (3) ALERTING =================================

            // (2) Indicate to the player (via blinking red) where the boss intends to attack for a few seconds.
            attackStage = AttackStage.ALERTING;

            _hitIndicator.duration = PRE_ATTACK_PAUSE_DURATION;
            _hitIndicator.startAlpha = HIT_INDICATOR_MIN_ALPHA;
            _hitIndicator.endAlpha = HIT_INDICATOR_MAX_ALPHA;
            _hitIndicator.StartBlinking();

            yield return new WaitForSeconds(PRE_ATTACK_PAUSE_DURATION); // Wait a bit longer for the player.
            _hitIndicator.StopBlinking();

            // ================================= (4) ATTACKING =================================

            attackStage = AttackStage.ATTACKING;

            // (4) Does the dash with a lot of speed.
            MoveBoss(true, endPosition,
                false, Vector3.zero, ATTACKING_TRANSITION_DURATION);
            yield return new WaitForSeconds(ATTACKING_TRANSITION_DURATION);

            yield return new WaitForSeconds(ATTACKING_PAUSE_DURATION); // Wait a bit longer for the player.

            // ================================= (5) RESETTING =================================

            attackStage = AttackStage.RESETTING;

            // (5) Slowly returns to a rest spot from the list of idle positions.
            int idleIndex = Random.Range(0, idlePositions.Count);
            Vector3 idlePosition = idlePositions[idleIndex].transform.position;

            MoveBoss(true, idlePosition,
                true, new Vector3(0, transform.rotation.eulerAngles.y, 0), IDLING_TRANSITION_DURATION);
            yield return new WaitForSeconds(IDLING_TRANSITION_DURATION);
            transform.right = idlePosition - _centerPosition;

            // ================================= (6) IDLING =================================

            attackStage = AttackStage.IDLING;

            // (6) Begins idling there until its next attack.
            MoveBoss(true, transform.position + new Vector3(0, IDLING_BOB_DISTANCE, 0),
                false, Vector3.zero, IDLING_BOB_DURATION, -1);

            yield return new WaitForSeconds(ATTACK_COOLDOWN);
        }
    }

    void TestMethod()
    {
    }

    void MoveBoss(bool move, Vector3 position,
        bool rotate, Vector3 rotation,
        float duration, int loopCount = 0)
    {
        // Break motion for anything done before.
        _bossMover.BreakMotion();

        // Duration and looping.
        _bossMover.loopCount = loopCount;
        _bossMover.duration = duration;

        // If the boss needs to move, set the parameters.
        if (move)
        {
            _bossMover.move = true;
            _bossMover.startPosition = transform.position;
            _bossMover.endPosition = position;
        }
        else
        {
            _bossMover.move = false;
        }

        // If the boss needs to rotate, set the parameters.
        if (rotate)
        {
            _bossMover.rotate = true;
            _bossMover.startRotation = transform.rotation.eulerAngles;
            _bossMover.endRotation = rotation;
        }
        else
        {
            _bossMover.rotate = false;
        }

        // Move the boss asynchronously (passive movement).
        _bossMover.TriggerMotion();
    }
}