using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HexGrid : MonoBehaviour {

	private HexCell[,] cells;

	public HexCell prefabCell;
	public GameObject prefabMage;
	public GameObject prefabOrb;

	public int nrows;
	public int ncols;

	public const int ROW_Y_SPACING = 14;

	public HexCell this[int x, int y] {
		get {
			return cells[y + nrows/2, x + ncols/2];
		}
	}

	void Awake() {
		MapLayout layout = MapLoader.layout;
		nrows = layout.nrows;
		ncols = layout.ncols;
		cells = new HexCell[nrows, ncols];
		GameManager.AfterInit(delegate {
			for (int i = 0; i < nrows; i++) {
				for (int j = 0; j < ncols; j++) {
					SpawnCell(i, j);
				}
			}

			foreach (HexPos spawn in MapLoader.layout.spawnR) {
				GameManager.GM.AddUnit(
					prefabMage, spawn.x, spawn.y, Team.LEFT);
			}
			foreach (HexPos spawn in MapLoader.layout.spawnB) {
				GameManager.GM.AddUnit(
					prefabMage, spawn.x, spawn.y, Team.RIGHT);
			}
			GameManager.GM.AddUnit(
				prefabOrb, MapLoader.layout.orbR.x, MapLoader.layout.orbR.y,
				Team.LEFT);
			GameManager.GM.AddUnit(
				prefabOrb, MapLoader.layout.orbB.x, MapLoader.layout.orbB.y,
				Team.RIGHT);

			foreach (MapLayout.ObstacleInstance oi
				in MapLoader.layout.obstacles) {
				GameManager.GM.AddUnit(
					oi.obstacle, oi.pos.x, oi.pos.y, Team.NONE);
			}

			foreach (HexPos pos in MapLoader.layout.fire) {
				this[pos.x, pos.y].SetEffect(Effect.FLAMES);
			}
			foreach (HexPos pos in MapLoader.layout.rain) {
				this[pos.x, pos.y].SetEffect(Effect.STORM);
			}
			foreach (HexPos pos in MapLoader.layout.snow) {
				this[pos.x, pos.y].SetEffect(Effect.SNOW);
			}

			Vector3 camPos = this[0, 0].transform.position;
			camPos.z = Camera.main.transform.position.z;
			Camera.main.transform.position = camPos;

			MapLoader.LoadEnd();
		});

	}

	void SpawnCell(int row, int col) {
		HexTerrain terrain = MapLoader.layout.defaultTerrain;
		HexCell cell = Instantiate(prefabCell, transform);

		cell.X = col - ncols / 2;
		cell.Y = row - nrows / 2;
		cell.row = row;
		cell.col = col;
		cell.terrain = MapLoader.layout.Find(cell.X, cell.Y);
		if (cell.terrain == null) {
			cell.terrain = MapLoader.layout.defaultTerrain;
		}

		float ppu = terrain.sprite.pixelsPerUnit;
		float width = terrain.sprite.bounds.max.x - terrain.sprite.bounds.min.x;
		float height = terrain.sprite.bounds.max.y - terrain.sprite.bounds.min.y;

		Vector2 position = new Vector2();

		// I don't know, this works (kinda)
		// This is what we get for trying to use pixel art with hexagons
		position.x = (col + row/2.0f) * width - 2*col/ppu - row/ppu;
		position.y = row * (height - (ROW_Y_SPACING / ppu));

		cell.transform.position = position;
		cells[row, col] = cell;
	}

	public bool ValidCoords(int x, int y) {
		int row = y + nrows/2;
		int col = x + ncols/2;
		return row >= 0 && col >= 0
			&& row < nrows && col < ncols;
	}

}
