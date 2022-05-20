using UnityEngine;

public class BossParameters : MonoBehaviour
{
    public static readonly float ARENA_TRANSITION_DURATION = 3f;

    public static readonly float PRE_ATTACK_TRANSITION_DURATION = 1f;
    public static readonly float PRE_ATTACK_PAUSE_DURATION = 3f;

    public static readonly float HIT_INDICATOR_MIN_ALPHA = 0.25f;
    public static readonly float HIT_INDICATOR_MAX_ALPHA = 0.6f;

    public static readonly float ATTACKING_TRANSITION_DURATION = 0.5f;
    public static readonly float ATTACKING_PAUSE_DURATION = 1f;

    public static readonly float IDLING_TRANSITION_DURATION = 1f;
    public static readonly float IDLING_BOB_DISTANCE = 0.2f;
    public static readonly float IDLING_BOB_DURATION = 1f;

    public static readonly float ATTACK_COOLDOWN = 4f;

    public static readonly float BOSS_HIT_FLASH_DURATION = 0.5f;
    public static readonly float BOSS_HIT_FLASH_MIN_ALPHA = 0f;
    public static readonly float BOSS_HIT_FLASH_MAX_ALPHA = 1f;

    public static readonly float INSTANT_MOVE_BUFFER = 0.1f;

    public static readonly float DEATH_SHAKE_DISTANCE = 0.2f;
    public static readonly float DEATH_SHAKE_DURATION = 0.1f;

    public static readonly float DEATH_FLASH_MIN_ALPHA = 0.25f;
    public static readonly float DEATH_FLASH_MAX_ALPHA = 1f;

    public static readonly float DEATH_FADE_DURATION = 3f;
    public static readonly float DEATH_FADE_MIN_ALPHA = 0f;
    public static readonly float DEATH_FADE_MAX_ALPHA = 1f;
}