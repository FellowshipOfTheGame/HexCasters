using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestMapLoader : MonoBehaviour {

	public MapLayout layout;

	void Start() {
		DontDestroyOnLoad(gameObject);
		SceneManager.sceneLoaded += Loaded;
		MapLoader.LoadLayout(layout);
	}

	void Loaded(Scene scene, LoadSceneMode mode) {
		SceneManager.sceneLoaded -= Loaded;
		Invoke("TestAnim", 0.1f);
	}

	void TestAnim() {
		// Mage m = GameManager.GM.grid[-1, -4].unit.asMage;
		// m.AnimateFireball(GameManager.GM.grid[0, 0]);
	}

}
