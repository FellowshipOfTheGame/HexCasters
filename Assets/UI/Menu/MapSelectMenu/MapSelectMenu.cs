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

	public void SelectMap(MapLayout ml) {
		MapLoader.LoadLayout(ml);
	}

	public void BackToTimerMenu() {
		SceneManager.LoadScene("TimerMenu");
	}

}
