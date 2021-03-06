using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	void Start() {
		if (!AudioManager.AM.IsPlaying("Menu")) {
			AudioManager.AM.Play("Menu");
			AudioManager.AM.SetVolume("soundsVolume", -20.0f);
			AudioManager.AM.SetVolume("musicVolume", -10.0f);
		}
	}

	public void PlayGame() {
		SceneManager.LoadScene("TimerMenu");
	}

	public void QuitGame() {
		Application.Quit();
	}

	public void ShowHelp() {
		SceneManager.LoadScene("Help Select");
	}

}
