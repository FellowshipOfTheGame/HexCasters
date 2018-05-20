using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// [CustomEditor(typeof(MapLayout))]
[Obsolete("Doesn't work, just use the default inspector")]
public class MapLayoutEditor : Editor {

	private HexTerrain newTerrain;
	private int newTerrainX;
	private int newTerrainY;

	private int curExpandedTerr = -1;

	private bool nondefaultFold = false;
	private bool spawnRFold = false;
	private bool spawnBFold = false;

	private bool orbFold = false;

	private bool obstaclesFold = false;
	private int newObstacleX;
	private int newObstacleY;

	private bool effectFold = false;
	private bool fireFold = false;
	private bool snowFold = false;
	private bool rainFold = false;

	public override void OnInspectorGUI() {
		MapLayout layout = target as MapLayout;
		EditorGUILayout.BeginHorizontal();
		layout.nrows = EditorGUILayout.IntField("Dimensions", layout.nrows);
		layout.ncols = EditorGUILayout.IntField(layout.ncols);
		EditorGUILayout.EndHorizontal();
		layout.defaultTerrain =
			EditorGUILayout.ObjectField(
				"Default terrain",
				layout.defaultTerrain,
				typeof(HexTerrain),
				false) as HexTerrain;

		nondefaultFold = EditorGUILayout.Foldout(
			nondefaultFold,
			"Nondefault terrains",
			true);
		if (nondefaultFold) {
			EditorGUI.indentLevel++;
			DifferentTerrainSection(layout);
			EditorGUILayout.Space();
			EditorGUI.indentLevel--;
		}

		spawnRFold = EditorGUILayout.Foldout(
			spawnRFold,
			"Red spawns",
			true);
		if (spawnRFold) {
			EditorGUI.indentLevel++;
			SpawnRSection(layout);
			EditorGUI.indentLevel--;
		}
		spawnBFold = EditorGUILayout.Foldout(
			spawnBFold,
			"Blue spawns",
			true);
		if (spawnBFold) {
			EditorGUI.indentLevel++;
			SpawnBSection(layout);
			EditorGUI.indentLevel--;
		}

		orbFold = EditorGUILayout.Foldout(
			orbFold,
			"Orbs",
			true);
		if (orbFold) {
			EditorGUI.indentLevel++;
			OrbSection(layout);
			EditorGUI.indentLevel--;
		}

		obstaclesFold = EditorGUILayout.Foldout(
			obstaclesFold,
			"Obstacles",
			true);
		if (obstaclesFold) {
			EditorGUI.indentLevel++;
			ObstacleSection(layout);
			EditorGUI.indentLevel--;
		}
		
		effectFold = EditorGUILayout.Foldout(
			effectFold,
			"Effects",
			true);

		if (effectFold) {
			EditorGUI.indentLevel++;
			fireFold = EditorGUILayout.Foldout(
				fireFold,
				"Fire",
				true);
			if (fireFold) {
				EditorGUI.indentLevel++;
				EffectSection(layout, ref layout.fire);
				EditorGUI.indentLevel--;
			}
			rainFold = EditorGUILayout.Foldout(
				rainFold,
				"Rain",
				true);
			if (rainFold) {
				EditorGUI.indentLevel++;
				EffectSection(layout, ref layout.rain);
				EditorGUI.indentLevel--;
			}
			snowFold = EditorGUILayout.Foldout(
				snowFold,
				"Snow",
				true);
			if (snowFold) {
				EditorGUI.indentLevel++;
				EffectSection(layout, ref layout.snow);
				EditorGUI.indentLevel--;
			}
			EditorGUI.indentLevel--;
		}
	}

	void DifferentTerrainSection(MapLayout layout) {
		newTerrain = EditorGUILayout.ObjectField(
				"Add new terrain",
				newTerrain,
				typeof(HexTerrain),
				false
			) as HexTerrain;
		EditorGUILayout.BeginHorizontal();
		newTerrainX = EditorGUILayout.IntField("Position", newTerrainX);
		newTerrainY = EditorGUILayout.IntField(newTerrainY);
		EditorGUILayout.EndHorizontal();
		if (newTerrain != null) {
			if (GUILayout.Button("Add")) {
				layout.diffTerrain.Add(
					new MapLayout.DifferentTerrainInstance(
						newTerrainX, newTerrainY, newTerrain));
			}
		}

		EditorGUILayout.Space();

		bool someExpandedTerrain = false;
		int i = 0;
		foreach (HexTerrain terrain in layout.ListDifferentTerrains()) {
			if (EditorGUILayout.Foldout(
				i == curExpandedTerr,
				terrain.type,
				true)) {

				EditorGUI.indentLevel++;
				curExpandedTerr = i;
				List<HexPos> posToRemove = new List<HexPos>();
				foreach (HexPos p in layout.Find(terrain)) {
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.SelectableLabel(
						String.Format("({0}, {1})", p.x, p.y));
					if (GUILayout.Button("x")) {
						posToRemove.Add(p);
					}
					EditorGUILayout.EndHorizontal();
				}
				posToRemove.Reverse();
				foreach (HexPos pos in posToRemove) {
					layout.RemoveTerrain(pos);
				}
				someExpandedTerrain = true;
				EditorGUI.indentLevel--;
			}
			i++;
		}

		if (!someExpandedTerrain) {
			curExpandedTerr = -1;
		}
	}

	void SpawnRSection(MapLayout layout) {
		SpawnSection(layout, ref layout.spawnR);
	}

	void SpawnBSection(MapLayout layout) {
		SpawnSection(layout, ref layout.spawnB);
	}

	void SpawnSection(MapLayout layout, ref HexPos[] spawns) {
		if (spawns == null) {
			spawns = new HexPos[0];
		}
		int count = EditorGUILayout.DelayedIntField("Count", spawns.Length);
		Array.Resize(ref spawns, count);
		for (int i = 0; i < count; i++) {
			if (spawns[i] == null) {
				spawns[i] = new HexPos(0, 0);
			}
			HexPos pos = spawns[i];
			EditorGUILayout.BeginHorizontal();
			pos.x = EditorGUILayout.IntField(pos.x);
			pos.y = EditorGUILayout.IntField(pos.y);
			EditorGUILayout.EndHorizontal();
		}
	}

	void OrbSection(MapLayout layout) {
		EditorGUILayout.BeginHorizontal();
		layout.orbR.x = EditorGUILayout.IntField("Red", layout.orbR.x);
		layout.orbR.y = EditorGUILayout.IntField(layout.orbR.y);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		layout.orbB.x = EditorGUILayout.IntField("Blue", layout.orbB.x);
		layout.orbB.y = EditorGUILayout.IntField(layout.orbB.y);
		EditorGUILayout.EndHorizontal();
	}

	void ObstacleSection(MapLayout layout) {
		List<int> toRemove = new List<int>();
		int i = 0;
		foreach (MapLayout.ObstacleInstance oi in layout.obstacles) {
			EditorGUILayout.BeginHorizontal();
			oi.obstacle = EditorGUILayout.ObjectField(
				oi.obstacle,
				typeof(GameObject),
				false) as GameObject;
			oi.pos.x = EditorGUILayout.IntField(oi.pos.x);
			oi.pos.y = EditorGUILayout.IntField(oi.pos.y);
			if (GUILayout.Button("x")) {
				toRemove.Add(i);
			}
			EditorGUILayout.EndHorizontal();
			i++;
		}
		toRemove.Reverse();
		foreach (int idx in toRemove) {
			layout.obstacles.RemoveAt(idx);
		}
		EditorGUILayout.BeginHorizontal();
		newObstacleX = EditorGUILayout.IntField(newObstacleX);
		newObstacleY = EditorGUILayout.IntField(newObstacleY);
		if (GUILayout.Button("Add")) {
			layout.obstacles.Add(
				new MapLayout.ObstacleInstance(
					newObstacleX, newObstacleY, null));
		}
		EditorGUILayout.EndHorizontal();
	}

	void EffectSection(MapLayout layout, ref HexPos[] eff) {
		if (eff == null) {
			eff = new HexPos[0];
		}
		int count = EditorGUILayout.DelayedIntField("Count", eff.Length);
		Array.Resize(ref eff, count);
		for (int i = 0; i < count; i++) {
			if (eff[i] == null) {
				eff[i] = new HexPos(0, 0);
			}
			HexPos pos = eff[i];
			EditorGUILayout.BeginHorizontal();
			pos.x = EditorGUILayout.IntField(pos.x);
			pos.y = EditorGUILayout.IntField(pos.y);
			EditorGUILayout.EndHorizontal();
		}
	}
}