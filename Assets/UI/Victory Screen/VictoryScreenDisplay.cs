using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictoryScreenDisplay : MonoBehaviour {

	public Transform victorsLayout;
	public Transform losersLayout;

	public Sprite maleMageNormalSprite;
	public Sprite femaleMageNormalSprite;

	public GameObject prefabMaleVictor;
	public GameObject prefabFemaleVictor;
	public GameObject prefabMaleLoser;
	public GameObject prefabFemaleLoser;

	public void ShowWinner(HashSet<HexUnit>[] teams, Team winner) {
		var w = teams[(int) winner];
		var l = teams[(int) winner.Opposite()];

		PopulateList(victorsLayout, w, prefabMaleVictor, prefabFemaleVictor);
		PopulateList(losersLayout, l, prefabMaleLoser, prefabFemaleLoser);
		ColorSprites(victorsLayout, TeamExtensions.COLORS[(int) winner]);
		ColorSprites(losersLayout, TeamExtensions.COLORS[(int) winner.Opposite()]);
	}

	void PopulateList(
			Transform layout, HashSet<HexUnit> team,
			GameObject malePrefab, GameObject femalePrefab) {
		GameObject prefab = null;
		foreach (var unit in team) {
			if (unit.isMage) {
				if (IsMale(unit)) {
					prefab = malePrefab;
				} else {
					prefab = femalePrefab;
				}
				Instantiate(prefab, layout, false);
			}
		}
	}

	bool IsMale(HexUnit unit) {
		return
			unit.GetComponent<SpriteRenderer>().sprite == maleMageNormalSprite;
	}

	void ColorSprites(Transform layout, Color color) {
		foreach (Transform child in layout) {
			var image = child.Find("Overlay").GetComponent<Image>();
			image.color = color;
		}
	}

}
