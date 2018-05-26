using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "hue", menuName = "HUE", order = 1)]
public class DummySO : ScriptableObject {

	[System.Serializable]
	public class Test {
		public GameObject thing;
		public HexPos pos;
	}

	public List<Test> prefab;
}
