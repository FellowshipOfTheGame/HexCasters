using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCellContent : MonoBehaviour {

	public GameObject prefabChar;
	public GameObject prefabMountain;
	public GameObject prefabOrb;

	void Start() {
		GameManager.AfterInit(delegate {
			GameManager gm = GameManager.GM;
			// HexGrid grid = gm.grid;

			gm.AddUnit(prefabChar, -4, -2, Team.LEFT);
			gm.AddUnit(prefabChar, -2, -4, Team.LEFT);
			gm.AddUnit(prefabChar, -2, -2, Team.LEFT);
			gm.AddUnit(prefabOrb, -3, -3, Team.LEFT);
			gm.AddUnit(prefabChar, 4, 2, Team.RIGHT);
			gm.AddUnit(prefabChar, 2, 2, Team.RIGHT);
			gm.AddUnit(prefabChar, 2, 4, Team.RIGHT);
			gm.AddUnit(prefabOrb, 3, 3, Team.RIGHT);

			List<HexUnit> mountains = new List<HexUnit>();
			mountains.Add(gm.AddUnit(prefabMountain, 0, 0).GetComponent<HexUnit>());
			mountains.Add(gm.AddUnit(prefabMountain, 3, -3).GetComponent<HexUnit>());
			mountains.Add(gm.AddUnit(prefabMountain, -3, 3).GetComponent<HexUnit>());

			foreach (HexUnit m in mountains) {
				foreach (var cell in m.cell.Radius(1)) {
					cell.ApplyEffect(Effect.STORM);
				}
			}

			gm.Begin();
		});
	}

}
