using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dagger : Weapon
{
    [SerializeField] float critChance = 0.5f;
    public override void Attack(GameObject other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        float rand = Random.value;
        if(damageable != null){
            // Deal double damage based on the critChance variable
            damageable.BeDamaged(rand <= critChance? damage * 2 : damage); // Damage the object
        }
        RaiseHit(other);
    }
}
