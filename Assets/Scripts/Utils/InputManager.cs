#define DEBUG

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

// All key binds used for the game.
public enum KeyBinds
{
    JUMP_KEY = KeyCode.Space,
    ATTACK_KEY = KeyCode.J,

    UP_KEY1 = KeyCode.UpArrow,
    UP_KEY2 = KeyCode.W,
    
#if DEBUG
    // Debug keys
    DEBUG1 = KeyCode.Alpha1,
    DEBUG2 = KeyCode.Alpha2,
    DEBUG3 = KeyCode.Alpha3,
    DEBUG4 = KeyCode.Alpha4,
    DEBUG5 = KeyCode.Alpha5,
    DEBUG6 = KeyCode.Alpha6,
    DEBUG7 = KeyCode.Alpha7,
    DEBUG8 = KeyCode.Alpha8,
    DEBUG9 = KeyCode.Alpha9,
    DEBUG0 = KeyCode.Alpha0
#endif
}