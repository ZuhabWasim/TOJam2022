using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchDamage : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.transform.CompareTag(PlayerController.PLAYER))
        {
            gameObject.GetComponent<WeaponController>().Attack();
        }
    }
}
