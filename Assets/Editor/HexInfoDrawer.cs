using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(MapLayout.HexInfo))]
public class HexInfoDrawer : PropertyDrawer {

	private const int fieldHeight = 16;
	private const int nFields = 4;

	private SerializedProperty propTerrain;
	private SerializedProperty propEffect;
	private SerializedProperty propContent;
	private SerializedProperty propContTeam;

	public override void OnGUI(
		Rect position, SerializedProperty property, GUIContent label) {

		Rect rectFold = new Rect(
			position.x, position.y,
			position.width, fieldHeight);

		property.isExpanded = EditorGUI.Foldout(
			rectFold, property.isExpanded, label, true);
		if (property.isExpanded) {
			EditorGUI.indentLevel++;
			Rect rectTerrain = new Rect(
				position.x, position.y + 1*fieldHeight,
				position.width, fieldHeight);
			Rect rectEffect = new Rect(
				position.x, position.y + 2*fieldHeight,
				position.width, fieldHeight);
			Rect rectContent = new Rect(
				position.x, position.y + 3*fieldHeight,
				position.width, fieldHeight);
			Rect rectContTeam = new Rect(
				position.x, position.y + 4*fieldHeight,
				position.width, fieldHeight);

			propTerrain = property.FindPropertyRelative("terrain");
			propEffect = property.FindPropertyRelative("effect");
			propContent = property.FindPropertyRelative("content");
			propContTeam = property.FindPropertyRelative("contentTeam");

			EditorGUI.PropertyField(
				rectTerrain, propTerrain, new GUIContent("Terrain"));
			EditorGUI.PropertyField(
				rectEffect, propEffect, new GUIContent("Effect"));
			EditorGUI.PropertyField(
				rectContent, propContent, new GUIContent("Content"));
			EditorGUI.PropertyField(
				rectContTeam, propContTeam, new GUIContent("Team"));
			EditorGUI.indentLevel--;
		}
	}

	public override float GetPropertyHeight(
		SerializedProperty property, GUIContent label) {

		return base.GetPropertyHeight(property, label) * (property.isExpanded ? (nFields+1) : 1);
	}

}
