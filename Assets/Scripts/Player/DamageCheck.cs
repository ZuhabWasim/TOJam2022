using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCheck : MonoBehaviour
{

    [SerializeField] private float _invincibleTime;
    [Range (0,1)][SerializeField] private float invincibilityDeltaTime;
    private SpriteRenderer _spriteRenderer;
    public bool IsInvincible{
        get;
        set;
    }
    private Color _curColor;

    private void Start()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _curColor = _spriteRenderer.color;
    }

    public void triggerInvincibility()
    {
        StartCoroutine(CountdownInvincibilityTime(_invincibleTime));
    }


    private IEnumerator CountdownInvincibilityTime(float seconds)
    {
        IsInvincible = true;
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
        IsInvincible = false;
    }

    private void setAlpha(float alpha)
    {
        _curColor.a = alpha;
        _spriteRenderer.color = _curColor;
    }
}
