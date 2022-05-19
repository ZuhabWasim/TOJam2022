using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HitIndicator : MonoBehaviour
{
    private const float BLINK_RATE = 0.5f;

    public SpriteRenderer hitbox = null;
    public float duration = 0f;
    public float startAlpha;
    public float endAlpha;

    [SerializeField] private bool fadingIn = true;
    [SerializeField] private float elapsedTime = 0;
    [SerializeField] private int blinkCount = 0;
    [SerializeField] private bool isBlinking = false;

    private const float SNAP_ALLOWANCE = 0.01f;

    // Update is called once per frame
    void Update()
    {
        if (!isBlinking)
        {
            elapsedTime = 0f;
            return;
        }

        elapsedTime += Time.deltaTime;

        float percentComplete = 0.0f;
        Debug.Log("percentComplete: " + percentComplete);

        percentComplete += Blink();

        CheckBlinkComplete(percentComplete, 1f);
    }

    public void StartBlinking()
    {
        blinkCount = Mathf.RoundToInt(duration / BLINK_RATE);
        duration = duration / blinkCount;
        this.isBlinking = true;
        fadingIn = true;
    }

    public void StopBlinking(float endAlpha = 0f)
    {
        blinkCount = 0;
        this.isBlinking = false;
        fadingIn = true;
        ChangeAlpha(endAlpha);
    }


    private float Blink()
    {
        float percentComplete = elapsedTime / duration;

        if (fadingIn)
        {
            ChangeAlpha(Mathf.Lerp(startAlpha, endAlpha, percentComplete));
        }
        else
        {
            ChangeAlpha(Mathf.Lerp(endAlpha, startAlpha, percentComplete));
        }

        return percentComplete;
    }

    private void CheckBlinkComplete(float percentComplete, float targetPercent)
    {
        if (percentComplete >= targetPercent - SNAP_ALLOWANCE)
        {
            FinishBlinking();

            if (blinkCount == 1) // Stop looping animation
            {
                isBlinking = false;
            }
            else
            {
                blinkCount -= 1;
            }

            elapsedTime = 0;
        }
    }

    private void FinishBlinking()
    {
        ChangeAlpha(fadingIn ? endAlpha : startAlpha);
        fadingIn = !fadingIn;
    }

    void ChangeAlpha(float newAlpha)
    {
        Color temp = hitbox.color;
        temp.a = newAlpha;
        hitbox.color = temp;
    }
}