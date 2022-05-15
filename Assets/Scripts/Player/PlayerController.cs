#define DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    private const string HORIZONTAL_AXIS = "Horizontal";
    private const string VERTICAL_AXIS = "Vertical";
    private const string PLAYER_TAG = "Player";
    private const float GRAVITY_SCALE = 5f;
    private const float ROPE_SNAP_OFFSET = 0.5f;

    [Header("Movement")] public float moveSpeed;
    public float jumpForce;
    public float terminalVelocity = 18f;
    public float crouchDashForce;

    [Header("Climbing")] public float climbingSpeed;
    public float climbCooldownDuration = 0.3f;
    public float climbGraceDuration = 0.2f;
    public LayerMask climbingLayer;

    [Header("Collision")] public Transform groundCheck;
    public LayerMask groundObjects;
    public float checkRadius;

    [Header("Combat")] 
    public WeaponController weaponController;
    [SerializeField] private float attackSpeed = 0.5f;

    [Header("Animation")] public GameObject sprite;
    public GameObject playerCenter;


    // Movement values.
    private Rigidbody2D _rigidbody;
    private float _lateralMovement;
    private float _verticalMovement;
    private float _crouchDashCooldown;
    private float _climbTimer;
    private float _climbGrace;

    // Movement states.
    private bool _onRope;
    private bool _crouchPress;
    private bool _climbPress;
    private bool _isJumping = false;
    private bool _isGrounded;

    private bool _climbing;
    private float _ropeX;
    private bool _crouching = false;

    // Sprite information.
    private bool _facingRight = true;
    private Vector2 _colliderSize;
    private Vector3 _spriteScale;

    // Armour Pieces.
    private bool _chestPiece = false;
    private bool _gauntlets = false;
    private bool _leggings = false;
    private bool _sword = false;

    // Combat related.
    private float _lastAttack = 0f;

    // Awake is called after initialization of all objects.
    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _colliderSize = GetComponent<CapsuleCollider2D>().size;
        _spriteScale = sprite.transform.localScale;

        RegisterEventListeners();
    }

    void RegisterEventListeners()
    {
        EventManager.Sub(InputManager.GetKeyDownEventName(KeyBinds.JUMP_KEY), HandleJump);
        EventManager.Sub(InputManager.GetKeyDownEventName(KeyBinds.ATTACK_KEY), HandleAttack);
        EventManager.Sub(InputManager.GetKeyDownEventName(KeyBinds.INTERACT_KEY), HandleInteract);
        EventManager.Sub(InputManager.GetKeyDownEventName(KeyBinds.MENU_KEY), HandleMenu);

        // Placeholder events for each armour piece lost.
        EventManager.Sub(InputManager.GetKeyDownEventName(KeyBinds.CHEST_PIECE), LostChestPiece);
        EventManager.Sub(InputManager.GetKeyDownEventName(KeyBinds.GAUNTLET), LostGauntlets);
        EventManager.Sub(InputManager.GetKeyDownEventName(KeyBinds.LEGGINGS), LostLeggings);
        EventManager.Sub(InputManager.GetKeyDownEventName(KeyBinds.SWORD), LostSword);
    }

    private void FixedUpdate()
    {
        // Only be able to jump again if we come back down. Put this back in Update because it works better.
        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundObjects);
        MovePlayer();
    }

    // Update is called once per frame
    void Update()
    {
        GetPlayerInput();


        AnimatePlayer();

        // Debug.Log("_isJumping" + _isJumping + ",    " +
        //           "_isCrouching" + _isCrouching + ",    " +
        //           "_isClimbing" +  _isClimbing + ",    " +
        //           "_isGrounded" + _isGrounded + ",    ");
        TickCooldowns();
    }

    void GetPlayerInput()
    {
        _lateralMovement = Input.GetAxis(HORIZONTAL_AXIS);
        _verticalMovement = Input.GetAxisRaw(VERTICAL_AXIS);
        _climbPress = _verticalMovement > 0f;
        _crouchPress = _verticalMovement < 0f;

        // Prioritize climbing in the air.
        if (_onRope && _verticalMovement != 0 && _climbTimer == 0f)
        {
            _climbing = true;
            if (_climbGrace == 0f)
            {
                _climbGrace = climbGraceDuration;
            }

            return;
        }

        if (_climbPress)
        {
            _crouchPress = false;
            return;
        }

        // Player is in the air elsewise.
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

        // Otherwise if the player isn't crouching, then get the player input.
        /*if (!_isCrouching)
        {
            _lateralMovement = Input.GetAxis(HORIZONTAL_AXIS); // Left/Right movement.
            //_lateralMovement = 0f;
        }*/
    }

    void AnimatePlayer()
    {
        // Changing direction.
        if (_lateralMovement > 0 && !_facingRight)
        {
            FlipCharacter();
        }
        else if (_lateralMovement < 0 && _facingRight)
        {
            FlipCharacter();
        }

        // TODO: Add animations/sprites.
        SpriteRenderer playerSprite = sprite.GetComponent<SpriteRenderer>();

        if (_climbing)
        {
            this.transform.localScale = Vector3.one;

            if (_isGrounded && _climbGrace == 0f)
            {
                playerSprite.color = new Color(1f, 1f, 1f);
                _climbing = false;
            }
            else
            {
                playerSprite.color = new Color(1f, 0f, 0f);
            }

            CrouchCharacter(false);
            return;
        }

        // Mid-air sprite.
        if (!_isGrounded)
        {
            this.transform.localScale = Vector3.one;
            playerSprite.color = new Color(0f, 0f, 1f);
            CrouchCharacter(false);
            return;
        }

        // Crouching sprite.
        if (_crouchPress)
        {
            playerSprite.color = new Color(0f, 1f, 0f);
            CrouchCharacter(true);
            return;
        }

        // Idle sprite.
        this.transform.localScale = Vector3.one;
        playerSprite.color = new Color(1f, 1f, 1f);
        CrouchCharacter(false);
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
            collider.size = new Vector2(_colliderSize.x, _colliderSize.y / 2);
            sprite.transform.localScale = new Vector3(_spriteScale.x, _spriteScale.y / 2, _spriteScale.z);
            _crouching = true;
            Vector3 position = _rigidbody.position;
            _rigidbody.position = new Vector3(position.x, position.y - _spriteScale.y / 4, position.z);
        }
        else if (!crouch && _crouching)
        {
            collider.size = new Vector2(_colliderSize.x, _colliderSize.y);
            sprite.transform.localScale = new Vector3(_spriteScale.x, _spriteScale.y, _spriteScale.z);
            _crouching = false;
            Vector3 position = _rigidbody.position;
            _rigidbody.position = new Vector3(position.x, position.y + _spriteScale.y / 2, position.z);
        }
    }

    void MovePlayer()
    {
        // Disable gravity to only rely on up/down input when the player is climbing.
        if (_climbing)
        {
            _rigidbody.gravityScale = 0f;
            // Snap the player to the ladder block
            SnapToRope();
            _rigidbody.velocity = new Vector2(_lateralMovement * moveSpeed, _verticalMovement * climbingSpeed);
        }
        else
        {
            _rigidbody.gravityScale = GRAVITY_SCALE;
            _rigidbody.velocity = new Vector2(_lateralMovement * moveSpeed,
                Mathf.Clamp(_rigidbody.velocity.y, -terminalVelocity, terminalVelocity));
        }

        // If the player scheduled a jump, trigger it once and set it to false.
        if (_isJumping)
        {
            // Re-enable regular movement
            _rigidbody.gravityScale = GRAVITY_SCALE;
            _rigidbody.velocity = new Vector2(_lateralMovement * moveSpeed,
                Mathf.Clamp(_rigidbody.velocity.y, -terminalVelocity, terminalVelocity));

            // Add the jump velocity.
            _rigidbody.AddForce(new Vector2(0f, jumpForce));

            // Set climbing on cooldown for a bit.
            _climbing = false;
            _climbTimer = climbCooldownDuration;
        }

        _isJumping = false;
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
    }

    void HandleJump()
    {
        if (_isGrounded || _climbing)
        {
            _isJumping = true;
            _crouchPress = false;
            _climbPress = false;
#if DEBUG
            Debug.Log("JUMP!!");
#endif
        }
    }

    public void FreezeInput(){

    }

    void HandleAttack()
    {
        float timeSinceLastAttack = Time.time - _lastAttack;
        if(timeSinceLastAttack < attackSpeed) return;
        _lastAttack = Time.time;

        if (_climbing)
        {
            return;
        }
        weaponController.Attack();
        // ------Attack Visualization--------
        // ----------------------------------

        //Color attackColor = attackPoint.GetComponentInChildren<SpriteRenderer>().color;
        //attackPoint.GetComponentInChildren<SpriteRenderer>().color =
        //    new Color(attackColor.g, attackColor.b, attackColor.r);

#if DEBUG
            //Debug.Log(" Hit enemy:  " + enemy.name);
#endif
        //}
#if DEBUG
        Debug.Log("ATTACK!!");
#endif
    }

    void HandleInteract()
    {
        Debug.Log("INTERACT!!");
    }

    void HandleMenu()
    {
        Debug.Log("Menu? Thinking emoji");
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

    public bool isCrouching()
    {
        return _crouching;
    }

#if DEBUG
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        Gizmos.color = Color.cyan;
        //Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
#endif

    void LostChestPiece()
    {
        _chestPiece = true;
#if DEBUG
        Debug.Log("Lost Chest Piece");
#endif
    }

    void LostGauntlets()
    {
        _gauntlets = true;
#if DEBUG
        Debug.Log("Lost Gauntlets");
#endif
    }

    void LostLeggings()
    {
        _leggings = true;
#if DEBUG
        Debug.Log("Lost Leggings");
#endif
    }

    void LostSword()
    {
        _sword = true;
#if DEBUG
        Debug.Log("Lost Sword");
#endif
    }
}