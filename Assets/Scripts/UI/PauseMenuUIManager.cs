using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuUIManager : MonoBehaviour
{
	[SerializeField] private GameObject pauseMenu;
	[SerializeField] private GameObject pauseButton;

	public Animator settingsButton;
	public Animator quitButton;
	public Animator menuButton;
	public Animator dialog;
	
	void Start() { Time.timeScale = 1; }
	
	public void Quit(){
		Application.Quit();
	}
	
	public void MainMenu(){
		SceneManager.LoadScene("MenuScene");
	}
	
	public void PauseButton(){
		
		
		pauseMenu.SetActive(true);
		pauseButton.SetActive(false);
		settingsButton.SetBool("isHidden", false);
		quitButton.SetBool("isHidden", false);
		menuButton.SetBool("isHidden", false);
		Time.timeScale = 0f;
	}
	
	public void ResumeButton(){
		Time.timeScale = 1.0f;
		settingsButton.SetBool("isHidden", true);
		quitButton.SetBool("isHidden", true);
		menuButton.SetBool("isHidden", true);
		pauseMenu.SetActive(false);
		pauseButton.SetActive(true);
		
	}
	

	
	public void OpenDialog(){
		dialog.SetBool("isHidden", false);
		settingsButton.SetBool("isHidden", true);
		quitButton.SetBool("isHidden", true);
		menuButton.SetBool("isHidden", true);
	}
	
	public void CloseDialog(){
		dialog.SetBool("isHidden", true);
		settingsButton.SetBool("isHidden", false);
		quitButton.SetBool("isHidden", false);
		menuButton.SetBool("isHidden", false);
	}

}
