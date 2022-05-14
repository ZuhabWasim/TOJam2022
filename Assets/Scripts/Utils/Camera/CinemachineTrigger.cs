using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ---------------------
// Cameron Hadfield
// TOJam 2022
// CinemachineTrigger.cs
// This class gives an easy-to-use trigger for swapping to static cameras from the default following cinemachine camera
// ---------------------
[RequireComponent(typeof (Collider2D))]
public class CinemachineTrigger : MonoBehaviour
{
    [SerializeField] bool playerInCollision;
    Collider2D _myCollider; // Guaranteed to have some collider
    void Start()
    {
        _myCollider = GetComponent<Collider2D>();
        if(!_myCollider.isTrigger) Debug.LogWarningFormat("Cinemachine Trigger %s may not be properly set up! (Collider not trigger)", gameObject.name);
    }

    // This might give unexpected behaviours with respawning at checkpoints
    private void OnCollisionEnter2D(Collision2D other) {
        playerInCollision = true;
    }
    private void OnCollisionExit2D(Collision2D other) {
        playerInCollision = false;
    }
    // --------------------------------------------------------------------
}
