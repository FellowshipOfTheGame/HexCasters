using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestMapLoader : MonoBehaviour {

	public MapLayout layout;
	public GameObject prefabWall;

	void Start() {
		DontDestroyOnLoad(gameObject);
		SceneManager.sceneLoaded += Loaded;
		MapLoader.LoadLayout(layout);
	}

	void Loaded(Scene scene, LoadSceneMode mode) {
		SceneManager.sceneLoaded -= Loaded;
		Invoke("OnLoad", 0.1f);
	}

	void OnLoad() {
	}

}
