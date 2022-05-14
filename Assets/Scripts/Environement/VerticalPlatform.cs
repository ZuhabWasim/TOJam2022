using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalPlatform : MonoBehaviour
{
    private PlatformEffector2D effector;
    public float waitDuration = 0.2f;
    public float _waitTime;

    private PlayerController player;
    
    // Start is called before the first frame update
    void Start()
    {
        effector = GetComponent<PlatformEffector2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        EventManager.Sub(InputManager.GetKeyDownEventName(KeyBinds.JUMP_KEY), ResetPlatform);
    }

    // Update is called once per frame
    void Update()
    {
        if (!player.isCrouching())
        {
            _waitTime = waitDuration;
        }

        if (player.isCrouching())
        {
            if (_waitTime <= 0)
            {
                effector.rotationalOffset = 180f;
                this.gameObject.layer = LayerMask.NameToLayer("Background");
                _waitTime = waitDuration;
            }
            else
            {
                _waitTime -= Time.deltaTime;
            }
        }
    }

    void ResetPlatform()
    {
        effector.rotationalOffset = 0f;
        this.gameObject.layer = LayerMask.NameToLayer("Ground");
    }
}
