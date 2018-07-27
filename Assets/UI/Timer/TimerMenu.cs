using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimerMenu : MonoBehaviour {

	public static float turnTime;
	public static bool isSelected;

	void Awake() {
		isSelected = false;
	}

	public void SelectTimer(float time) {
		turnTime = time;
		isSelected = true;
		SceneManager.LoadScene("MapSelectMenu");
	}

	public void BackToMainMenu() {
		SceneManager.LoadScene("MainMenu");
	}
}
