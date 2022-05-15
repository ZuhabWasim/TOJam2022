using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ---------------------
// Cameron Hadfield
// TOJam 2022
// Checkpoint.cs
// Describes the behaviour for any and all Checkpoints
// ---------------------
public class Checkpoint : MonoBehaviour, IDamageable {
    public static Checkpoint ActiveCheckpoint = null; // Want some kind of default ActiveCheckpoint, but how do we define which gets to be the default?
    public static GameObject GetActiveCheckpoint() {
        return ActiveCheckpoint.gameObject;
    }
    // ----------- STATIC EVENTS ------------
    public delegate void OnActivateCheckpoint();
    public static event OnActivateCheckpoint ActivateCheckpoint;
    // --------------------------------------

    public bool activated = false;

    public void BeDamaged(float _){
        ActiveCheckpoint = this;
        ActivateCheckpoint?.Invoke(); 

        activated = true;
        Debug.Log("Activated checkpoint!");
    }

}