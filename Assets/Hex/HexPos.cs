using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HexPos {

	public int x;
	public int y;

	public HexPos(int x, int y) {
		this.x = x;
		this.y = y;
	}

	public override string ToString() {
		return string.Format("({0}, {1})", x, y);
	}

}
