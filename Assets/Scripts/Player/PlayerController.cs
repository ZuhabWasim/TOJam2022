using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        RegisterEventListeners();
    }

    // Update is called once per frame
    void Update()
    {
        /*
         * HandleMovement()
         */
    }

    void RegisterEventListeners()
    {
        EventManager.Sub( InputManager.GetKeyDownEventName( KeyBinds.JUMP_KEY ), HandleJump );
        EventManager.Sub( InputManager.GetKeyDownEventName( KeyBinds.ATTACK_KEY ), HandleAttack );
        EventManager.Sub( InputManager.GetKeyDownEventName( KeyBinds.INTERACT_KEY ), HandleInteract );
        EventManager.Sub( InputManager.GetKeyDownEventName( KeyBinds.MENU_KEY ), HandleMenu );
    }

    void HandleJump()
    {
        Debug.Log("JUMP!!");
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
}
