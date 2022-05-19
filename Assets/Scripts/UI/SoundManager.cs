using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] public AudioSource audioSource;
	public AudioClip jumpSound;
	public AudioClip swordSound;
	public AudioClip slideSound;
	public AudioClip climbSound;
	public AudioClip hitSound;
	public AudioClip runSound;
	public AudioClip birdSound;
	public AudioClip snakeSound;
	public AudioClip fireSound;
	public AudioClip metalSound;
	public AudioClip bossLaughSound;
	public AudioClip bossAttackSound;
	public AudioClip bossDeathSound;
	
	public void PlayJump(){
		audioSource.volume = 1f;
		audioSource.PlayOneShot(jumpSound);
	}
	
	public void PlaySword(){
		audioSource.volume = 0.9f;
		audioSource.PlayOneShot(swordSound);
	}
	
	public void PlaySlide(){
		audioSource.volume = 1f;
		audioSource.PlayOneShot(slideSound);
	}
	
	public void PlayClimb(){
		audioSource.volume = 0.6f;
		audioSource.PlayOneShot(climbSound);
	}
	
	public void PlayHit(){
		audioSource.volume = 1f;
		audioSource.PlayOneShot(hitSound);
	}
	
	public void PlayRun(){
		audioSource.volume = 1f;
		audioSource.PlayOneShot(runSound);
	}
	
	public void PlayBird(){
		audioSource.volume = 1f;
		audioSource.PlayOneShot(birdSound);
	}
	public void PlaySnake(){
		audioSource.volume = 0.9f;
		audioSource.PlayOneShot(snakeSound);
	}
	public void PlayFire(){
		audioSource.volume = 1f;
		audioSource.PlayOneShot(fireSound);
	}
	public void PlayMetal(){
		audioSource.volume = 0.4f;
		audioSource.PlayOneShot(metalSound);
	}
	public void PlayBossLaugh(){
		audioSource.volume = 0.5f;
		audioSource.PlayOneShot(bossLaughSound);
	}
	public void PlayBossAttack(){
		audioSource.volume = 0.5f;
		audioSource.PlayOneShot(bossAttackSound);
	}
	public void PlayBossDeath(){
		audioSource.volume = 0.7f;
		audioSource.PlayOneShot(bossDeathSound);
	}
}
