using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Map", menuName = "Map Layout", order = 1)]
public class MapLayout : ScriptableObject {	

	public enum Effect {
		NONE,
		FLAMES,
		STORM,
		SNOW
	}

	[Serializable]
	public class HexInfo {
		public HexTerrain terrain;
		public MapLayout.Effect effect = MapLayout.Effect.NONE;

		public GameObject content;
		public Team contentTeam;
	}

	[Serializable]
	public class PosInfoDict :
		SerializableDictionary<HexPos, MapLayout.HexInfo> {}

	public int nrows;
	public int ncols;

	public HexTerrain defaultTerrain;

	public PosInfoDict info;
/*
	public List<HexPos> Find(HexTerrain type) {
		List<HexPos> positions = new List<HexPos>();
		foreach (DifferentTerrainInstance inst in diffTerrain) {
			if (inst.terrain == type) {
				positions.Add(inst.pos);
			}
		}
		return positions;
	}

	public HexTerrain Find(int x, int y) {
		foreach (DifferentTerrainInstance inst in diffTerrain) {
			if (inst.pos.x == x && inst.pos.y == y) {
				return inst.terrain;
			}
		}
		return null;
	}

	public void RemoveTerrain(HexTerrain terr) {
		List<int> toRemove = new List<int>();
		int i = 0;
		foreach (DifferentTerrainInstance inst in diffTerrain) {
			if (inst.terrain == terr) {
				toRemove.Add(i);
			}
			i++;
		}

		toRemove.Reverse();
		foreach (int idx in toRemove) {
			diffTerrain.RemoveAt(idx);
		}
	}

	public void RemoveTerrain(HexPos pos) {
		List<int> toRemove = new List<int>();
		int i = 0;
		foreach (DifferentTerrainInstance inst in diffTerrain) {
			if (inst.pos.x == pos.x && inst.pos.y == pos.y) {
				toRemove.Add(i);
			}
			i++;
		}

		toRemove.Reverse();
		foreach (int idx in toRemove) {
			diffTerrain.RemoveAt(idx);
		}
	}

	public HashSet<HexTerrain> ListDifferentTerrains() {
		HashSet<HexTerrain> terr = new HashSet<HexTerrain>();
		foreach (DifferentTerrainInstance inst in diffTerrain) {
			terr.Add(inst.terrain);
		}
		return terr;
	}
*/
}
