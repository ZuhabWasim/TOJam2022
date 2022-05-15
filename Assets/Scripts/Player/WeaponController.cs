using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WeaponController : MonoBehaviour
{
    [SerializeField] LayerMask attackFilter;
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
    }
    
    public void Attack(){
        Collider2D[] collisions = Physics2D.OverlapCircleAll(transform.position, weapon.range, attackFilter.value);
        if(collisions.Length > 0){
            collisions.Where(collider => collider.GetComponent<IDamageable>() != null)
                .Select(collider => collider.gameObject)
                .ToList()
                .ForEach(damageable => {weapon?.Attack(damageable);});
        }
    }
}