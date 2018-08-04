using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HelpManager : MonoBehaviour {

	public Transform canvas;
	public int startScreen = 0;

	public Button nextButton;
	public Button prevButton;

	Transform screens;
	int curScreen;

	void Awake() {
		screens = canvas.Find("Screens");
	}

	void Start() {
		DisableAllScreens();
		EnableScreen(startScreen);
	}

	void EnableScreen(int screen) {
		curScreen = screen;
		screens.GetChild(screen).gameObject.SetActive(true);
		prevButton.interactable = screen != 0;
		nextButton.interactable = screen != screens.childCount-1;
	}

	void DisableScreen(int screen) {
		screens.GetChild(screen).gameObject.SetActive(false);
	}

	void DisableAllScreens() {
		foreach (Transform scr in screens) {
			scr.gameObject.SetActive(false);
		}
	}

	public void NextScreen() {
		if (curScreen + 1 == screens.childCount) {
			return;
		}
		DisableScreen(curScreen);
		EnableScreen(curScreen + 1);
	}

	public void PrevScreen() {
		if (curScreen == 0) {
			return;
		}
		DisableScreen(curScreen);
		EnableScreen(curScreen - 1);
	}

	public void ExitHelp() {
		SceneManager.LoadScene("Help Select");
	}
}
