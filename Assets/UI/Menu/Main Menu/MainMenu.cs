using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	public MapLayout layout;

	public void PlayGame() {
		SceneManager.LoadScene("MapSelectMenu");
	}

	public void QuitGame() {
		Application.Quit();
	}

	public void ShowHelp() {
		SceneManager.LoadScene("InfoDump");
	}

}
