using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckFlyingEnemyTriggers : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "AggroTriggerFE")
        {
            collision.gameObject.GetComponentInParent<FlyingEnemy>().isAggroed = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "ReturnTriggerFE")
        {
            collision.gameObject.GetComponentInParent<FlyingEnemy>().isAggroed = false;
        }
    }
}
