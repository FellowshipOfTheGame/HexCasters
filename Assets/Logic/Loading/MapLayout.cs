using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Map", menuName = "Map Layout", order = 1)]
public class MapLayout : ScriptableObject {

	[System.Serializable]
	public class DifferentTerrainInstance {
		public HexTerrain terrain;
		public HexPos pos;

		public DifferentTerrainInstance(int x, int y, HexTerrain terr) {
			pos = new HexPos(x, y);
			terrain = terr;
		}
	}

	[System.Serializable]
	public class ObstacleInstance {
		public GameObject obstacle;
		public HexPos pos;

		public ObstacleInstance(int x, int y, GameObject obst) {
			pos = new HexPos(x, y);
			obstacle = obst;
		}
	}

	public int nrows;
	public int ncols;

	public HexTerrain defaultTerrain;

	public List<DifferentTerrainInstance> diffTerrain;
	public List<ObstacleInstance> obstacles;
	public HexPos[] spawnR;
	public HexPos[] spawnB;

	public HexPos orbR;
	public HexPos orbB;

	public HexPos[] rain;
	public HexPos[] snow;
	public HexPos[] fire;

	// TODO effects

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

}
