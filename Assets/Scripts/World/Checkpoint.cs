using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ---------------------
// Cameron Hadfield
// TOJam 2022
// Checkpoint.cs
// Describes the behaviour for any and all Checkpoints
// ---------------------
public class Checkpoint : MonoBehaviour, IDamageable
{
    public static Checkpoint
        ActiveCheckpoint =
            null; // Want some kind of default ActiveCheckpoint, but how do we define which gets to be the default?

    public static GameObject GetActiveCheckpoint()
    {
        return ActiveCheckpoint.gameObject;
    }

    // ----------- STATIC EVENTS ------------
    public delegate void OnActivateCheckpoint();

    public static event OnActivateCheckpoint ActivateCheckpoint;
    // --------------------------------------

    public bool activated = false;

    public void BeDamaged(float _)
    {
        Activate();
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag(PlayerController.PLAYER))
        {
            Activate();
			
        }
    }

    private void Activate()
    {
        ActiveCheckpoint = this;
        ActivateCheckpoint?.Invoke();
		if (activated == false){
			FindObjectOfType<SoundManager>().PlayFire();
			if (FindObjectOfType<PlayerController>()._leggings == true){
				GameObject go = GameObject.Find("FinalBossStartTrigger");
				DialogueTrigger trigger = (DialogueTrigger) go.GetComponent(typeof(DialogueTrigger));
				trigger.TriggerDialogue();
			}
		}
        activated = true;
        Debug.Log("Activated checkpoint!");
    }
}