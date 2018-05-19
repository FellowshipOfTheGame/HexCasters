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

		nondefaultFold = EditorGUILayout.Foldout(
			nondefaultFold,
			"Nondefault terrains",
			true);
		if (nondefaultFold) {
			DifferentTerrainSection(layout);
			EditorGUILayout.Space();
		}
		// TODO team spawns
	}

	void DifferentTerrainSection(MapLayout layout) {
		if (layout.diffTerrain == null) {
			layout.diffTerrain = new MapLayout.DTList();
		}

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
}
