using UnityEngine;

[RequireComponent(typeof (Collider2D))]
public class AggroTrigger : MonoBehaviour {
    public delegate void AggroChange(bool aggro);
    public event AggroChange Aggro;
    private Collider2D _triggerRadius;
    private void Start() {
        _triggerRadius = GetComponent<Collider2D>();
        if(_triggerRadius == null) {
            Debug.LogError("Please assign this GameObject a Collider2D");
            return;
        }
        _triggerRadius.isTrigger=true; // make sure it is a trigger

    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")) Aggro?.Invoke(true);
    }
    private void OnTriggerExit2D(Collider2D other) {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")) Aggro?.Invoke(false);
    }
}