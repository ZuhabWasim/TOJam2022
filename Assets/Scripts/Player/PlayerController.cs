#define DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private const string HORIZONTAL_AXIS = "Horizontal";
    private const string VERTICAL_AXIS = "Vertical";
    private const string PLAYER_TAG = "Player";
    private const float GRAVITY_SCALE = 5f;

    [Header("Movement")] 
    public float moveSpeed;
    public float jumpForce;
    public float terminalVelocity = 18f;
    public float crouchDashForce;
    
    [Header("Climbing")] 
    public float climbingSpeed;
    public float climbCooldown = 0.3f;
    public LayerMask climbingLayer;

    [Header("Collision")]
    public Transform groundCheck;
    public LayerMask groundObjects;
    public float checkRadius;

    [Header("Combat")] public Transform attackPoint;
    public float attackRadius;
    public LayerMask enemyLayer;

    [Header("Animation")] public GameObject sprite;

    // Movement values.
    private Rigidbody2D _rigidbody;
    private float _lateralMovement;
    private float _verticalMovement;
    private float _crouchDashCooldown;
    private float _climbTimer;
    
    // Movement states.
    private bool _onRope;
    private bool _crouchPress;
    private bool _climbPress;
    private bool _isJumping = false;
    private bool _isGrounded;

    private bool climbing;

    // Sprite information
    private bool _facingRight = true;
    private Vector2 _colliderSize;
    private Vector3 _spriteScale;

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
            climbing = true;
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

        // Climbing sprite. TODO: Change the precedence so you dont see climbing sprite unless you're actually on a rope.
        if (climbing)
        {
            this.transform.localScale = Vector3.one;
            if (!_isGrounded)
            {
                playerSprite.color = new Color(1f, 0f, 0f);
            } else 
            {
                playerSprite.color = new Color(1f, 1f, 1f);
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

        if (crouch)
        {
            collider.size = new Vector2(_colliderSize.x, _colliderSize.y / 2);
            sprite.transform.localScale = new Vector3(_spriteScale.x, _spriteScale.y / 2, _spriteScale.z);
        }
        else
        {
            collider.size = new Vector2(_colliderSize.x, _colliderSize.y);
            sprite.transform.localScale = new Vector3(_spriteScale.x, _spriteScale.y, _spriteScale.z);
        }
    }

    void MovePlayer()
    {
        // Disable gravity to only rely on up/down input when the player is climbing.
        if (climbing)
        {
            _rigidbody.gravityScale = 0f;
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
            climbing = false;
            _climbTimer = climbCooldown;
        }
        _isJumping = false;
    }

    void TickCooldowns()
    {
        _climbTimer = Mathf.Max(0f, _climbTimer - Time.deltaTime);
    }

    void HandleJump()
    {
        if (_isGrounded || climbing)
        {
            _isJumping = true;
            _crouchPress = false;
            _climbPress = false;
#if DEBUG
            Debug.Log("JUMP!!");
#endif
        }
    }

    void HandleAttack()
    {
        // TODO: If the player was climbing, you can't attack.

        // Visualize Attack
        Color attackColor = attackPoint.GetComponentInChildren<SpriteRenderer>().color;
        attackPoint.GetComponentInChildren<SpriteRenderer>().color =
            new Color(attackColor.g, attackColor.b, attackColor.r);

        // Detect everything hit by player.
        // TODO: Change from Default layer to Enemy layer.
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, enemyLayer);

        // Damage the enemies
        foreach (Collider2D enemy in hitEnemies)
        {
            // Don't damage yourself.
            if (enemy.gameObject.tag == PLAYER_TAG)
            {
                continue;
            }

            // For now de-rendering objects hit but this is where you can damage enemies.
            // TODO: Add damaging of enemies.
            enemy.gameObject.SetActive(false);
#if DEBUG
            Debug.Log(" Hit enemy:  " + enemy.name);
#endif
        }
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
            climbing = false;
#if DEBUG
            Debug.Log("Off of climbing surface.");
#endif
        }
    }

#if DEBUG
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
#endif
}