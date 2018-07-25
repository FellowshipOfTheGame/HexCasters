using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimerMenu : MonoBehaviour {

	public static TimerMenu TMenu;

	public float turnTime;

	void Start() {
		TMenu = this;
	}

	public void SelectTimer(float time) {
		turnTime = time;
		SceneManager.LoadScene("MapSelectMenu");
	}

	public void BackToMainMenu() {
		SceneManager.LoadScene("MainMenu");
	}
}
