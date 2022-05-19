using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointAnimator : MonoBehaviour
{
    public GameObject checkpointObject = null;

    [SerializeField] private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        if (checkpointObject == null)
        {
            Debug.LogWarning("Checkpoint Animator not set, there will be no animations");
            return;
        }
		
        Checkpoint.ActivateCheckpoint += checkpoint_OnActivateCheckpoint;
    }

    void checkpoint_OnActivateCheckpoint()
    {
        GameObject activatedCheckpoint = Checkpoint.GetActiveCheckpoint();
		
        if (checkpointObject == activatedCheckpoint)
        {
            _animator.SetBool("isActivated", true);
        }
    }
}