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

    public delegate void OnActivateCheckpoint();
    public event OnActivateCheckpoint ActivateCheckpoint;
    public void BeDamaged(float _){
        ActiveCheckpoint = this;
        ActivateCheckpoint?.Invoke(); 
    }

}