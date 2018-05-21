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

			foreach (var inst in MapLoader.layout.info) {
				HexPos pos = inst.Key;
				MapLayout.HexInfo info = inst.Value;

				HexCell cell = this[pos.x, pos.y];
				if (info.terrain != null) {
					cell.terrain = info.terrain;
				}

				switch (info.effect) {
					case MapLayout.Effect.FLAMES:
						cell.SetEffect(Effect.FLAMES);
						break;
					case MapLayout.Effect.SNOW:
						cell.SetEffect(Effect.SNOW);
						break;
					case MapLayout.Effect.STORM:
						cell.SetEffect(Effect.STORM);
						break;
				}

				if (info.content != null) {
					GameManager.GM.AddUnit(
						info.content, pos.x, pos.y, info.contentTeam);
				}
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
		cells[row, col] = cell;
	}

	public bool ValidCoords(int x, int y) {
		int row = y + nrows/2;
		int col = x + ncols/2;
		return row >= 0 && col >= 0
			&& row < nrows && col < ncols;
	}

}
