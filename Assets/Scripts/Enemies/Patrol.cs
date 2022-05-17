using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : MonoBehaviour
{
    public float speed;
    public float distanceForRay;
    // Can start some enemies moving left by editing the console value
    public bool movingRight = true;

    public Transform groundDetection;
    public LayerMask groundMask;

    private int _directionModifier = 1;
    private SpriteRenderer _spriteRenderer;
    private Health _health;
    private WeaponController _weaponController;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(groundDetection.position, Vector2.down * distanceForRay);
    }

    void Start()
    {
        _spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
        _health = gameObject.GetComponent<Health>();
        if (_health) _health.Death += OnDeath;
        _weaponController = gameObject.GetComponent<WeaponController>();
        if(!_weaponController) Debug.LogWarning("No Weapon Controller on, this enemy will not damage the player: " + name);
    }

   

    void Update()
    {
        _spriteRenderer.flipX = movingRight;

        RaycastHit2D groundinfo = Physics2D.Raycast(groundDetection.position, Vector2.down, distanceForRay, groundMask);
        if (groundinfo.collider)
        {
            transform.Translate(Vector2.right * _directionModifier * speed * Time.deltaTime);
        } else {
            reverseMovement();
        }
    }

    private void OnDeath()
    {
		FindObjectOfType<SoundManager>().PlaySnake();
        gameObject.SetActive(false);
		
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") || collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            reverseMovement();
        }
        _weaponController.Attack(); // Weapon controller will determine if anything can be hit
		
    }

    void reverseMovement()
    {
        // First, rotate the ground detection.
        groundDetection.RotateAround(GetComponentInParent<Transform>().position, Vector3.up, 180);

        // Second, reverse the boolean and the direction modifier.
        movingRight = !movingRight;
        _directionModifier *= -1;
    }

}
