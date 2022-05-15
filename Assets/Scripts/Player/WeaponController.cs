using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Collider2D))]
public class WeaponController : MonoBehaviour
{
    [SerializeField] ContactFilter2D attackFilter;
    [SerializeField] Collider2D collider;
    [SerializeField] Weapon _weapon;
    public Weapon weapon {
        get{
            return _weapon;
        }
        set{
            _weapon = weapon;
        }
    }

    // Check for a weapon on this gameobject if there is not one assigned, 
    // If we can't find one, warn that there isn't one.
    void Awake(){
        if(!weapon){
            weapon = GetComponent<Weapon>();
            if(!weapon){
                Debug.LogWarning("No weapon assigned");
            }
        }
        collider = GetComponent<Collider2D>();
    }
    
    public void Attack(){
        List<Collider2D> collisions = new List<Collider2D>();
        int numCollisions = collider.OverlapCollider(attackFilter, collisions);
        if(numCollisions > 0){
            collisions.Where(collider => collider.GetComponent<IDamageable>() != null)
                .Select(collider => collider.gameObject)
                .ToList()
                .ForEach(damageable => {weapon?.Attack(damageable);});
        }
    }
}