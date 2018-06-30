using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapSelectMenu : MonoBehaviour {

	public Text mapName;

	public void MouseOver(string name) {
		mapName.text = name;
	}

	public void MouseExit() {
		mapName.text = "";
	}

	public void SelectMap(MapLayout layout) {
		MapLoader.LoadLayout(layout);
	}

	public void BackToMainMenu() {
		SceneManager.LoadScene("MainMenu");
	}

}
