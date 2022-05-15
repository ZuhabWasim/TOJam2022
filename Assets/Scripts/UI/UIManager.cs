using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
	public Animator startButton;
	public Animator settingsButton;
	public Animator dialog;
	public Animator animator;

	void Awake() {
		animator = GameObject.Find("Transition").GetComponent<Animator>();

	}
	void Start() { 
		Time.timeScale = 1; 
	}
	
	
	public void OpenMenuSettings() {
		startButton.SetBool("isHidden", true);
		settingsButton.SetBool("isHidden", true);
		dialog.SetBool("isHidden", false);
	}
	
	public void StartGame(){

		animator.SetTrigger("TriggerTransition");
		
		SceneManager.LoadScene("TestSceneLarge");
	}
	
	public void CloseMenuSettings(){
		startButton.SetBool("isHidden", false);
		settingsButton.SetBool("isHidden", false);
		dialog.SetBool("isHidden", true);
	}
	

	

	
}
