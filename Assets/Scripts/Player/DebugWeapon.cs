#define HITTEST
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugWeapon : Weapon
{
    [SerializeField] GameObject target;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    { 
        #if HITTEST
            if ( Input.GetMouseButtonDown(0)) {
                this.Attack(target);
            }
        #endif
    }
}
