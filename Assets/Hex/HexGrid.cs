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

	public const int ROW_Y_SPACING = 14;

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
		// Vector3 camPos = Camera.main.transform.position;
		camPos.z = Camera.main.transform.position.z;
		Camera.main.transform.position = camPos;
	}

	void SpawnCell(int row, int col, HexTerrain terrain) {
		HexCell cell = Instantiate(prefabCell, transform);
		cell.terrain = terrain;
		float ppu = terrain.sprite.pixelsPerUnit;
		float width = terrain.sprite.bounds.max.x - terrain.sprite.bounds.min.x;
		float height = terrain.sprite.bounds.max.y - terrain.sprite.bounds.min.y;

		Vector2 position = new Vector2();

		// I don't know, this works (kinda)
		// This is what we get for trying to use pixel art with hexagons
		position.x = (col + row/2.0f) * width - 2*col/ppu - row/ppu;
		position.y = row * (height - (ROW_Y_SPACING / ppu));

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
