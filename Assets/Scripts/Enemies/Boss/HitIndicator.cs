using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HitIndicator : MonoBehaviour
{
    private const float BLINK_RATE = 0.5f;

    public SpriteRenderer hitbox = null;

    [Header("Parameters")] public float startAlpha;
    public float endAlpha;

    [Tooltip("How long the blinking will go on for.")]
    public float duration = 0f;

    [Tooltip("Optional: Ignores duration to keep looping.")]
    public bool loopEndlessly = false;

    [Tooltip("Optional: How fast you want the blinking to oscillate.")]
    public float blinkRate = BLINK_RATE;

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

        percentComplete += Blink();

        CheckBlinkComplete(percentComplete, 1f);
    }

    public void StartBlinking()
    {
        if (loopEndlessly)
        {
            blinkCount = -1;
            duration = blinkRate;
        }
        else
        {
            blinkCount = Mathf.RoundToInt(duration / blinkRate);
            duration = duration / blinkCount;
        }

        this.isBlinking = true;
        fadingIn = true;
    }

    public void StopBlinking(float endingAlpha = 0f)
    {
        blinkCount = 0;
        this.isBlinking = false;
        fadingIn = true;
        ChangeAlpha(endingAlpha);
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
            else if (blinkCount != -1)
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

    public void ChangeAlpha(float newAlpha)
    {
        Color temp = hitbox.color;
        temp.a = newAlpha;
        hitbox.color = temp;
    }

    public void ChangeColor(Color newColor)
    {
        Color temp = newColor;
        temp.a = hitbox.color.a; // Preserve the Alpha
        hitbox.color = temp;
    }

    public void IndicateBlinking(float startingAlpha, float endingAlpha, float blinkingDuration,
        float blinkingRate = BLINK_RATE, bool loopEndless = false)
    {
        this.startAlpha = startingAlpha;
        this.endAlpha = endingAlpha;
        this.duration = blinkingDuration;
        this.loopEndlessly = loopEndless;
        this.blinkRate = blinkingRate;
        this.StartBlinking();
    }
}