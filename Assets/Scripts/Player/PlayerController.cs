using System;
using System.Collections;
using System.Collections.Generic;
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

    private Rigidbody2D _rigidbody;
    private bool _facingRight = true;
    private float _moveDirection;
    private bool _isJumping = false;
    private int _jumpCount;

    // Done after initialization of all objects.
    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }
    // Start is called before the first frame update
    void Start()
    {
        RegisterEventListeners();
    }
    
    void RegisterEventListeners()
    {
        EventManager.Sub( InputManager.GetKeyDownEventName( KeyBinds.JUMP_KEY ), HandleJumpDown );
        // EventManager.Sub( InputManager.GetKeyUpEventName( KeyBinds.JUMP_KEY ), HandleJumpUp );
        EventManager.Sub( InputManager.GetKeyDownEventName( KeyBinds.ATTACK_KEY ), HandleAttack );
        EventManager.Sub( InputManager.GetKeyDownEventName( KeyBinds.INTERACT_KEY ), HandleInteract );
        EventManager.Sub( InputManager.GetKeyDownEventName( KeyBinds.MENU_KEY ), HandleMenu );
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    // Update is called once per frame
    void Update()
    {
        GetLateralInput();

        AnimatePlayer();
    }
    
    void GetLateralInput()
    {
        _moveDirection = Input.GetAxis(HORIZONTAL_AXIS);
        if (Input.GetButtonDown("Jump"))
        {
            _isJumping = true;
        }
        
    }

    void AnimatePlayer()
    {
        if (_moveDirection > 0 && !_facingRight)
        {
            FlipCharacter();
        } else if (_moveDirection < 0 && _facingRight)
        {
            FlipCharacter();
        }
    }

    void MovePlayer()
    {
        _rigidbody.velocity = new Vector2(_moveDirection * moveSpeed, _rigidbody.velocity.y);
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
    
    void HandleJumpDown()
    {
        //_isJumping = true;
        Debug.Log("JUMP!!");
    }
    
    /*void HandleJumpUp()
    {
        _isJumping = false;
        //Debug.Log("JUMP!!");
    }*/

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
}
