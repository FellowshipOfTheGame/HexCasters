using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundMapLoader : MonoBehaviour {

	public MapLayout layout;

	void Awake() {
		SceneManager.sceneLoaded += Loaded;
		MapLoader.LoadLayout(layout, LoadSceneMode.Additive);
	}

	void Start() {
		GameObject[] goArray = SceneManager.GetSceneByName("MapBaseScene").GetRootGameObjects();
		foreach (GameObject go in goArray) {
			if (go.name == "Canvas" || go.name == "Menu Canvas" || go.name == "Winner Canvas") {
				go.SetActive(false);
			}
			if (go.name == "Board") {
				go.GetComponent<Timer>().enabled = false;
			}
		}
	}

	void Loaded(Scene scene, LoadSceneMode mode) {
		SceneManager.sceneLoaded -= Loaded;
		//Invoke("OnLoad", 0.1f);
	}

	void OnLoad() {
	}

}
