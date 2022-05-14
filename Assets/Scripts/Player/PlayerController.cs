using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private const string HORIZONTAL_AXIS = "Horizontal";
    private const string VERTICAL_AXIS = "Vertical";
        
    public float moveSpeed;
    public float jumpForce;

    public Transform ceilingCheck;
    public Transform groundCheck;
    public LayerMask groundObjects;
    
    public float checkRadius;
    public int maxJumpCount;
    public GameObject sprite;

    private Rigidbody2D _rigidbody;
    private bool _facingRight = true;
    private float _lateralMovement;
    private bool _isCrouching;
    private bool _isClimbing;
    private bool _isJumping = false;
    private int _jumpCount;
    private bool _isGrounded;
    private Vector2 _colliderSize;
    private Vector3 _spriteScale;
    

    // Done after initialization of all objects.
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
        EventManager.Sub( InputManager.GetKeyDownEventName( KeyBinds.JUMP_KEY ), HandleJump );
        EventManager.Sub( InputManager.GetKeyDownEventName( KeyBinds.ATTACK_KEY ), HandleAttack );
        EventManager.Sub( InputManager.GetKeyDownEventName( KeyBinds.INTERACT_KEY ), HandleInteract );
        EventManager.Sub( InputManager.GetKeyDownEventName( KeyBinds.MENU_KEY ), HandleMenu );
    }
    
    // Update is called once per frame
    void Update()
    {
        GetPlayerInput();

        AnimatePlayer();

        Debug.Log("_isJumping" + _isJumping + ",    " +
                  "_isCrouching" + _isCrouching + ",    " +
                  "_isClimbing" +  _isClimbing + ",    " +
                  "_isGrounded" + _isGrounded + ",    ");
        
        // Only be able to jump again if we come back down.
        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundObjects);
        MovePlayer();
    }
    
    void GetPlayerInput()
    {
        _lateralMovement = Input.GetAxis(HORIZONTAL_AXIS);

        if (!_isGrounded)
        {
            _isCrouching = false;
            _isClimbing = false;
            return;
        }
        Debug.Log(Input.GetKey(KeyCode.UpArrow));
        _isCrouching = Input.GetKey(KeyCode.DownArrow);
        _isClimbing = Input.GetKey(KeyCode.UpArrow);

        if (_isCrouching && _isClimbing)
        {
            _isCrouching = false;
        }

    }

    void AnimatePlayer()
    {
        if (_lateralMovement > 0 && !_facingRight)
        {
            FlipCharacter();
        } else if (_lateralMovement < 0 && _facingRight)
        {
            FlipCharacter();
        }

        SpriteRenderer playerSprite = sprite.GetComponent<SpriteRenderer>();

        // Changing the player sprite.
        if (!_isGrounded)
        {
            this.transform.localScale = Vector3.one;
            playerSprite.color = new Color(0f, 0f, 1f);
            CrouchCharacter(false);
            return;
        }
        
        if (_isCrouching)
        {
            playerSprite.color = new Color(0f, 1f, 0f);
            CrouchCharacter(true);
            return;
        }
        
        if (_isClimbing)
        {
            this.transform.localScale = Vector3.one;
            playerSprite.color = new Color(1f, 0f, 0f);
            return;
        }
        
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
    }

    void FlipCharacter()
    {
        _facingRight = !_facingRight;
        transform.Rotate(0f, 180f, 0f);
    }

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
            Debug.Log("JUMP!!");
        }
    }

    void HandleClimbDown()
    {
        _isClimbing = true;
    }
    
    void HandleClimbUp()
    {
        _isClimbing = false;
    }
    
    void HandleCrouchDown()
    {
        _isCrouching = true;
    }
    
    void HandleCrouchUp()
    {
        _isCrouching = false;
    }
    
    void HandleAttack()
    {
        Debug.Log("ATTACK!!");
    }
    
    void HandleInteract()
    {
        Debug.Log("INTERACT!!");
    }
    
    void HandleMenu()
    {
        Debug.Log("Menu? Thinking emoji");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
    }
}
