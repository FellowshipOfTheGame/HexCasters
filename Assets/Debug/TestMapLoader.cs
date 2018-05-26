using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMapLoader : MonoBehaviour {

	public MapLayout layout;

	void Start() {
		DontDestroyOnLoad(gameObject);
		MapLoader.LoadLayout(layout);
	}

}
