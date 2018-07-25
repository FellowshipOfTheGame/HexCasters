using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	public void PlayGame() {
		SceneManager.LoadScene("TimerMenu");
	}

	public void QuitGame() {
		Application.Quit();
	}

	public void ShowHelp() {
		SceneManager.LoadScene("InfoDump");
	}

}
