using UnityEngine;
public class DebugDamageable : MonoBehaviour, IDamageable{
    public void BeDamaged(float dmg){
        Debug.Log("Ouch!!");
    }
}