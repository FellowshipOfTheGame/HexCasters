using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpManager : MonoBehaviour {

	public Transform canvas;
	public int startScreen = 0;

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
	}

	void DisableScreen(int screen) {
		screens.GetChild(screen).gameObject.SetActive(false);
	}

	void DisableAllScreens() {
		foreach (Transform scr in screens) {
			scr.gameObject.SetActive(false);
		}
	}

	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			NextScreen();
		} else if (Input.GetMouseButtonDown(1)) {
			PrevScreen();
		}
	}

	void NextScreen() {
		if (curScreen + 1 == screens.childCount) {
			return;
		}
		DisableScreen(curScreen);
		EnableScreen(curScreen + 1);
	}

	void PrevScreen() {
		if (curScreen == 0) {
			return;
		}
		DisableScreen(curScreen);
		EnableScreen(curScreen - 1);
	}
}
