#define DEBUG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BossCutsceneManager : MonoBehaviour
{
    [SerializeField] private GameObject bossSprite;
    [Header("Positions")] [SerializeField] private GameObject startPosition;
    [SerializeField] private GameObject awaitPosition;
    [SerializeField] private GameObject awaitMiddlePosition;

    private BossMover _bossMover;
    private HitIndicator _bossSpriteIndicator;

    private IEnumerator _coroutine;

    // Start is called before the first frame update
    void Start()
    {
        Assert.IsNotNull(bossSprite);

        _bossSpriteIndicator = bossSprite.GetComponent<HitIndicator>();
        _bossMover = GetComponent<BossMover>();

        StartAtPlayer();

#if DEBUG
        EventManager.Sub(InputManager.GetKeyDownEventName(KeyBinds.DEBUG6), StartAtPlayer);
        EventManager.Sub(InputManager.GetKeyDownEventName(KeyBinds.DEBUG7), MoveToArena);
        EventManager.Sub(InputManager.GetKeyDownEventName(KeyBinds.DEBUG0), KillBoss);
#endif
    }

    void RestartCoroutine(IEnumerator coroutine)
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }

        this._coroutine = coroutine;
        StartCoroutine(_coroutine);
    }

    public void StartAtPlayer()
    {
        RestartCoroutine(IdleAtPosition(startPosition.transform.localPosition, BossParameters.INSTANT_MOVE_BUFFER));
#if DEBUG
        Debug.Log("Moving boss to start area.");
#endif
    }

    public void MoveToArena()
    {
        // TODO: Trigger this when the player is done the start up dialog
        
        RestartCoroutine(MoveAndClip(awaitMiddlePosition.transform.localPosition,
            BossParameters.ARENA_TRANSITION_DURATION, awaitPosition.transform.localPosition));

#if DEBUG
        Debug.Log("Moving boss to arena.");
#endif
    }

    IEnumerator IdleAtPosition(Vector3 idlePosition, float duration)
    {
        _bossMover.MoveBoss(true, idlePosition,
            false, Vector3.zero, duration);

        yield return new WaitForSeconds(duration);

        _bossMover.MoveBoss(true, transform.localPosition + new Vector3(0, BossParameters.IDLING_BOB_DISTANCE, 0),
            false, Vector3.zero, BossParameters.IDLING_BOB_DURATION, -1);
    }

    IEnumerator MoveAndClip(Vector3 movePosition, float duration, Vector3 clipPosition)
    {
        _bossMover.BreakMotion();
        yield return new WaitForSeconds(BossParameters.INSTANT_MOVE_BUFFER);

        _bossMover.MoveBoss(true, movePosition,
            false, Vector3.zero, duration);

        yield return new WaitForSeconds(duration);

        _bossMover.MoveBoss(true, clipPosition,
            false, Vector3.zero, BossParameters.INSTANT_MOVE_BUFFER);

        yield return new WaitForSeconds(BossParameters.INSTANT_MOVE_BUFFER);
    }

    public void AnimateDeath()
    {
        // Make the boss violently shake back and forth while flashing red.
        _bossMover.BreakMotion();
        _bossSpriteIndicator.ChangeColor(Color.red);
        _bossSpriteIndicator.IndicateBlinking(BossParameters.DEATH_FLASH_MAX_ALPHA,
            BossParameters.DEATH_FLASH_MIN_ALPHA, 0, loopEndless: true);
        _bossMover.MoveBoss(true, transform.localPosition + new Vector3(BossParameters.DEATH_SHAKE_DISTANCE, 0, 0),
            false, Vector3.zero, BossParameters.DEATH_SHAKE_DURATION, -1);
			
		GameObject go = GameObject.Find("FinalBossEndTrigger");
        DialogueTrigger trigger = (DialogueTrigger) go.GetComponent(typeof(DialogueTrigger));
        trigger.TriggerDialogue();
        // TODO: Trigger dialog for what happens when the boss is dying.
    }

    public void KillBoss()
    {
        // TODO: Invoke this method when the player is done hearing the boss dialog.
		
        StartCoroutine(BossDisappear());
#if DEBUG
        Debug.Log("The boss is dead!");
#endif
    }

    IEnumerator BossDisappear()
    {
        _bossSpriteIndicator.IndicateBlinking(BossParameters.DEATH_FADE_MAX_ALPHA,
            BossParameters.DEATH_FADE_MIN_ALPHA, BossParameters.DEATH_FADE_DURATION,
            blinkingRate: BossParameters.DEATH_FADE_DURATION);
		FindObjectOfType<SoundManager>().PlayBossDeath();
        yield return new WaitForSeconds(BossParameters.DEATH_FADE_DURATION);

        this.gameObject.SetActive(false);
		FindObjectOfType<PauseMenuUIManager>().StartFinalCutscene();
    }
}