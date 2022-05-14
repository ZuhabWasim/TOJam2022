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
    private const float DASH_TIME = 1f;
    // TODO: Max movement 
    
    [Header("Movement")] public float moveSpeed;

    public float jumpForce;
    public float crouchDashForce;
    //public int maxJumpCount;

    [Header("Collision")] public Transform ceilingCheck;
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
    private float _crouchDashCooldown;
    
    /*private int _jumpCount;*/

    // Movement states.
    private bool _isCrouching;
    private bool _isClimbing;
    private bool _isJumping = false;
    private bool _isGrounded;
    
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

    // Update is called once per frame
    void Update()
    {
        GetPlayerInput();

        AnimatePlayer();

        // Only be able to jump again if we come back down. Put this back in Update because it works better.
        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundObjects);
        MovePlayer();

        // Debug.Log("_isJumping" + _isJumping + ",    " +
        //           "_isCrouching" + _isCrouching + ",    " +
        //           "_isClimbing" +  _isClimbing + ",    " +
        //           "_isGrounded" + _isGrounded + ",    ");
    }

    void GetPlayerInput()
    {
        _lateralMovement = Input.GetAxis(HORIZONTAL_AXIS); // Left/Right movement.
        float verticalMovement = Input.GetAxisRaw(VERTICAL_AXIS);
        _isClimbing = verticalMovement > 0f;
        _isCrouching = verticalMovement < 0f;

        // Prioritize climbing in the air.
        if (_isClimbing)
        {
            _isCrouching = false;
            return;
        }

        // Player is in the air elsewise.
        if (!_isGrounded)
        {
            _isCrouching = false;
            _isClimbing = false;
            return;
        }

        // Prioritize climbing over crouching if both are pressed.
        if (_isCrouching && _isClimbing)
        {
            _isCrouching = false;
        }

        // Otherwise we see if the player was crouching.
        if (_isCrouching)
        {
            _lateralMovement = 0f;
        }
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
        if (_isClimbing)
        {
            this.transform.localScale = Vector3.one;
            playerSprite.color = new Color(1f, 0f, 0f);
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
        if (_isCrouching)
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

    void MovePlayer()
    {
        _rigidbody.velocity = new Vector2(_lateralMovement * moveSpeed, _rigidbody.velocity.y);
        if (_isJumping)
        {
            _rigidbody.AddForce(new Vector2(0f, jumpForce));
        }

        _isJumping = false;
        Debug.Log(_rigidbody.velocity);
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

    void HandleJump()
    {
        if (_isGrounded)
        {
            _isJumping = true;
            _isCrouching = false;
            _isClimbing = false;
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