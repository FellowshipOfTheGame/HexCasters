using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Map", menuName = "Map Layout", order = 1)]
public class MapLayout : ScriptableObject {

	public int nrows;
	public int ncols;

	public HexTerrain defaultTerrain;

	public Dictionary<HexTerrain, List<HexPos>> diffTerrain;

}
