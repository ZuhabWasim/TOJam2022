using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchDamage : MonoBehaviour
{
    public bool Active = true;
    
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Active && collision.transform.CompareTag(PlayerController.PLAYER))
        {
            gameObject.GetComponent<WeaponController>().Attack();
        }
    }
}
