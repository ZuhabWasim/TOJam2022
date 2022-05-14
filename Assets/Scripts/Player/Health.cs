// PREPROC
#define DEBUG
#define DMGTEST
#define DEATHTEST

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ---------------------
// Cameron Hadfield
// TOJam 2022
// Health.cs
// This class tracks the health of anything, providing a listenable death trigger when that health reaches zero
// Also exposes damage functions
// ---------------------
public class Health : MonoBehaviour, IDamageable
{
    // -----EVENTS-----------------------------
    public delegate void OnHealthChange(float oldHealth, float newHealth);
    public event OnHealthChange HealthChanged; 

    public delegate void OnDeath();
    public event OnDeath Death;

    // ------HEALTH RELATED--------------------
    [SerializeField] private float _health;
    public float health {
        get{
            return _health;
        } 
        private set{
            float oldHealth = _health;
            _health = value;

            HealthChanged?.Invoke(oldHealth, _health);
            if(_health <= 0){
                Death?.Invoke();
            }
        }
    }

    // -------- DEATH :( ------------------------
    [SerializeField] public float maxHealth = 10; // Provide default for rapid dev

    public void BeDamaged(float dmg){
        health -= dmg;
        // TODO
        // This is where we would put checks based on resistances...
        // Want to restrict damage to while you're alive?
    }

    void Start(){
        health = maxHealth;

        #if DMGTEST
        HealthChanged+=(float oldHealth, float newHealth)=>{Debug.Log(oldHealth + " -> " + newHealth);};
        #endif
        #if DEATHTEST
        Death+=()=>{Debug.Log("Character Death");};
        #endif
    }

    void Update(){
        #if DMGTEST
        if(Input.GetMouseButtonDown(0)){
            BeDamaged(2);
        }
        else if(Input.GetMouseButtonDown(1)){
            BeDamaged(-2);
        }
        #endif
    }

}
