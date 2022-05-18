using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckFlyingEnemyTriggers : MonoBehaviour
{
    //[SerializeField] float aggroCooldown;

    //bool isAggroCD = false;


    //private void OnTriggerStay2D(Collider2D collision)
    //{
        //if (collision.tag == "AggroTriggerFE" && !isAggroCD)
        //{
            ////collision.gameObject.GetComponentInParent<FlyingEnemy>().isAggroed = true;
        //}
    //}

    //private void OnTriggerExit2D(Collider2D collision)
    //{
        //if (collision.tag == "ReturnTriggerFE" && collision.gameObject.GetComponentInParent<FlyingEnemy>().isAggroed)
        //{
            //isAggroCD = true;
            //collision.gameObject.GetComponentInParent<FlyingEnemy>().isAggroed = false;
            //StartCoroutine(CountdownAggroCD(aggroCooldown));
        //}
    //}

    //private IEnumerator CountdownAggroCD(float seconds)
    //{
        //while (true)
        //{
            //yield return new WaitForSeconds(seconds);
            //isAggroCD = false;
            //Debug.LogError("done cd");
            //yield break;
        //}
    //}
}
