using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour {

	public static bool inGameMenuIsActive = false;
	public GameObject inGameMenu;

	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (inGameMenuIsActive) {
				Resume();
			}
			else {
				LoadInGameMenu();
			}
		}
	}

	public void Resume() {
		inGameMenu.SetActive(false);
		inGameMenuIsActive = false;
	}

	public void LoadInGameMenu() {
		inGameMenu.SetActive(true);
		inGameMenuIsActive = true;
	}

	public void LoadMainMenu() {
		SceneManager.LoadScene("MainMenu");
	}
}
