using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * EventManager heavily based on Lakshya's design from Reflection.
 * 
 * Event Manager that you can use to fire and subscribe to events. Used as an abstraction layer to avoid interacting
 * with components and GameObjects directly.
 *
 * Usage:
 *  When something happens in a script, fire a particular event (of string name)
 *      EventManager.Fire("Player health 0")
 *  Optional, pass in the particular GameObject that fired the event or any component you want.
 *      EventManager.Fire("Player health 0", this.Enemy)
 *  Subscribe to the particular event wherever you need to see and the method it calls when it sees the event triggered.
 *      EventManager.Sub("Player health 0", EndGame)
 */
public class EventManager : MonoBehaviour
{
    private static Hashtable eventListeners = new Hashtable();
    private static Hashtable eventlistenersArgs = new Hashtable();

    // Fires triggerEvent specifying the given GameObject that's requested if there is one.
    public static void Fire(string triggerEvent, GameObject gameObject = null)
    {
        // Triggers all callbacks that are subscribed to the event without arguments.
        if (eventListeners.ContainsKey(triggerEvent))
        {
            List<System.Action> callbacks = ((List<System.Action>) eventListeners[triggerEvent]);

            foreach (System.Action callback in callbacks)
            {
                try
                {
                    callback();
                    Debug.Log(triggerEvent);
                }
                catch (System.Exception)
                {
                    continue;
                }
            }
        }

        // Triggers all callbacks that are subscribed to the event with arguments.
        if (eventlistenersArgs.ContainsKey(triggerEvent))
        {
            List<System.Action<GameObject>> callbacksArgs =
                ((List<System.Action<GameObject>>) eventlistenersArgs[triggerEvent]);
            foreach (System.Action<GameObject> callback in callbacksArgs)
            {
                try
                {
                    callback(gameObject);
                }
                catch (System.Exception)
                {
                    continue;
                }
            }
        }
    }

    // Subscribes to a particular event.
    public static void Sub(string triggerEvent, System.Action callback)
    {
        // Instantiate a list of callbacks for this event if possible.
        if (!eventListeners.ContainsKey(triggerEvent))
        {
            eventListeners.Add(triggerEvent, new List<System.Action>());
        }

        // Add this listener to the list if not already.
        List<System.Action> callbacks = ((List<System.Action>) eventListeners[triggerEvent]);
        if (!callbacks.Contains(callback))
        {
            callbacks.Add(callback);
        }
    }

    // Subs to a particular event with args.
    public static void Sub(string triggerEvent, System.Action<GameObject> callback)
    {
        // Instantiate a list of callbacks for this event if possible.
        if (!eventlistenersArgs.ContainsKey(triggerEvent))
        {
            eventlistenersArgs.Add(triggerEvent, new List<System.Action<GameObject>>());
        }

        // Add this listener to the list if not already.
        List<System.Action<GameObject>>
            callbacks = ((List<System.Action<GameObject>>) eventlistenersArgs[triggerEvent]);
        if (!callbacks.Contains(callback))
        {
            callbacks.Add(callback);
        }
    }

    // Unsubscribes a callback from its event.
    public static void Unsub(string triggerEvent, System.Action callback)
    {
        if (!eventListeners.ContainsKey(triggerEvent))
        {
            return;
        }

        ((List<System.Action>) eventListeners[triggerEvent]).Remove(callback);
    }

    // Unsubscribes a callback from its event with specifying a GameObject.
    public static void Unsub(string triggerEvent, System.Action<GameObject> callback)
    {
        if (!eventlistenersArgs.ContainsKey(triggerEvent))
        {
            return;
        }

        ((List<System.Action<GameObject>>) eventListeners[triggerEvent]).Remove(callback);
    }
}