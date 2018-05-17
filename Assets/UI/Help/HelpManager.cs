using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpManager : MonoBehaviour {

	public Transform canvas;
	public int startScreen = 0;

	public Text nextButtonText;
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
		if (screen + 1 == screens.childCount) {
			nextButtonText.text = "Finish";
		} else {
			nextButtonText.text = "Next";
		}
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
		// if (Input.GetMouseButtonDown(0)) {
		// 	NextScreen();
		// } else if (Input.GetMouseButtonDown(1)) {
		// 	PrevScreen();
		// }
	}

	public void NextScreen() {
		if (curScreen + 1 == screens.childCount) {
			ExitHelp();
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
		Application.Quit();
	}
}
