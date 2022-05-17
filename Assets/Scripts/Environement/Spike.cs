using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spike : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.transform.CompareTag(PlayerController.PLAYER))
        {
            Health playerHealth = GameObject.FindGameObjectWithTag(PlayerController.PLAYER).GetComponent<Health>();
            playerHealth.BeDamaged(playerHealth.health);
        }
    }
}
