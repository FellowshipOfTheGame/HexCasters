using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="HexType", menuName="New Hex type", order=1)]
public class HexTerrain : ScriptableObject {
	public string type;
	public Sprite sprite;
	public int movementCost = 1;
}
