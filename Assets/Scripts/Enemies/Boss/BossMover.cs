#undef DEBUG

using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

/*
 * Based on by Lakshya Gupta's InterpolateTransform.cs script made for Reflection.
 */
public class BossMover : MonoBehaviour
{
    public int loopCount = 0;
    public float duration = 0f;

    [Header("Positional")] public bool move = true;
    public Vector3 startPosition;
    public Vector3 endPosition;
    private bool movingForward = true;

    [Header("Rotational")] public bool rotate = false;
    public Vector3 startRotation;
    public Vector3 endRotation;
    private bool rotatingForward = true;

    private float elapsedTime = 0;
    [SerializeField] private bool isMoving = false;

    private const float SNAP_ALLOWANCE = 0.01f;

    void Start()
    {
#if DEBUG
        EventManager.Sub(InputManager.GetKeyDownEventName(KeyBinds.CROWCH_DASH_KEY), TriggerMotion);
#endif
    }
    
    // Update is called once per frame
    void Update()
    {
        if (!isMoving)
        {
            elapsedTime = 0f;
            return;
        }

        elapsedTime += Time.deltaTime;

        float percentComplete = 0.0f;

        if (move)
        {
            percentComplete += Move();
        }

        if (rotate)
        {
            percentComplete += Rotate();
        }

        CheckMotionComplete(percentComplete, (move ? 1f : 0f) + (rotate ? 1f : 0f));
    }

    public void TriggerMotion()
    {
        this.isMoving = true;
        movingForward = true;
    }

    public void BreakMotion()
    {
        this.isMoving = false;
        movingForward = true;
    }

    private float Move()
    {
        float percentComplete = elapsedTime / duration;

        if (movingForward)
        {
            this.gameObject.transform.localPosition = Vector3.Lerp(startPosition, endPosition, percentComplete);
        }
        else
        {
            this.gameObject.transform.localPosition = Vector3.Lerp(endPosition, startPosition, percentComplete);
        }

        return percentComplete;
    }

    private float Rotate()
    {
        float percentComplete = elapsedTime / duration;

        if (rotatingForward)
        {
            this.gameObject.transform.localRotation =
                Quaternion.Euler(Vector3.Lerp(startRotation, endRotation, percentComplete));
        }
        else
        {
            this.gameObject.transform.localRotation =
                Quaternion.Euler(Vector3.Lerp(endRotation, startRotation, percentComplete));
        }

        return percentComplete;
    }

    private void CheckMotionComplete(float percentComplete, float targetPercent)
    {
        if (percentComplete >= targetPercent - SNAP_ALLOWANCE)
        {
            if (move) FinishMoving();

            if (rotate) FinishRotating();

            if (loopCount == 0) // Stop looping animation
            {
                isMoving = false;
            }
            else if (loopCount != -1) // Endlessly loop animation
            {
                loopCount -= 1;
            }

            elapsedTime = 0;
        }
    }

    private void FinishMoving()
    {
        this.gameObject.transform.localPosition = movingForward ? endPosition : startPosition;
        movingForward = !movingForward;
    }

    private void FinishRotating()
    {
        this.gameObject.transform.localRotation = Quaternion.Euler(rotatingForward ? endRotation : startRotation);
        rotatingForward = !rotatingForward;
    }
}