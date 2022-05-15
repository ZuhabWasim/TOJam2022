using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFunctions : MonoBehaviour
{
    private AudioSource jumpSound;
	private AudioSource swordSound;
	private AudioSource daggerSound;
	private AudioSource slideSound;
	private AudioSource climbSound;
	private AudioSource hitSound;
	private AudioSource runFastSound;
	private AudioSource runSlowSound;
	
	void Start()
    {
        jumpSound = GetComponent<AudioSource>();
		swordSound = GetComponent<AudioSource>();
		daggerSound = GetComponent<AudioSource>();
		slideSound = GetComponent<AudioSource>();
		climbSound = GetComponent<AudioSource>();
		hitSound = GetComponent<AudioSource>();
		runFastSound = GetComponent<AudioSource>();
		runSlowSound = GetComponent<AudioSource>();
    }
	
	void Update(){
		jumpSound.Play();
		swordSound.Play();
		daggerSound.Play();
		slideSound.Play();
		climbSound.Play();
		hitSound.Play();
		runFastSound.Play();
		runSlowSound.Play();
	}
	
}
