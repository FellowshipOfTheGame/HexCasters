using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour {

	private float timeInTitleScreen;
	public float maxTimeInTitleScreen;

	public void Start() {
		timeInTitleScreen = 0.0f;
	}

	public void Update() {
		timeInTitleScreen += Time.deltaTime;
		if (timeInTitleScreen >= maxTimeInTitleScreen || Input.GetMouseButton(0)) {
			GoToMainMenu();
		}
	}

	public void GoToMainMenu() {
		SceneManager.LoadScene("MainMenu");
	}
}
