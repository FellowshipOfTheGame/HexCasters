using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// [CustomPropertyDrawer(typeof(HexPos))]
public class HexPosDrawer : PropertyDrawer {

	private float fieldHeight = 16f;

	public override void OnGUI(
		Rect position, SerializedProperty property, GUIContent label) {

		Rect rectX = new Rect(
			position.x, position.y,
			position.width, fieldHeight);
		Rect rectY = new Rect(
			position.x, position.y + 1*fieldHeight,
			position.width, fieldHeight);

		if (property.isExpanded) {
			SerializedProperty propX = property.FindPropertyRelative("x");
			SerializedProperty propY = property.FindPropertyRelative("y");

			EditorGUI.PropertyField(rectX, propX, new GUIContent("X"));
			EditorGUI.PropertyField(rectY, propY, new GUIContent("Y"));
		}
	}

	public override float GetPropertyHeight(
		SerializedProperty property, GUIContent label) {

		return 3*fieldHeight;
	}

}
