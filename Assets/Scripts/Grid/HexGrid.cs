using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HexGrid : MonoBehaviour {

	private HexCell[,] cells;

	public HexCell prefabCell;

	public int nrows;
	public int ncols;

	public HexTerrain terrainPlains;
	public HexTerrain terrainHills;

	public HexCell this[int x, int y] {
		get {
			return cells[y + nrows/2, x + ncols/2];
		}
	}

	void Awake() {
		GameManager.AfterInit(delegate {
			cells = new HexCell[nrows, ncols];
			for (int i = 0; i < nrows; i++) {
				for (int j = 0; j < ncols; j++) {
					// TODO read terrain info from somewhere
					// (map scriptable object?)
					SpawnCell(i, j, terrainPlains);
				}
			}
		});

		Vector3 camPos = this[0, 0].transform.position;
		camPos.z = Camera.main.transform.position.z;
		Camera.main.transform.position = camPos;
	}

	void SpawnCell(int row, int col, HexTerrain terrain) {
		HexCell cell = Instantiate(prefabCell, transform);
		cell.terrain = terrain;

		Vector2 position = new Vector2();
		position.x = (col * 2.0f + row) * HexCell.INNER_RADIUS;
		position.y = row * 1.5f * HexCell.OUTER_RADIUS;
		cell.transform.position = position;
		cell.X = col - ncols / 2;
		cell.Y = row - nrows / 2;
		cell.row = row;
		cell.col = col;
		cell.terrain = terrain;

		cells[row, col] = cell;
	}

	public bool ValidCoords(int x, int y) {
		int row = y + nrows/2;
		int col = x + ncols/2;
		return row >= 0 && col >= 0
			&& row < nrows && col < ncols;
	}

}
