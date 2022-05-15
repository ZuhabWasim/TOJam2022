using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCheck : MonoBehaviour
{

    [SerializeField] private int _patrolEnemyDmg;
    [SerializeField] private int _flyingEnemyDmg;
    [SerializeField] private float _invincibleTime;
    [SerializeField] private float invincibilityDeltaTime;
    private SpriteRenderer _spriteRenderer;
    private bool _isInvincible;
    private Color _curColor;

    private void Start()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _curColor = _spriteRenderer.color;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!_isInvincible)
        {
            if (collision.tag == "PatrolEnemy")
            {
                this.gameObject.GetComponent<Health>().BeDamaged(_patrolEnemyDmg);
                triggerInvcinibility();
            }

            if (collision.tag == "FlyingEnemy")
            {
                this.gameObject.GetComponent<Health>().BeDamaged(_flyingEnemyDmg);
                triggerInvcinibility();
            }
        }
    }

    private void triggerInvcinibility()
    {
        StartCoroutine(CountdownInvincibilityTime(_invincibleTime));
    }


    private IEnumerator CountdownInvincibilityTime(float seconds)
    {
        _isInvincible = true;
        for (float i = 0; i < seconds; i += invincibilityDeltaTime)
        {
            // Alternate between 0 and 1 scale to simulate flashing
            if (_spriteRenderer.color.a > 0.0f)
            {
                setAlpha(0.0f);
            }
            else
            {
                setAlpha(1.0f);
            }
            yield return new WaitForSeconds(invincibilityDeltaTime);
        }
        _isInvincible = false;
    }

    private void setAlpha(float alpha)
    {
        _curColor.a = alpha;
        _spriteRenderer.color = _curColor;
    }
}
