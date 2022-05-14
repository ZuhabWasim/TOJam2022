using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ---------------------
// Cameron Hadfield
// TOJam 2022
// Weapon.cs
// This class is used as the main method of interaction with the health system.
// The simplest form of "use to give damage"
// Realistically this is just a base class
// ---------------------
public class Weapon : MonoBehaviour
{
    public delegate void OnHit(GameObject other);
    public event OnHit Hit;
    // --------DAMAGE----------
    [SerializeField] private float _damage = 1;
    public float damage{
        get{return _damage;}
        set{_damage=value;} // not really necessary to modify a weapon's damage but hey, might as well chuck it in there
    }

    protected void Attack(GameObject other){
        IDamageable damageable = other.GetComponent<IDamageable>();
        if(damageable != null){
            damageable.BeDamaged(damage); // Damage the object
        }
        Hit?.Invoke(other);
        // TODO
        // Maybe we want separate events for if the object was damageable or not? 
        // since it is purely the animation perspective then maybe we can just do that from the individual IDamageable components
    }
}