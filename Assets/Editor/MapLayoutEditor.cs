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

	public override void OnInspectorGUI() {
		MapLayout layout = target as MapLayout;
		EditorGUILayout.BeginHorizontal();
		layout.nrows = EditorGUILayout.IntField("Dimensions:", layout.nrows);
		layout.ncols = EditorGUILayout.IntField(layout.ncols);
		EditorGUILayout.EndHorizontal();
		layout.defaultTerrain =
			EditorGUILayout.ObjectField(
				"Default terrain:",
				layout.defaultTerrain,
				typeof(HexTerrain),
				false) as HexTerrain;

		EditorGUILayout.Space();
		DifferentTerrainSection(layout);
		EditorGUILayout.Space();
	}

	void DifferentTerrainSection(MapLayout layout) {
		EditorGUILayout.LabelField("Different terrain instances:");
		if (layout.diffTerrain == null) {
			layout.diffTerrain = new Dictionary<HexTerrain, List<HexPos>>();
		}

		bool someExpandedTerrain = false;
		int i = 0;
		List<HexTerrain> terrToRemove = new List<HexTerrain>();
		foreach (KeyValuePair<HexTerrain, List<HexPos>> entry
			in layout.diffTerrain) {
			HexTerrain terrain = entry.Key;
			List<HexPos> pos = entry.Value;
			if (EditorGUILayout.Foldout(
				i == curExpandedTerr,
				terrain.type,
				true)) {

				curExpandedTerr = i;
				List<int> toRemove = new List<int>();
				for (int j = 0; j < pos.Count; j++) {
					EditorGUILayout.BeginHorizontal();
					pos[j].x = EditorGUILayout.IntField(pos[j].x);
					pos[j].y = EditorGUILayout.IntField(pos[j].y);
					if (GUILayout.Button("Remove")) {
						toRemove.Add(j);
					}
					EditorGUILayout.EndHorizontal();
				}
				toRemove.Reverse();
				foreach (int idx in toRemove) {
					pos.RemoveAt(idx);
				}
				if (pos.Count == 0) {
					terrToRemove.Add(terrain);
				} else {
					someExpandedTerrain = true;
				}
			}
			i++;
		}

		foreach (HexTerrain terrain in terrToRemove) {
			layout.diffTerrain.Remove(terrain);
		}

		if (!someExpandedTerrain) {
			curExpandedTerr = -1;
		}

		EditorGUILayout.Space();
		newTerrain = EditorGUILayout.ObjectField(
				"Add new terrain:",
				newTerrain,
				typeof(HexTerrain),
				false
			) as HexTerrain;
		EditorGUILayout.BeginHorizontal();
		newTerrainX = EditorGUILayout.IntField("Position:", newTerrainX);
		newTerrainY = EditorGUILayout.IntField(newTerrainY);
		EditorGUILayout.EndHorizontal();
		if (newTerrain != null) {
			if (GUILayout.Button("Add")) {
				if (!layout.diffTerrain.ContainsKey(newTerrain)) {
					layout.diffTerrain[newTerrain] = new List<HexPos>();
				}
				List<HexPos> list = layout.diffTerrain[newTerrain];
				list.Add(new HexPos(newTerrainX, newTerrainY));
			}
		}
	}
}
