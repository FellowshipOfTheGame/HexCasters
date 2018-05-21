using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour {

	public MapLayout layout;

	public void PlayGame() {
		MapLoader.LoadLayout(layout);
	}

	public void QuitGame() {
		Application.Quit();
	}

}
