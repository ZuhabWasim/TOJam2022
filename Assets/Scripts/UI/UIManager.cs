using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
	public Animator startButton;
	public Animator settingsButton;
	public Animator dialog;
	

	void Start() { Time.timeScale = 1; }
	
	public void OpenMenuSettings() {
		startButton.SetBool("isHidden", true);
		settingsButton.SetBool("isHidden", true);
		dialog.SetBool("isHidden", false);
	}
	
	
	
	
	public void StartGame(){
		SceneManager.LoadScene("UIOverlay");
	}
	
	public void CloseMenuSettings(){
		startButton.SetBool("isHidden", false);
		settingsButton.SetBool("isHidden", false);
		dialog.SetBool("isHidden", true);
	}
	

	
}
