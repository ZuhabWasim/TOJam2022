#define DEBUG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/*
 * Boss AI and how it works. The boss will follow a set of instructions on a random interval and place.
 *  (1) Randomly get an attack path from the list of start and end points.
 *  (2) Indicate to the player (via blinking red) where the boss intends to attack for a few seconds.
 *  (3) Moves into the position and orientation to perform the dash.
 *  (4) Does the dash with a lot of speed.
 *  (5) Slowly returns to a rest spot from the list of idle positions.
 *  (6) Begins idling there until its next attack.
 * 
 */
public class BossAI : MonoBehaviour
{
    // Start is called before the first frame update

    // Dash attack
    /*
     * Dash Attack
     * - Start position
     * - End position
     * - Length for the whole attack
     * - orientation while attacking
     * - End position and orientation
     */

    private const float IDLING_TRANSITION_TIME = 1f;
    private const float IDLING_BOB_DISTANCE = 0.2f;
    private const float IDLING_BOB_DURATION = 1f;

    [SerializeField] private GameObject arenaCenter = null;
    private Vector3 _centerPosition;

    public List<GameObject> attackStartPositions = null;
    public List<GameObject> attackEndPositions = null;

    public List<GameObject> idlePositions = null;


    [SerializeField] private float _attackCooldownLength = 5f;
    private float _attackCooldown = 0f;

    private BossMover _bossMover;
    private bool _facingRight;

    void Start()
    {
        _bossMover = GetComponent<BossMover>();
        _centerPosition = arenaCenter == null ? Vector3.zero : arenaCenter.transform.position;

        Assert.AreEqual(attackStartPositions.Count, attackEndPositions.Count);
        Assert.IsTrue(attackStartPositions.Count > 0);
        Assert.IsTrue(idlePositions.Count > 0);
        Assert.IsNotNull(arenaCenter);

#if DEBUG
        EventManager.Sub(InputManager.GetKeyDownEventName(KeyBinds.CROWCH_DASH_KEY), TestMethod);
#endif
    }

    // Update is called once per frame
    void Update()
    {
        TickCooldowns();

        if (_attackCooldown == 0f)
        {
            StartCoroutine(Attack());
            _attackCooldown = _attackCooldownLength;
        }
    }

    IEnumerator Attack()
    {
        // Get information about the next attack.
        /*int attackIndex = Random.Range(0, attackStartPositions.Count);
        Vector3 startPosition = attackStartPositions[attackIndex].transform.position;
        Vector3 endPosition = attackEndPositions[attackIndex].transform.position;
        Vector3 centerVector = (endPosition - startPosition) * ((endPosition - startPosition).magnitude / 2);*/

        // Hit box indicator for 3 seconds

        // Line up the attack

        // Perform the dash

        // Randomly choose from a list of idling positions.

        // Get the position of idling.
        int idleIndex = Random.Range(0, idlePositions.Count);
        Vector3 idlePosition = idlePositions[idleIndex].transform.position;

        // Move the boss to idling position.
        MoveBoss(true, idlePosition,
            true, new Vector3(0, transform.rotation.eulerAngles.y, 0), IDLING_TRANSITION_TIME);
        yield return new WaitForSeconds(IDLING_TRANSITION_TIME);
        CheckBossOrientation(idlePosition - _centerPosition);

        MoveBoss(true, transform.position + new Vector3(0, IDLING_BOB_DISTANCE, 0),
            false, Vector3.zero, IDLING_BOB_DURATION, -1);
    }
    /*void LineUpAttackTransition()
    {
        
    }

    IEnumerator LineUpAttack(Vector3 position, Vector3 rotation, bool facingRight)
    {
        
    }*/

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


    void CheckBossOrientation(Vector3 direction)
    {
        bool facingRight = GetFacingRight(direction);
        if (facingRight && !_facingRight)
        {
            FlipBoss();
        }
        else if (!facingRight && _facingRight)
        {
            FlipBoss();
        }
    }

    bool GetFacingRight(Vector3 direction)
    {
        float angleRight = Vector3.Angle(direction, Vector3.right);
        float angleLeft = Vector3.Angle(direction, Vector3.left);
        return angleRight > angleLeft;
    }

    void FlipBoss()
    {
        _facingRight = !_facingRight;
        transform.Rotate(0f, 180f, 0f);
    }


    void TickCooldowns()
    {
        _attackCooldown = Mathf.Max(0f, _attackCooldown - Time.deltaTime);
    }
}