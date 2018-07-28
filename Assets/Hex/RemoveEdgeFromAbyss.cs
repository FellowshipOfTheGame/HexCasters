using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveEdgeFromAbyss : MonoBehaviour {
	public HexCell cell;
	public HexTerrain abyss;
	void Start() {
		if (cell.terrain == abyss) {
			transform.Find("Edge").gameObject.SetActive(false);
		}
	}
}
