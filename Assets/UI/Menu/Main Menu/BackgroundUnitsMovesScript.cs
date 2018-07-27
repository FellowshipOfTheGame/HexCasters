using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundUnitsMovesScript : MonoBehaviour {

	void Start () {
		MovesSequence();
	}

	private void MovesSequence() {
		HexCell nextPos = null;
		foreach (HexUnit u in GameManager.GM.teams[0]) {
			GameManager.GM.Click(u.cell);
		}
	}
}
