using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="HexType", menuName="New Hex type", order=1)]
public class HexTerrain : ScriptableObject {
	public string type;
	public SpriteSet spriteSet;
	public int movementCost = 1;
	public bool transponible = true;

	public Sprite sprite {
		get {
			return spriteSet.sprites[0];
		}
	}
}
