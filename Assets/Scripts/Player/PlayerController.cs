#define DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    // Player Tag
    public static string PLAYER = "Player";

    // MOVEMENT CONSTANTS
    private const string HORIZONTAL_AXIS = "Horizontal";
    private const string VERTICAL_AXIS = "Vertical";
    private const float GRAVITY_SCALE = 5f;

    private const float GROUND_CHECK_RADIUS = 0.38f;
    private const float CEILING_CHECK_RADIUS = 0.35f;

    // Armour constants.
    private const float POST_CHESTPPIECE_SPEED = 1000f;
    private const float PRE_CHESTPIECE_SPEED = 350f;
    private const float STARTING_MOVE_SPEED = 4f;

    // Misc constants.
    private const float ROPE_SNAP_OFFSET = 0f;
    private const float CLIMBING_LATERAL_REDUCTION = 0.5f;
    private const float CROUCH_DOWN_OFFSET = 0.33f;
    private const float RESPAWN_POINT_OFFSET = 1f;

    [Header("Movement")] public float moveSpeed;
    public float jumpForce;
    public float terminalVelocity = 20f;

    // CROWCH DASH +++
    [SerializeField] private float _dashForce;
    [SerializeField] private float _StartDashTime;
    private float _dashTime;

    [Header("Climbing")] public float climbingSpeed;
    public float climbCooldownDuration = 0.3f;
    public float climbGraceDuration = 0.2f;
    public LayerMask climbingLayer;

    [Header("Collision")] public Transform groundCheck;
    public Transform ceilingCheck;
    public LayerMask groundObjects;
    public float checkRadius;

    [Header("Combat")] public WeaponController weaponController;
    [SerializeField] private float attackSpeed = 0.5f;

    [Header("Animation")] public GameObject sprite;
    public GameObject playerCenter;
    public Animator animator;
    public ParticleSystem particles;

    [Header("Camera")] [SerializeField] Transform cameraFollowPoint;
    [SerializeField] float lookaheadMinimumHoldTime = 1f;
    [SerializeField] [Range(0, 3f)] float lookaheadDistance;


    [Header("RuntimeController")] [SerializeField]
    private RuntimeAnimatorController a_Armored;

    [SerializeField] private RuntimeAnimatorController a_ChestPiece;
    [SerializeField] private RuntimeAnimatorController a_Gauntlets;
    [SerializeField] private RuntimeAnimatorController a_Unarmoured;

    // Player movement values.
    private Rigidbody2D _rigidbody;
    private float _lateralMovement;
    private float _verticalMovement;

    // Timers for movement/input restrictions/leniency.
    private float _crouchDashCooldown;
    private float _climbTimer;
    private float _climbGrace;

    // Player movement states.
    private bool _climbPress; // The player *wants* to climb.
    private bool _onRope; // The player is near a rope.
    private float _ropeX; // Where the rope is located to latch on to.
    private bool _climbing; // The player is currently climbing.

    private bool _crouchPress; // The player *wants* to crouch.
    private bool _crouching = false; // The player is currently crouching.
    private bool _isCrouchDashing;

    private bool _isJumping = false; // The player invokes jumping.
    private bool _isGrounded; // The player is currently on the ground.
    private bool _headBumped; // The player's head is touching a ceiling.

    private bool _inputFrozen = false;

    public delegate void OnInputFreeze(bool frozen);

    public event OnInputFreeze InputFreeze;

    // Sprite information.
    private bool _facingRight = true;
    private Vector2 _colliderSize;
    private Vector3 _spriteScale;

    // Armour Pieces.
    private bool _chestPiece = false;
    private bool _gauntlets = false;

    public bool _leggings = false;
    private float _attackCooldown = 0f;

    // Camera related
    private float _verticalLookCooldown = 0f;

    // Awake is called after initialization of all objects.
    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializeVariables();
        RegisterEventListeners();
        PutOnArmour();
        _lateralMovement = 0f;
        GameObject go = GameObject.Find("TutorialTrigger");
        DialogueTrigger trigger = (DialogueTrigger) go.GetComponent(typeof(DialogueTrigger));
        trigger.TriggerDialogue();
    }

    void InitializeVariables()
    {
        _colliderSize = GetComponent<CapsuleCollider2D>().size;
        _spriteScale = sprite.transform.localScale;
        _dashTime = _StartDashTime;
        if (cameraFollowPoint == null)
        {
            Debug.LogWarning("You should add a camera follow point, assigning self for now");
            cameraFollowPoint = this.transform;
        }
    }

    void RegisterEventListeners()
    {
        EventManager.Sub(InputManager.GetKeyDownEventName(KeyBinds.JUMP_KEY), HandleJump);
        EventManager.Sub(InputManager.GetKeyDownEventName(KeyBinds.ATTACK_KEY), HandleAttack);

        // What to do when the player dies.
        Health playerHealth = GetComponent<Health>();
        playerHealth.Death += playerHealth_OnDeath;

#if DEBUG
        // Placeholder events for each armour piece lost.
        EventManager.Sub(InputManager.GetKeyDownEventName(KeyBinds.DEBUG1), LostChestPiece);
        EventManager.Sub(InputManager.GetKeyDownEventName(KeyBinds.DEBUG2), LostGauntlets);
        EventManager.Sub(InputManager.GetKeyDownEventName(KeyBinds.DEBUG3), LostLeggings);
#endif
    }

    private void FixedUpdate()
    {
        // Only be able to jump again if we come back down. Put this back in Update because it works better.
        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, GROUND_CHECK_RADIUS, groundObjects);
        _headBumped = Physics2D.OverlapCircle(ceilingCheck.position, CEILING_CHECK_RADIUS, groundObjects);
        MovePlayer();
        CrouchDash();
    }

    // Update is called once per frame
    void Update()
    {
        // Get what the player *wants* to do (via button presses).
        GetPlayerInput();

        // Handle what the player *is allowed* to do in their current situation.
        RefreshPlayerStates();

        // Refresh any leniency counters, e.g. climbing prevention.
        TickCooldowns();

        // Animate the player's actions to the Animation Controller.
        AnimatePlayer();
    }

    void GetPlayerInput()
    {
        if (_inputFrozen) return;

        _lateralMovement = Input.GetAxis(HORIZONTAL_AXIS);
        _verticalMovement = Input.GetAxisRaw(VERTICAL_AXIS);
        _climbPress = _gauntlets && _verticalMovement > 0f;
        _crouchPress = _leggings && _verticalMovement < 0f;

        // Allows the player to jump off of ropes.
        if (_gauntlets && _onRope && _verticalMovement != 0 && _climbTimer == 0f)
        {
            _climbing = true;
            if (_climbGrace == 0f)
            {
                _climbGrace = climbGraceDuration;
            }

            return;
        }

        // Prioritize climbing over crouching if both are pressed.
        if (_climbPress)
        {
            _crouchPress = false;
        }

        // Lookahead Camera Related
        if ((Math.Abs(_verticalMovement) > 0) && !_climbing)
        {
            _verticalLookCooldown = _verticalLookCooldown == -1 ? lookaheadMinimumHoldTime : _verticalLookCooldown;
            if (_verticalLookCooldown == 0)
            {
                cameraFollowPoint.localPosition = new Vector2(0, lookaheadDistance * _verticalMovement);
                _verticalLookCooldown = -1;
            }
        }
        else if (Math.Abs(_verticalMovement) == 0)
        {
            // We run this each update...
            // Good enough for now?
            _verticalLookCooldown = -1;
            cameraFollowPoint.localPosition = new Vector2(0, 0);
        }

        // Player is in the air else wise.
        if (!_isGrounded)
        {
            _crouchPress = false;
            _climbPress = false;
            return;
        }

        // Prioritize climbing over crouching if both are pressed.
        if (_crouchPress && _climbPress)
        {
            _crouchPress = false;
        }
    }

    void CreateDust()
    {
        particles?.Play();
    }

    void RefreshPlayerStates()
    {
        // Changing direction of the player.
        if (_lateralMovement > 0 && !_facingRight && !_climbing)
        {
            CreateDust();
            FlipCharacter();
        }
        else if (_lateralMovement < 0 && _facingRight && !_climbing)
        {
            CreateDust();
            FlipCharacter();
        }

        // Stop the character from climbing after they reach the bottom.
        if (_climbing)
        {
            if (_isGrounded && _climbGrace == 0f)
            {
                _climbing = false;
            }

            CrouchCharacter(false);
            return;
        }

        // Ensure the player can't crouch when in the air.
        if (!_isGrounded)
        {
            CrouchCharacter(false);
            return;
        }

        // Crouch the player if they want to.
        if (_crouchPress)
        {
            CrouchCharacter(true);
            return;
        }

        // If none of these states, the player must be idle.
        CrouchCharacter(false);
    }

    void AnimatePlayer()
    {
        // Set parameters for all movement controllers.
        animator.SetBool("isCrouching", _crouching);
        animator.SetBool("isJumping", !_isGrounded);
        animator.SetBool("isAttacking", _attackCooldown > 0f);
        animator.SetFloat("horizontalSpeed", Mathf.Abs(_lateralMovement));
        animator.SetBool("isClimbing",
            _climbing && (Mathf.Abs(_verticalMovement) > 0.01f || Mathf.Abs(_lateralMovement) > 0.01f));
        animator.SetBool("isOnRope", _climbing);
        animator.SetBool("isCrouchSliding", _isCrouchDashing);
    }

    void FlipCharacter()
    {
        _facingRight = !_facingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    // Shortens the character based on if they're crouching. Note not the local transform but just the collider and sprite.
    void CrouchCharacter(bool crouch)
    {
        CapsuleCollider2D collider = this.GetComponent<CapsuleCollider2D>();

        if (crouch && !_crouching)
        {
            // Make the player shorter.
            collider.size = new Vector2(_colliderSize.x, _colliderSize.y / 2);
            sprite.transform.localScale = new Vector3(_spriteScale.x, _spriteScale.y * 0.8f, _spriteScale.z);

            // Sprite specific adjustments.
            Vector3 position = sprite.transform.position;
            sprite.transform.position = new Vector3(position.x, position.y + 0.2f, position.z);

            // Push the player up a bit to avoid clipping through the ground.
            position = transform.position;
            this.transform.position = new Vector3(position.x, position.y - CROUCH_DOWN_OFFSET, position.z);

            _crouching = true;
        }
        else if (!crouch && _crouching)
        {
            // Don't let the player stand up in a tunnel.
            if (_headBumped) return;

            // Return the player to their original height.
            collider.size = new Vector2(_colliderSize.x, _colliderSize.y);
            sprite.transform.localScale = new Vector3(_spriteScale.x, _spriteScale.y, _spriteScale.z);

            // Sprite specific adjustments.
            Vector3 position = sprite.transform.position;
            sprite.transform.position = new Vector3(position.x, position.y - 0.2f, position.z);

            // Push the player up a bit to avoid clipping through the ground.
            position = transform.position;
            this.transform.position = new Vector3(position.x, position.y + CROUCH_DOWN_OFFSET, position.z);

            _crouching = false;
        }
    }

    void MovePlayer()
    {
        // Disable gravity to only rely on up/down input when the player is climbing.
        if (_climbing)
        {
            _rigidbody.gravityScale = 0f;
            // Snap the player to the ladder block
            //SnapToRope();
            _rigidbody.velocity = new Vector2(_lateralMovement * moveSpeed * CLIMBING_LATERAL_REDUCTION,
                _verticalMovement * climbingSpeed);
            if (!FindObjectOfType<SoundManager>().audioSource.isPlaying && _rigidbody.velocity != new Vector2(0f,0f))
            {
                FindObjectOfType<SoundManager>().PlayClimb();
            }
        }
        else
        {
            _rigidbody.gravityScale = GRAVITY_SCALE;
            _rigidbody.velocity = new Vector2(_lateralMovement * moveSpeed,
                Mathf.Clamp(_rigidbody.velocity.y, -terminalVelocity, terminalVelocity));
            if (Math.Abs(_rigidbody.velocity.x) > 0.1 && !_crouching && _isGrounded &&
                !FindObjectOfType<SoundManager>().audioSource.isPlaying)
            {
                FindObjectOfType<SoundManager>().PlayRun();
            }
        }

        // If the player scheduled a jump, trigger it once and set it to false.
        if (_isJumping)
        {
            // Re-enable regular movement
            _rigidbody.gravityScale = GRAVITY_SCALE;
            _rigidbody.velocity = new Vector2(_lateralMovement * moveSpeed,
                Mathf.Clamp(_rigidbody.velocity.y, -terminalVelocity, terminalVelocity));

            // Add the jump velocity.
            CreateDust();
            FindObjectOfType<SoundManager>().PlayJump();
            _rigidbody.AddForce(new Vector2(0f, jumpForce));

            // Set climbing on cooldown for a bit.
            _climbing = false;
            _climbTimer = climbCooldownDuration;
        }

        _isJumping = false;

        // Limit the player's movement speed if they're crouching.
        if (_crouching)
        {
            _rigidbody.velocity = new Vector2(0f, _verticalMovement);
        }
    }

    void CrouchDash()
    {
        if (!_isCrouchDashing)
        {
            return;
        }

        //dash if timer is greater than 0
        if (_dashTime >= 0)
        {
            if (!FindObjectOfType<SoundManager>().audioSource.isPlaying)
            {
                FindObjectOfType<SoundManager>().PlaySlide();
            }

            _dashTime -= Time.deltaTime;
            Vector2 velocity = _rigidbody.velocity;
            if (!_facingRight)
            {
                _rigidbody.velocity = new Vector2(Vector2.left.x * _dashForce, velocity.y);
            }
            else if (_facingRight)
            {
                _rigidbody.velocity = new Vector2(Vector2.right.x * _dashForce, velocity.y);
            }
        }

        //reset dash time and dash bool
        if (_dashTime <= 0)
        {
            _isCrouchDashing = false;
            _dashTime = _StartDashTime;
        }
    }


    void SnapToRope()
    {
        Vector3 position = _rigidbody.transform.position;
        _rigidbody.transform.position = new Vector3(_ropeX - ROPE_SNAP_OFFSET, position.y, position.z);
    }

    void TickCooldowns()
    {
        _climbTimer = Mathf.Max(0f, _climbTimer - Time.deltaTime);
        _climbGrace = Mathf.Max(0f, _climbGrace - Time.deltaTime);
        _attackCooldown = Mathf.Max(0f, _attackCooldown - Time.deltaTime);
        _verticalLookCooldown = _verticalLookCooldown >= 0
            ? Mathf.Max(0f, _verticalLookCooldown - Time.deltaTime)
            : _verticalLookCooldown; // Has a -1 state
    }

    void HandleJump()
    {
        if (_inputFrozen) return;
        if (!_crouching && (_isGrounded || _climbing))
        {
            _isJumping = true;
            _crouchPress = false;
            _climbPress = false;
            CreateDust();
#if DEBUG
            Debug.Log("JUMP!!");
#endif
            return;
        }

        if (_crouching)
        {
            _isCrouchDashing = true;
            CreateDust();
#if DEBUG
            Debug.Log("CROUCH DASH!!");
#endif
        }
    }

    void HandleAttack()
    {
        if (_inputFrozen) return;

        if (_attackCooldown > 0f) return;

        // Don't let the player attack while climbing.
        if (_climbing) return;

        // Don't let the player attack while crouching.
        if (_crouching) return;

        _attackCooldown = attackSpeed;

        weaponController.Attack();
        FindObjectOfType<SoundManager>().PlaySword();
#if DEBUG
        Debug.Log("ATTACK!!");
#endif
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((int) Math.Pow(2, other.gameObject.layer) == climbingLayer.value)
        {
            _onRope = true;
            _ropeX = other.transform.position.x;
#if DEBUG
            Debug.Log("On climbing surface.");
#endif
        }

        if (other.CompareTag("LostArmor") && !_chestPiece)
        {
            LostChestPiece();
            other.GetComponent<PlaceOnAlter>().Place();
        }

        if (other.CompareTag("LostLeggings") && !_leggings)
        {
            LostLeggings();
            other.GetComponent<PlaceOnAlter>().Place();
        }

        if (other.CompareTag("LostGauntlets") && !_gauntlets)
        {
            LostGauntlets();
            other.GetComponent<PlaceOnAlter>().Place();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if ((int) Math.Pow(2, other.gameObject.layer) == climbingLayer.value)
        {
            _onRope = false;
            _climbing = false;
#if DEBUG
            Debug.Log("Off of climbing surface.");
#endif
        }
    }

    public float GetVerticalMovement()
    {
        return _verticalMovement;
    }

    // Start method to revoke abilities in the beginning of the game.
    void PutOnArmour()
    {
        // Assign the fully-armoured animations.
        animator.runtimeAnimatorController = a_Armored;

        // Movement debilitation.
        moveSpeed = STARTING_MOVE_SPEED;

        // Chest piece jump restriction.
        jumpForce = PRE_CHESTPIECE_SPEED;
    }

    void LostChestPiece()
    {
        FindObjectOfType<SoundManager>().PlayMetal();
        _lateralMovement = 0f;
        GameObject go = GameObject.Find("ChestPlateTrigger");
        DialogueTrigger trigger = (DialogueTrigger) go.GetComponent(typeof(DialogueTrigger));
        trigger.TriggerDialogue();

        animator.runtimeAnimatorController = a_ChestPiece;
        _chestPiece = true;
        jumpForce = POST_CHESTPPIECE_SPEED;
        moveSpeed += 1f;


#if DEBUG
        Debug.Log("Lost Chest Piece");
#endif
    }

    void LostGauntlets()
    {
        FindObjectOfType<SoundManager>().PlayMetal();
        _lateralMovement = 0f;
        GameObject go2 = GameObject.Find("GauntletsTrigger");
        DialogueTrigger trigger2 = (DialogueTrigger) go2.GetComponent(typeof(DialogueTrigger));
        trigger2.TriggerDialogue();

        animator.runtimeAnimatorController = a_Gauntlets;
        _gauntlets = true;
        moveSpeed += 1f;
#if DEBUG
        Debug.Log("Lost Gauntlets");
#endif
    }

    void LostLeggings()
    {
        FindObjectOfType<SoundManager>().PlayMetal();
        _lateralMovement = 0f;
        GameObject go = GameObject.Find("LeggingsTrigger");
        DialogueTrigger trigger = (DialogueTrigger) go.GetComponent(typeof(DialogueTrigger));
        trigger.TriggerDialogue();

        animator.runtimeAnimatorController = a_Unarmoured;
        _leggings = true;
        moveSpeed += 1f;
#if DEBUG
        Debug.Log("Lost Leggings");
#endif
    }

    void playerHealth_OnDeath()
    {
        Vector3 position = Checkpoint.GetActiveCheckpoint().transform.position;
        this.transform.position = new Vector3(position.x, position.y + RESPAWN_POINT_OFFSET, position.z);
        Health playerHealth = GetComponent<Health>();
        playerHealth.FullyHeal();
#if DEBUG
        Debug.Log("YOU DIED.");
#endif
    }

    // MUST be called by someone outside of the player controller, 
    // ideally UI or a dedicated script
    public void FreezeInput(bool freeze)
    {
		_lateralMovement = 0;
        _inputFrozen = freeze;
        InputFreeze?.Invoke(freeze);
    }

#if DEBUG
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, GROUND_CHECK_RADIUS);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(ceilingCheck.position, CEILING_CHECK_RADIUS);
    }
#endif
}