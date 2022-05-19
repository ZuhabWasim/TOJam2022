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
    [Tooltip("Setting this to -1 will loop the animation endlessly.")]
    public int loopCount = 0;

    [Tooltip("How long (in seconds) the movement should take.")]
    public float duration = 0f;

    [Header("Positional")] public bool move = true;
    public Vector3 startPosition;
    public Vector3 endPosition;
    private bool _movingForward = true;

    [Header("Rotational")] public bool rotate = false;
    public Vector3 startRotation;
    public Vector3 endRotation;
    private bool _rotatingForward = true;

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
        _movingForward = true;
    }

    public void BreakMotion()
    {
        this.isMoving = false;
        _movingForward = true;
    }

    private float Move()
    {
        float percentComplete = elapsedTime / duration;

        if (_movingForward)
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

        if (_rotatingForward)
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
        this.gameObject.transform.localPosition = _movingForward ? endPosition : startPosition;
        _movingForward = !_movingForward;
    }

    private void FinishRotating()
    {
        this.gameObject.transform.localRotation = Quaternion.Euler(_rotatingForward ? endRotation : startRotation);
        _rotatingForward = !_rotatingForward;
    }

    public void MoveBoss(bool isMove, Vector3 movePosition,
        bool isRotate, Vector3 rotationAngle,
        float desiredDuration, int loopMovement = 0)
    {
        // Break motion for anything done before.
        BreakMotion();

        // Duration and looping.
        this.loopCount = loopMovement;
        this.duration = desiredDuration;

        // If the boss needs to move, set the parameters.
        if (isMove)
        {
            this.move = true;
            this.startPosition = transform.localPosition;
            this.endPosition = movePosition;
        }
        else
        {
            this.move = false;
        }

        // If the boss needs to rotate, set the parameters.
        if (isRotate)
        {
            this.rotate = true;
            this.startRotation = transform.localRotation.eulerAngles;
            this.endRotation = rotationAngle;
        }
        else
        {
            this.rotate = false;
        }

        // Move the boss asynchronously (passive movement).
        TriggerMotion();
    }
}