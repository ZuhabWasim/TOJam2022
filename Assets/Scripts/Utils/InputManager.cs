using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * EventManager heavily based on Lakshya's design from Reflection.
 * 
 * RegisteredKeyBind struct that holds the information of any particular bind.
 */
struct RegisteredKeybind
{
    public readonly KeyCode key;
    bool isPressed;
    public readonly string keydownEvent;
    public readonly string keyupEvent;

    public bool IsPressed()
    {
        return isPressed;
    }

    public void SetPressed(bool pressed)
    {
        isPressed = pressed;
    }

    public RegisteredKeybind(KeyCode key)
    {
        this.key = key;
        isPressed = false;
        keydownEvent = this.key.ToString() + "Down";
        keyupEvent = this.key.ToString() + "Up";
    }
}

/*
 * Handles all key bind presses and fires an event for every key down, or key up event.  
 */
public class InputManager : MonoBehaviour
{
    private static Hashtable registeredKeybinds = new Hashtable();

    void Awake()
    {
        RegisterKeybinds();
    }

    void RegisterKeybinds()
    {
        KeyBinds[] keyBinds = (KeyBinds[]) System.Enum.GetValues(typeof(KeyBinds));
        foreach (KeyBinds key in keyBinds)
        {
            if (registeredKeybinds.Contains(key)) continue;
            
            RegisteredKeybind k = new RegisteredKeybind((KeyCode) key);
            registeredKeybinds.Add(key, k);
        }
    }

    public static string GetKeyDownEventName(KeyBinds key)
    {
        return ((RegisteredKeybind) registeredKeybinds[key]).keydownEvent;
    }

    public static string GetKeyUpEventName(KeyBinds key)
    {
        return ((RegisteredKeybind) registeredKeybinds[key]).keyupEvent;
    }

    void Update()
    {
        foreach (DictionaryEntry entry in registeredKeybinds)
        {
            RegisteredKeybind keybind = (RegisteredKeybind) entry.Value;
            if (Input.GetKeyDown(keybind.key) && !keybind.IsPressed())
            {
                keybind.SetPressed(true);
                EventManager.Fire(keybind.keydownEvent);
            }
            else if (Input.GetKeyUp(keybind.key))
            {
                keybind.SetPressed(false);
                EventManager.Fire(keybind.keyupEvent);
            }
        }
    }
}

/*
 * 	H_AXIS = "Horizontal";
	V_AXIS = "Vertical";
	MOUSE_X = "Mouse X";
	MOUSE_Y = "Mouse Y";
 */
// All key binds used for the game.
public enum KeyBinds
{
    JUMP_KEY = KeyCode.Space,
    ATTACK_KEY = KeyCode.J,
    INTERACT_KEY = KeyCode.C,
    MENU_KEY = KeyCode.Escape,

    UP_KEY = KeyCode.UpArrow,
    DOWN_KEY = KeyCode.DownArrow,
    CROWCH_DASH_KEY = KeyCode.LeftShift,
    // Armour piece keys for debugging.
    CHEST_PIECE = KeyCode.Alpha1,
    GAUNTLET = KeyCode.Alpha3,
    LEGGINGS = KeyCode.Alpha2,
    SWORD = KeyCode.Alpha4,
    
    // This is for debug only
    DEBUG_TRIGGER = KeyCode.Alpha0
}