using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapLayout))]
public class MapLayoutEditor : Editor {

	private HexTerrain newTerrain;
	private int newTerrainX;
	private int newTerrainY;

	private int curExpandedTerr = -1;

	private bool nondefaultFold = false;
	private bool spawnRFold = false;
	private bool spawnBFold = false;

	private bool orbFold = false;

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
			DifferentTerrainSection(layout);
			EditorGUILayout.Space();
		}

		spawnRFold = EditorGUILayout.Foldout(
			spawnRFold,
			"Red spawns",
			true);
		if (spawnRFold) {
			SpawnRSection(layout);
		}
		spawnBFold = EditorGUILayout.Foldout(
			spawnBFold,
			"Blue spawns",
			true);
		if (spawnBFold) {
			SpawnBSection(layout);
		}

		orbFold = EditorGUILayout.Foldout(
			orbFold,
			"Orbs",
			true);
		if (orbFold) {
			OrbSection(layout);
		}

		// TODO obstacles
		// TODO effects

	}

	void DifferentTerrainSection(MapLayout layout) {
		if (layout.diffTerrain == null) {
			layout.diffTerrain = new MapLayout.DTList();
		}

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

				curExpandedTerr = i;
				List<HexPos> posToRemove = new List<HexPos>();
				foreach (HexPos p in layout.Find(terrain)) {
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.SelectableLabel(p.x.ToString());
					EditorGUILayout.SelectableLabel(p.y.ToString());
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
}
