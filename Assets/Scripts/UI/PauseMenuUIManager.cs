using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuUIManager : MonoBehaviour
{
	[SerializeField] private GameObject pauseMenu;
	[SerializeField] private GameObject pauseButton;

	void Start() { Time.timeScale = 1; }
	
	public void Quit(){
		Application.Quit();
	}
	
	public void MainMenu(){
		SceneManager.LoadScene("MenuScene");
	}
	
	public void PauseButton(){
		Time.timeScale = 0f;
		pauseMenu.SetActive(true);
		pauseButton.SetActive(false);
	}
	
	public void ResumeButton(){
		Time.timeScale = 1.0f;
		pauseMenu.SetActive(false);
		pauseButton.SetActive(true);
	}
}
