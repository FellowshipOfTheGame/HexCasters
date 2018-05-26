using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HexCell : MonoBehaviour {

	[SerializeField]
	private HexPos pos;
	// private int x, y, z;
	public int row, col;
	public HexTerrain terrain;
	[SerializeField]
	private GameObject _content;
	public GameObject content {
		get {
			return _content;
		}
		set {
			_content = value;
			if (content == null) {
				// unit.cell = null;
				unit = null;
			} else {
				unit = content.GetComponent<HexUnit>();
				if (content.transform.parent == null) {
					content.transform.SetParent(transform, false);
				}
				unit.cell = this;
			}
		}
	}
	public HexUnit unit {
		get;
		private set;
	}
	public string effect {
		get;
		private set;
	}

	public int X {
		get {
			return pos.x;
		}
		set {
			pos.x = value;
		}
	}
	public int Y {
		get {
			return pos.y;
		}
		set {
			pos.y = value;
		}
	}
	public int Z {
		get {
			return -X - Y;
		}
	}


	public Color highlight {
		get {
			return highlightRend.color;
		}
		set {
			highlightRend.color = value;
		}
	}

	public static int[][] directions = new int[][] {
		new int[] {0, 1},
		new int[] {1, 0},
		new int[] {1, -1},
		new int[] {0, -1},
		new int[] {-1, 0},
		new int[] {-1, 1}
	};

	public const float OUTER_RADIUS = 0.5f;
	// public const float INNER_RADIUS = OUTER_RADIUS * 0.86602540378f;// root(3)/2
	public const float INNER_RADIUS = OUTER_RADIUS * 61.0f / 64.0f;// root(3)/2 for 32x32 pixel art

	private SpriteRenderer highlightRend;
	private SpriteRenderer rend;
	private HexGrid grid;

	void Awake() {
		highlightRend = transform.Find("Highlight")
			.GetComponent<SpriteRenderer>();
		rend = GetComponent<SpriteRenderer>();
		highlight = Highlight.NONE;
		effect = Effect.NONE;
		GameManager.AfterInit(delegate { grid = GameManager.GM.grid; });
	}

	void Start() {
		rend.sprite = terrain.sprite;
	}

	public void Select(BaseEventData data) {
		PointerEventData pdata = data as PointerEventData;
		if (pdata.button == 0) {
			GameManager.GM.Click(this);
		}
	}

	public void StartHover() {
		GameManager.GM.HoverEnter(this);
	}

	public void EndHover() {
		GameManager.GM.HoverExit(this);
	}

	public int GetMovementCost() {
		int cost = terrain.movementCost;
		if (effect == Effect.SNOW) {
			cost++;
		}
		return cost;
	}

	public IEnumerable<HexCell> EnumerateNeighbors() {
		for (int i = 0; i < directions.Length; i++) {
			HexCell neigh = GetNeighbor(i);
			if (neigh != null) {
				yield return neigh;
			}
		}
	}

	public int DirectionTo(HexCell other) {
		if (other.X == this.X) {
			if (other.Y > this.Y) {
				return 0;
			} else {
				return 3;
			}
		} else if (other.Y == this.Y) {
			if (other.X > this.X) {
				return 1;
			} else {
				return 4;
			}
		} else if (other.Z == this.Z) {
			if (other.X > this.X) {
				return 2;
			} else {
				return 5;
			}
		} else {
			// cells are not aligned along any axis
			return -1;
		}
	}

	public HexCell GetNeighbor(int dir) {
		int[] delta = directions[dir];
		int neighX = X + delta[0];
		int neighY = Y + delta[1];
		if (grid.ValidCoords(neighX, neighY)) {
			return grid[neighX, neighY];
		}
		return null;
	}

	public override int GetHashCode() {
		return 113 * row + col;
	}

	public int ManhattanDistanceTo(HexCell cell) {
		return (Math.Abs(cell.X - this.X)
				+ Math.Abs(cell.Y - this.Y)
				+ Math.Abs(cell.Z - this.Z)) / 2;
	}

	public Area Radius(
			int r,
			bool blockedByTerrain=false,
			bool blockedByUnits=false) {
		Area a = new Area();
		Queue<HexCell> q = new Queue<HexCell>();
		Queue<int> qdist = new Queue<int>();
		q.Enqueue(this);
		qdist.Enqueue(0);
		while (q.Count > 0) {
			HexCell next = q.Dequeue();
			int nextDist = qdist.Dequeue();
			if (nextDist <= r) {
				a.Add(next);
				foreach (HexCell neigh in next.EnumerateNeighbors()) {
					if (a.Contains(neigh)) {
						// no reason to reexplore a cell,
						// skip
						continue;
					}

					if (blockedByUnits && neigh.content != null) {
						// cannot explore cell,
						// skip
						continue;
					}

					int inc;
					q.Enqueue(neigh);
					if (blockedByUnits) {
						inc = neigh.GetMovementCost();
					} else {
						inc = 1;
					}
					qdist.Enqueue(nextDist + inc);
				}
			}
		}

		return a;
	}

	public Area LineTo(HexCell dest, bool includeOrigin=false) {
		return Line(
			DirectionTo(dest),
			ManhattanDistanceTo(dest),
			includeOrigin);
	}

	public Area Line(
			int direction,
			int maxLength,
			bool includeOrigin=false,
			bool blockedByUnits=true) {
		Area a = new Area();
		HexCell cur = this;

		if (includeOrigin) {
			a.Add(this);
		}

		for (int i = 0; i < maxLength; i++) {
			cur = cur.GetNeighbor(direction);
			if (cur == null) {
				// end of grid or invalid direction
				break;
			}
			a.Add(cur);
			if (blockedByUnits && cur.content != null) {
				// found a unit
				break;
			}
		}

		return a;
	}

	public Area StormConnectedComponent() {
		if (effect != Effect.STORM) {
			return AsArea();
		}

		Area aoe = new Area();
		HexCell cur;
		Queue<HexCell> q = new Queue<HexCell>();
		q.Enqueue(this);
		while (q.Count > 0) {
			cur = q.Dequeue();
			aoe.Add(cur);
			foreach (HexCell neigh in cur.EnumerateNeighbors()) {
				if (neigh.effect == Effect.STORM && !aoe.Contains(neigh)) {
					q.Enqueue(neigh);
				}
			}
		}
		return aoe;
	}

	public Area AsArea() {
		Area a = new Area();
		a.Add(this);
		return a;
	}

	public void MoveContentTo(HexCell dest) {
		if (content == null || this == dest) {
			// nothing to do
			return;
		}
		content.transform.SetParent(dest.transform, false);
		dest.content = content;
		content = null;
	}

	public void SetEffect(string ef) {
		if (effect != Effect.NONE) {
			transform.Find(effect).gameObject.SetActive(false);
		}
		effect = ef;
		if (effect != Effect.NONE) {
			transform.Find(effect).gameObject.SetActive(true);
		}
	}

	public void ApplyFlames() {
		if (effect == Effect.SNOW || effect == Effect.STORM) {
			SetEffect(Effect.NONE);
		} else {
			SetEffect(Effect.FLAMES);
		}
	}

	public void ApplyStorm() {
		if (effect == Effect.FLAMES) {
			SetEffect(Effect.NONE);
		} else if (effect != Effect.SNOW) {
			SetEffect(Effect.STORM);
		}
	}

	public void ApplySnow() {
		if (effect == Effect.FLAMES) {
			SetEffect(Effect.NONE);
		} else {
			foreach (HexCell cell in StormConnectedComponent()) {
				cell.SetEffect(Effect.SNOW);
			}
		}
	}

	public void ApplyEffect(string ef) {
		if (ef == Effect.FLAMES) {
			ApplyFlames();
		} else if (ef == Effect.STORM) {
			ApplyStorm();
		} else if (ef == Effect.SNOW) {
			ApplySnow();
		}
	}


	private void UpdateName() {
		gameObject.name = String.Format(
			"Cell ({0:+0; -#}, {1:+0; -#}, {2:+0; -#})", X, Y, Z);
	}

}
