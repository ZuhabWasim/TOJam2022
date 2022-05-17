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
	
	public void PlayJump(){
		audioSource.volume = 1f;
		audioSource.PlayOneShot(jumpSound);
	}
	
	public void PlaySword(){
		audioSource.volume = 1f;
		audioSource.PlayOneShot(swordSound);
	}
	
	public void PlaySlide(){
		audioSource.volume = 1f;
		audioSource.PlayOneShot(slideSound);
	}
	
	public void PlayClimb(){
		audioSource.volume = 0.8f;
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
	
}
