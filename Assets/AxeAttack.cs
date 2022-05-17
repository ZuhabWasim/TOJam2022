using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeAttack : MonoBehaviour
{
    [SerializeField] float attackSpeed = 0.5f;
    private Animator _animator;
    private bool _isAggroed = false;
    private float _attackCooldown = 0f;

    private void Start()
    {
        _animator = gameObject.transform.parent.GetComponentInChildren<Animator>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            _isAggroed = true;
            _animator.SetBool("isAggroed", true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            _isAggroed = false;
            _animator.SetBool("isAggroed", false);
        }
    }

    void HandleAttack()
    {
        _attackCooldown = attackSpeed;
        gameObject.GetComponent<WeaponController>().Attack();
    }

    private void Update()
    {
        _attackCooldown = Mathf.Max(0f, _attackCooldown - Time.deltaTime);
        while (_isAggroed && _attackCooldown <= 0f )
        {
            HandleAttack();
        }
    }
}
