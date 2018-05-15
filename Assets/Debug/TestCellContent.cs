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

			gm.AddUnit(prefabChar, -4, -1, Team.LEFT);
			gm.AddUnit(prefabChar, -1, -4, Team.LEFT);
			gm.AddUnit(prefabChar, -2, -2, Team.LEFT);
			gm.AddUnit(prefabOrb, -3, -3, Team.LEFT);

			gm.AddUnit(prefabChar, 4, 1, Team.RIGHT);
			gm.AddUnit(prefabChar, 2, 2, Team.RIGHT);
			gm.AddUnit(prefabChar, 1, 4, Team.RIGHT);
			gm.AddUnit(prefabOrb, 3, 3, Team.RIGHT);

			gm.AddUnit(prefabMountain, 0, 0);
			gm.AddUnit(prefabMountain, 3, -3);
			gm.AddUnit(prefabMountain, -3, 3);

			// foreach (HexUnit m in mountains) {
			// 	foreach (var cell in m.cell.Radius(1)) {
			// 		cell.ApplyEffect(Effect.STORM);
			// 	}
			// }
			foreach (HexCell cell in gm.grid[-4, 4].Line(2, 8, true, false)) {
				if (cell.unit == null) {
					cell.ApplyEffect(Effect.STORM);
				} else {
					cell.ApplyEffect(Effect.SNOW);
				}
			}

			gm.Begin();
		});
	}

}
